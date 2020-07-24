""" Game controller for live game on dream or real (planning or manual play)"""
#  Copyright (c) 2020, - All Rights Reserved
#  This file is part of the Evolutionary Planning on a Learned World Model thesis.
#  Unauthorized copying of this file, via any medium is strictly prohibited without the consensus of the authors.
#  Written by Thor V.A.N. Olesen <thorolesen@gmail.com> & Dennis T.T. Nguyen <dennisnguyen3000@yahoo.dk>.

import torch
import numpy as np
from utility.visualizer import Visualizer
from environment.simulated_environment import SimulatedEnvironment


class SimulatedPlanningController:
    def __init__(self, config, preprocessor, vae, mdrnn):
        self.config = config
        self.preprocessor = preprocessor
        self.action = np.array([0., 0., 0.])
        self.vae = vae
        self.mdrnn = mdrnn
        self.simulated_environment = SimulatedEnvironment(self.config, self.vae, self.mdrnn)
        self.visualizer = Visualizer()
        self.is_dream_play = config['is_dream_play']

    # ##Manual controls
    def on_key_press(self, event):
        """ Defines key pressed behavior """
        if event.key == 'up':
            self.action[1] = 1
        if event.key == 'down':
            self.action[2] = .8
        if event.key == 'left':
            self.action[0] = -1
        if event.key == 'right':
            self.action[0] = 1

    def on_key_release(self, event):
        """ Defines key pressed behavior """
        if event.key == 'up':
            self.action[1] = 0
        if event.key == 'down':
            self.action[2] = 0
        if event.key == 'left' and self.action[0] == -1:
            self.action[0] = 0
        if event.key == 'right' and self.action[0] == 1:
            self.action[0] = 0
    # ##

    def _encode_state(self, state):
        state = self.preprocessor.resize_frame(state).unsqueeze(0)
        reconstruction, z_mean, z_log_standard_deviation = self.vae(state)
        latent_state = self.vae.sample_reparametarization(z_mean, z_log_standard_deviation)
        return latent_state, reconstruction

    def _synchronize_simulated_environment(self, current_state, action, hidden_state=None):
        latent_state, decoded_state = self._encode_state(current_state)
        z, r, d, h = self.simulated_environment.step(action, hidden_state_h=hidden_state, latent_state_z=latent_state, is_simulation_real_environment=True)
        return z, r, h  # next latent, reward, next hidden

    def play_game(self, agent, environment):
        with torch.no_grad():
            self.simulated_environment.reset()
            self.simulated_environment.figure.canvas.mpl_connect('key_press_event', lambda event: self.on_key_press(event))
            self.simulated_environment.figure.canvas.mpl_connect('key_release_event', lambda event: self.on_key_release(event))

            is_done = False
            total_steps, total_reward, total_simulated_reward = 0, 0, 0

            current_state = environment.reset()
            for _ in range(50):  # Skip zoom
                current_state, _, _, _ = environment.step(self.action)
            environment.render()

            latent_state, simulated_reward, hidden_state = self._synchronize_simulated_environment(current_state, self.action)

            while not is_done:
                if not self.config['is_manual_control']:
                    if self.config['planning']['planning_agent'] == "MCTS":
                        self.action = agent.search(self.simulated_environment, latent_state, hidden_state)
                    else:
                        self.action, elites = agent.search(self.simulated_environment, latent_state, hidden_state)

                if self.is_dream_play:
                    latent_state, simulated_reward, is_done, hidden_state = self.simulated_environment.step(self.action, is_simulation_real_environment=True)
                    self.simulated_environment.render()
                    total_reward += simulated_reward
                    total_simulated_reward = total_reward
                else:
                    current_state, reward, is_done, _ = environment.step(self.action)
                    latent_state, simulated_reward, hidden_state = self._synchronize_simulated_environment(current_state, self.action, hidden_state)
                    total_reward += reward
                    total_simulated_reward += simulated_reward
                    environment.render()
                    self.simulated_environment.render()

                total_steps += 1
                print(f"Reward: {round(total_reward)} | Sim. reward: {round(total_simulated_reward)} | {total_steps} steps")

            return total_steps, total_reward, total_simulated_reward

