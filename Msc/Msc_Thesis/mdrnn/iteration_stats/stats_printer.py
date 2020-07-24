#  Copyright (c) 2020, - All Rights Reserved
#  This file is part of the Evolutionary Planning on a Learned World Model thesis.
#  Unauthorized copying of this file, via any medium is strictly prohibited without the consensus of the authors.
#  Written by Thor V.A.N. Olesen <thorolesen@gmail.com> & Dennis T.T. Nguyen <dennisnguyen3000@yahoo.dk>.

import pickle
from os.path import exists

filename = 'iterative_stats_World_Model_I.pickle'
if exists(filename):
    with open(f'{filename}', 'rb') as file:
        stats_data = pickle.load(file)
        print(stats_data)

else:
    print(f'File not found: {filename}')