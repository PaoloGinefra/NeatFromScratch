using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Species
{
    public List<NEAT_Brain> population;
    public NEAT_Brain representative;
    public int SpeciesID;
    public float averageFitness;
    public float adjustedFitness;
    public int N_offpsring;
    public int gensSinceImprovement;

    public Species(int SpeciesID, NEAT_Brain representative)
    {
        this.SpeciesID = SpeciesID;
        population = new List<NEAT_Brain>();
        this.representative = representative;
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
        float pastAvaregeFitness = averageFitness;
        averageFitness = 0;
        foreach (NEAT_Brain brain in population)
        {
            averageFitness += brain.fitness;
        }
        averageFitness /= population.Count;

        if (averageFitness > pastAvaregeFitness)
            gensSinceImprovement = 0;
        else
            gensSinceImprovement++;
    }

}
