using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Species
{
    public List<NEAT_Brain> population;
    public int SpeciesID;
    public float averageFitness;
    public float adjustedFitness;
    public int N_offpsring;

    public Species(int SpeciesID)
    {
        this.SpeciesID = SpeciesID;
        population = new List<NEAT_Brain>();
    }

    public void AddToSpecies(NEAT_Brain brain)
    {
        population.Add(brain);
    }

    public void CalculateAdjustedFitness()
    {
        adjustedFitness = 0;
        foreach (NEAT_Brain brain in population)
        {
            adjustedFitness += brain.fitness;
        }
        adjustedFitness /= population.Count;
    }

    public void CalculateAverageFitness()
    {
        averageFitness = 0;
        foreach (NEAT_Brain brain in population)
        {
            averageFitness += brain.fitness;
        }
        averageFitness /= population.Count;
    }

}
