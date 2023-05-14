using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class NEAT_Population
{
    public int populationSize;
    public List<NEAT_Brain> population;

    public int inputSize, outputSize, hiddenSize;
    public float connectionPercentage;

    public List<Species> species = new List<Species>();

    public float targetSpeciesNumber = 10;
    public float speciationThreshold = 0.5f;
    public float thresholdModifier = 0.1f;
    int speciesID = 0;

    public NEAT_Population(int populationSize, int inputSize, int outputSize, int hiddenSize, float connectionPercentage)
    {
        this.populationSize = populationSize;
        this.inputSize = inputSize;
        this.outputSize = outputSize;
        this.hiddenSize = hiddenSize;
        this.connectionPercentage = connectionPercentage;

        Initialise();
    }

    void Initialise()
    {
        population = new List<NEAT_Brain>();
        for (int i = 0; i < populationSize; i++)
        {
            population.Add(new NEAT_Brain(inputSize, outputSize, hiddenSize, connectionPercentage));
        }
    }

    public void Speciate()
    {
        for (int i = species.Count - 1; i >= 0; i--)
        {
            if (species[i].population.Count == 0)
            {
                species.RemoveAt(i);
                continue;
            }
            //get random representative
            species[i].representative = species[i].population[Random.Range(0, species[i].population.Count)];
            species[i].population.Clear();
        }

        //Update species
        foreach (Species species in species)
        {
            species.AddToSpecies(species.representative);
            population.Remove(species.representative);

            for (int i = population.Count - 1; i >= 0; i--)
            {
                float score = ComparisonCheck(species.representative, population[i]);
                if (score < speciationThreshold)
                {
                    species.AddToSpecies(population[i]);
                    population.Remove(population[i]);
                }
            }

            species.CalculateAverageFitness();
            species.CalculateAdjustedFitness();
        }

        // Speciate remaining population
        while (population.Count > 0)
        {
            NEAT_Brain currentBrain = population[Random.Range(0, population.Count)];
            species.Add(new Species(speciesID, currentBrain));
            species[species.Count - 1].AddToSpecies(currentBrain);
            population.Remove(currentBrain);

            for (int i = population.Count - 1; i >= 0; i--)
            {
                float score = ComparisonCheck(currentBrain, population[i]);
                if (score < speciationThreshold)
                {
                    species[species.Count - 1].AddToSpecies(population[i]);
                    population.Remove(population[i]);
                }
            }

            species[species.Count - 1].CalculateAverageFitness();
            species[species.Count - 1].CalculateAdjustedFitness();

            speciesID++;
        }

        // Update threshold
        if (species.Count < targetSpeciesNumber)
        {
            speciationThreshold -= thresholdModifier;
        }
        else if (species.Count > targetSpeciesNumber)
        {
            speciationThreshold += thresholdModifier;
        }

        // Compute avarage adjusted fitness
        float averageAdjustedFitness = 0;
        foreach (Species species in species)
        {
            averageAdjustedFitness += species.adjustedFitness;
        }

        // Calculate N_offspring
        foreach (Species species in species)
        {
            species.N_offpsring = Mathf.FloorToInt(species.adjustedFitness / averageAdjustedFitness * populationSize);
        }
    }

    float ComparisonCheck(NEAT_Brain brain1, NEAT_Brain brain2)
    {
        // Comparison Check = Execess Genes + Disjoint Genes + Weight Difference

        if (brain1.connections.Count == 0 && brain2.connections.Count == 0)
        {
            return 100;
        }

        // Excess Genes + Disjoint Genes
        int maxInvvoationID1 = brain1.connections.Count != 0 ? brain1.connections.Max(x => x.innovationID) : 0;
        int maxInvvoationID2 = brain2.connections.Count != 0 ? brain2.connections.Max(x => x.innovationID) : 0;

        int[] InnovationIDs = new int[Mathf.Max(maxInvvoationID1, maxInvvoationID2) + 1];

        foreach (Connection connection in brain1.connections)
        {
            if (connection.enabled)
                InnovationIDs[connection.innovationID] += 1;
        }

        foreach (Connection connection in brain2.connections)
        {
            if (connection.enabled)
                InnovationIDs[connection.innovationID] += 1;
        }

        int excessGenesAndDisjointGenes = InnovationIDs.Count(x => x == 1);

        //normalize by dividing by the total number of genes
        excessGenesAndDisjointGenes /= InnovationIDs.Count(x => x != 0);

        // Weight Difference
        float averageWeightDifference = 0;

        foreach (Connection connection1 in brain1.connections)
        {
            foreach (Connection connection2 in brain2.connections)
            {
                if (connection1.innovationID == connection2.innovationID)
                {
                    averageWeightDifference += Mathf.Abs(connection1.weight - connection2.weight);
                    break;
                }
            }
        }

        averageWeightDifference /= InnovationIDs.Count(x => x == 2);

        return excessGenesAndDisjointGenes + averageWeightDifference;
    }

    NEAT_Brain ChooseParent(Species species)
    {
        float totalFitness = 0;
        foreach (NEAT_Brain brain in species.population)
        {
            totalFitness += brain.fitness;
        }

        float random = Random.Range(0, totalFitness);
        float runningSum = 0;

        foreach (NEAT_Brain brain in species.population)
        {
            runningSum += brain.fitness;
            if (runningSum > random)
            {
                return brain;
            }
        }

        return null;
    }

    void CrossBreedSpecies(Species species)
    {
        List<NEAT_Brain> newSpeciesPopulation = new List<NEAT_Brain>();
        for (int i = 0; i < species.N_offpsring; i++)
        {
            NEAT_Brain parent1 = ChooseParent(species);
            NEAT_Brain parent2 = ChooseParent(species);

            if (parent1.fitness < parent2.fitness)
            {
                NEAT_Brain temp = parent1;
                parent1 = parent2;
                parent2 = temp;
            }

            NEAT_Brain child = new NEAT_Brain(parent1);

            //Chose randomly connection weights bwtween matching connections in parents
            foreach (Connection connection in child.connections)
            {
                if (parent2.connections.Exists(x => x.innovationID == connection.innovationID))
                {
                    if (Random.Range(0, 2) == 0)
                    {
                        connection.weight = parent2.connections.Find(x => x.innovationID == connection.innovationID).weight;
                    }
                }
            }
            population.Add(child);
            newSpeciesPopulation.Add(child);
        }

        species.population.Clear();
        species.population.AddRange(newSpeciesPopulation);
    }

    public void CrossBreed()
    {
        // Cross breed
        foreach (Species species in species)
        {
            CrossBreedSpecies(species);
        }

        // Fill up population with random species
        while (population.Count < populationSize)
        {
            population.Add(new NEAT_Brain(inputSize, outputSize, hiddenSize, connectionPercentage));
        }
    }
}
