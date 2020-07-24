""" MDRNN Trainer responsible for training and reloading MDRNNs
    Based on: https://github.com/ctallec/world-models/blob/master/trainmdrnn.py
"""
#  Copyright (c) 2020, - All Rights Reserved
#  This file is part of the Evolutionary Planning on a Learned World Model thesis.
#  Unauthorized copying of this file, via any medium is strictly prohibited without the consensus of the authors.
#  Written by Thor V.A.N. Olesen <thorolesen@gmail.com> & Dennis T.T. Nguyen <dennisnguyen3000@yahoo.dk>.

import torch
import numpy as np
import multiprocessing
import torch.nn.functional as f
from os import mkdir
from tqdm import tqdm
from torch import optim
from functools import partial
from os.path import join, exists
from torchvision import transforms
from torch.distributions import Normal
from torch.utils.data import DataLoader
from mdrnn.learning import ReduceLROnPlateau
from utility.loaders import RolloutSequenceDataset
from mdrnn.learning import EarlyStopping


def transform(frames):
    # 0=batch size, 3=img channels, 1 and 2 = img dims, / 255 normalize
    transform = transforms.Lambda(lambda img: np.transpose(img, (0, 3, 1, 2)) / 255)
    return transform(frames)


# Loss for gaussian mixture model
def gmm_loss(batch, mus, sigmas, logpi, reduce=True):
    batch = batch.unsqueeze(-2)
    normal_dist = Normal(mus, sigmas)
    g_log_probs = normal_dist.log_prob(batch)
    g_log_probs = logpi + torch.sum(g_log_probs, dim=-1)
    max_log_probs = torch.max(g_log_probs, dim=-1, keepdim=True)[0]
    g_log_probs = g_log_probs - max_log_probs
    g_probs = torch.exp(g_log_probs)
    probs = torch.sum(g_probs, dim=-1)

    log_prob = max_log_probs.squeeze() + torch.log(probs)
    if reduce:
        return - torch.mean(log_prob)
    return - log_prob


