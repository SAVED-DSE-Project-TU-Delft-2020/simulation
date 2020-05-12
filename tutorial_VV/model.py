def get_range(concept):
    energy = concept['battery_mass']*concept['energy_density']
    flight_time = energy/concept['pow_cruise']
    total_range = flight_time * concept['vel_cruise']
    return total_range
