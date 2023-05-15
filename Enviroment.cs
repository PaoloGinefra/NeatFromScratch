using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Enviroment : MonoBehaviour
{
    public float[][] XORInputs = new float[][] { new float[] { 0, 0 }, new float[] { 0, 1 }, new float[] { 1, 0 }, new float[] { 1, 1 } };
    public int[] XOROutputs = new int[] { 0, 1, 1, 0 };

    public NEAT_Population population;

    NetworkDrawer networkDrawer;

    void Start()
    {
        networkDrawer = FindObjectOfType<NetworkDrawer>();

        population = new NEAT_Population(50, 3, 1, 1, 0.8f);
        foreach (NEAT_Brain brain in population.population)
        {
            EvaluateFitness(brain);
        }

        Debug.Log(population.species.Count);
        Debug.Log(population.population.Count);
    }

    void Update()
    {
        RunGeneration();
        Debug.Log("Pop size: " + population.population.Count);
        Debug.Log("Species size: " + population.species.Count);
        Debug.Log("threshold: " + population.speciationThreshold);
        logBest();
    }

    void logBest()
    {
        NEAT_Brain best = population.population.OrderByDescending(x => x.fitness).First();

        networkDrawer.brain = best;

        for (int i = 0; i < XORInputs.Length; i++)
        {
            best.LoadInput(XORInputs[i]);
            best.RunNetwork();
            float output = best.GetOutput()[0];
            Debug.Log(XORInputs[i][0] + " XOR " + XORInputs[i][1] + " = " + output + " | " + XOROutputs[i]);
        }

        Debug.Log("Fitness: " + best.fitness);
    }

    public void RunGeneration()
    {
        population.Speciate();
        population.CrossBreed();
        population.Mutate();
        foreach (NEAT_Brain brain in population.population)
        {
            EvaluateFitness(brain);
        }
    }

    public void EvaluateFitness(NEAT_Brain brain)
    {
        float fitness = 0;
        for (int i = 0; i < XORInputs.Length; i++)
        {
            brain.LoadInput(XORInputs[i]);
            brain.RunNetwork();
            float output = brain.GetOutput()[0];
            fitness += 1 - Mathf.Abs(output - XOROutputs[i]);
        }
        brain.fitness = fitness;
    }
}