class MDRNNTrainer:
    def __init__(self, config, preprocessor, logger):
        self.config = config
        self.device = torch.device('cuda' if torch.cuda.is_available() else 'cpu')
        self.logger = logger
        self.preprocessor = preprocessor
        self.vae = None
        self.mdrnn = None

        self.session_name = 'mdrnn_'+self.config['experiment_name']
        self.model_dir = self.config['mdrnn_dir']
        self.data_dir = self.config['data_dir']
        self.best_mdrnn_filename = self.config['experiment_name'] + '_' + self.config["mdrnn_trainer"]["mdrnn_best_filename"]
        self.checkpoint_filename = self.config['experiment_name'] + '_' + self.config["mdrnn_trainer"]["mdrnn_checkpoint_filename"]
        if not exists(self.model_dir):
            mkdir(self.model_dir)

        self.latent_size = self.config["latent_size"]
        self.batch_size = self.config["mdrnn_trainer"]["batch_size"]
        self.sequence_length = self.config["mdrnn_trainer"]["sequence_length"]

        self.num_workers = self.config['mdrnn_trainer']['num_workers']
        self.num_workers = self.num_workers if self.num_workers <= multiprocessing.cpu_count() else multiprocessing.cpu_count()

        self.optimizer = None
        self.scheduler = None
        self.earlystopping = None

        self.train_loader = None
        self.test_loader = None

        self.is_iterative = self.config["is_iterative_train_mdrnn"] and not self.config["is_train_mdrnn"]

    def train(self, vae, mdrnn, data_dir=None, max_epochs=None):
        self.data_dir = self.data_dir if data_dir is None else data_dir
        self.vae = vae.to(self.device)
        self.mdrnn = mdrnn.to(self.device)
        self._load_data()
        self.logger.is_logging = not self.is_iterative
        self.logger.start_log_training_minimal(name=self.session_name)
        self.optimizer = optim.Adam(self.mdrnn.parameters(), lr=self.config['mdrnn_trainer']['learning_rate'])
        self.scheduler = ReduceLROnPlateau(self.optimizer, 'min', factor=0.5, patience=5)
        self.earlystopping = EarlyStopping('min', patience=30)

        train = partial(self._data_pass, is_train=True, include_reward=True)
        test = partial(self._data_pass, is_train=False, include_reward=True)

        start_epoch, current_best = self._reload_training_session()
        max_epochs = self.config['mdrnn_trainer']['max_epochs'] if max_epochs is None else max_epochs

        if start_epoch > max_epochs:
            raise Exception(f'Inconsistent start epoch {start_epoch} and max_epoch {max_epochs}')

        test_loss = 0
        for epoch in range(start_epoch, max_epochs + 1):
            train(epoch)
            test_loss = test(epoch)
            self.scheduler.step(test_loss)
            self.earlystopping.step(test_loss)

            is_best = not current_best or test_loss < current_best
            current_best = test_loss if is_best else current_best

            self._save_checkpoint({'epoch': epoch, 'state_dict': self.mdrnn.state_dict(),
                                   'precision': test_loss, 'optimizer': self.optimizer.state_dict(),
                                   'scheduler': self.scheduler.state_dict(),
                                   'earlystopping': self.earlystopping.state_dict()
                                   }, is_best)
            if self.earlystopping.stop:
                print(f"End of Training because of early stopping at epoch {epoch}")
                break
        self.logger.end_log_training('mdrnn')
        return self.mdrnn, test_loss

    def reload_model(self, mdrnn):
        reload_file = join(self.model_dir, f'checkpoints/{self.best_mdrnn_filename}')
        if not exists(reload_file):
            raise Exception('No MDRNN model found...')
        state = torch.load(reload_file) if torch.cuda.is_available() else torch.load(reload_file, map_location=torch.device('cpu'))
        mdrnn.load_state_dict(state['state_dict'])
        print(f'Reloaded MDRNN model - {state["epoch"]}')
        return mdrnn

    def _reload_training_session(self):
        reload_file = join(self.model_dir, f"checkpoints/{'iterative_' if self.is_iterative else ''}{self.checkpoint_filename}")
        best_file = join(self.model_dir, f"checkpoints/{'iterative_' if self.is_iterative else ''}{self.best_mdrnn_filename}")

        reload_file = reload_file if exists(reload_file) and not self.is_iterative else best_file

        if exists(reload_file) and self.config['mdrnn_trainer']['is_continue_model']:
            state = torch.load(reload_file) if torch.cuda.is_available() else torch.load(reload_file, map_location=torch.device('cpu'))
            best_test_loss = None
            if exists(best_file):
                best_state = torch.load(best_file) if torch.cuda.is_available() else torch.load(best_file, map_location=torch.device('cpu'))
                best_test_loss = best_state['precision']
                print(f"Reloading mdrnn at epoch {state['epoch']}, with best test error {best_test_loss} at epoch {best_state['epoch']}")
            else:
                print(f"Reloading mdrnn at epoch {state['epoch']}")

            self.mdrnn.load_state_dict(state["state_dict"])
            self.optimizer.load_state_dict(state["optimizer"])
            self.scheduler.load_state_dict(state['scheduler'])
            self.earlystopping.load_state_dict(state['earlystopping'])
            return state['epoch'], best_test_loss
        print('No mdrnn found. Skip reloading...')
        return 1, None  # start epoch

    def _load_data(self):  # To avoid loading data when not training
        train_dataset = RolloutSequenceDataset(root=self.data_dir,
                                               seq_len=self.config['mdrnn_trainer']['sequence_length'],
                                               transform=transform, train=True,
                                               buffer_size=self.config['mdrnn_trainer']['train_buffer_size'])

        test_dataset = RolloutSequenceDataset(root=self.data_dir,
                                              seq_len=self.config['mdrnn_trainer']['sequence_length'],
                                              transform=transform, train=False,
                                              buffer_size=self.config['mdrnn_trainer']['test_buffer_size'])

        self.train_loader = DataLoader(dataset=train_dataset,
                                       batch_size=self.config['mdrnn_trainer']['batch_size'],
                                       num_workers=self.num_workers, shuffle=True)

        self.test_loader = DataLoader(dataset=test_dataset,
                                      batch_size=self.config['mdrnn_trainer']['batch_size'],
                                      num_workers=self.num_workers, shuffle=False)

    def _to_latent(self, obs, next_obs):
        """ Transform observations to latent space.  """
        image_height, image_width = self.config['preprocessor']['img_height'], self.config['preprocessor']['img_width']
        image_reduction_size = self.config['preprocessor']['img_width']

        with torch.no_grad():
            obs, next_obs = [
                f.interpolate(x.view(-1, 3, image_height, image_width), size=image_reduction_size,
                           mode='bilinear', align_corners=True)
                for x in (obs, next_obs)]

            (obs_mu, obs_logsigma), (next_obs_mu, next_obs_logsigma) = [
                self.vae(x)[1:] for x in (obs, next_obs)]

            latent_obs, latent_next_obs = [
                (x_mu + x_logsigma.exp() * torch.randn_like(x_mu)).view(self.batch_size, self.sequence_length, self.latent_size)
                for x_mu, x_logsigma in
                [(obs_mu, obs_logsigma), (next_obs_mu, next_obs_logsigma)]]
        return latent_obs, latent_next_obs

    def _get_loss(self, latent_obs, action, reward, terminal, latent_next_obs, include_reward: bool):
        latent_obs, action, reward, terminal, latent_next_obs = [arr.transpose(1, 0)
                                                                for arr in
                                                                [latent_obs, action, reward, terminal, latent_next_obs]]
        mus, sigmas, logpi, rs, ds, _ = self.mdrnn(action, latent_obs)
        gmm = gmm_loss(latent_next_obs, mus, sigmas, logpi)
        bce = f.binary_cross_entropy_with_logits(ds, terminal)
        if include_reward:
            mse = f.mse_loss(rs, reward)
            scale = self.latent_size + 2
        else:
            mse = 0
            scale = self.latent_size + 1
        loss = (gmm + bce + mse) / scale
        return dict(gmm=gmm, bce=bce, mse=mse, loss=loss)

    def _data_pass(self, epoch, is_train, include_reward):
        """ One pass through the data """
        if is_train:
            self.mdrnn.train()
            loader = self.train_loader
        else:
            self.mdrnn.eval()
            loader = self.test_loader

        loader.dataset.load_next_buffer()

        cum_loss = 0
        cum_gmm = 0
        cum_bce = 0
        cum_mse = 0

        progress_bar = tqdm(total=len(loader.dataset) if len(loader.dataset) >= 0 else 1, desc=f"{'Train' if is_train else 'Test'} Epoch {epoch}")
        for i, data in enumerate(loader):
            obs, action, reward, terminal, next_obs = [arr.to(self.device) for arr in data]
            if obs.shape[0] is not self.batch_size:
                print('Skipped bad data')
                progress_bar.update(self.batch_size)
                continue  # Skip bad data from corner

            # transform obs to latent states
            latent_obs, latent_next_obs = self._to_latent(obs, next_obs)

            if is_train:
                losses = self._get_loss(latent_obs, action, reward, terminal, latent_next_obs, include_reward)
                self.optimizer.zero_grad()
                losses['loss'].backward()
                torch.nn.utils.clip_grad_norm_(parameters=self.mdrnn.parameters(), max_norm=self.config["mdrnn_trainer"]["gradient_clip"])
                self.optimizer.step()
            else:
                with torch.no_grad():
                    losses = self._get_loss(latent_obs, action, reward, terminal, latent_next_obs, include_reward)

            cum_loss += losses['loss'].item()
            cum_gmm += losses['gmm'].item()
            cum_bce += losses['bce'].item()
            cum_mse += losses['mse'].item() if hasattr(losses['mse'], 'item') else losses['mse']

            loss = cum_loss / (i + 1)
            bce = cum_bce / (i + 1)
            gmm = cum_gmm / self.config['latent_size'] / (i + 1)
            mse = cum_mse / (i + 1)
            progress_bar.set_postfix_str(f"loss={loss} bce={bce} gmm={gmm} mse={mse}")
            progress_bar.update(self.batch_size)

        progress_bar.close()

        if len(loader.dataset) <= 0:
            print(f"Issue with data loader size {len(loader.dataset)}")
            return

        average_loss = cum_loss * self.batch_size / len(loader.dataset)

        self.logger.log_loss('mdrnn', average_loss, epoch, is_train=is_train)
        return average_loss

    def _save_checkpoint(self, state, is_best):
        best_model_filename = join(self.model_dir, f"checkpoints/{'iterative_' if self.is_iterative else ''}{self.best_mdrnn_filename}")
        checkpoint_filename = join(self.model_dir, f"checkpoints/{'iterative_' if self.is_iterative else ''}{self.checkpoint_filename}")
        torch.save(state, checkpoint_filename)
        if is_best or self.is_iterative:
            torch.save(state, best_model_filename)
            print(f'New best model found and saved')

