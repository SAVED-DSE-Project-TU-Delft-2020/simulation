import sys
sys.path.append('..')

from simulation.parameters import concept_A, concept_B, concept_C
from simulation.model_1 import get_range

def test_ranges():

    assert get_range(concept_A) > 150000, 'Range of concept A is insufficient'
    assert get_range(concept_B) > 150000, 'Range of concept B is insufficient'
    assert get_range(concept_C) > 150000, 'Range of concept C is insufficient'

