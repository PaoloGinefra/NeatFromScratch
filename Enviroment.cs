using System.Collections;
using System.Collections.Generic;
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

        population = new NEAT_Population(100, 3, 1, 1, 0.5f);
        RunGeneration();

        Debug.Log(population.species.Count);
    }

    int index = 0;
    void Update()
    {
        networkDrawer.brain = population.population[index];

        if (Input.GetKeyDown(KeyCode.Space))
        {
            index++;
            if (index >= population.population.Count)
            {
                index = 0;
            }

            for (int i = 0; i < XORInputs.Length; i++)
            {
                population.population[index].LoadInput(XORInputs[i]);
                population.population[index].RunNetwork();
                float output = population.population[index].GetOutput()[0];
                Debug.Log(XORInputs[i]);
                Debug.Log(output);
            }

        }
    }

    public void RunGeneration()
    {
        foreach (NEAT_Brain brain in population.population)
        {
            EvaluateFitness(brain);
        }
        population.Speciate();
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
        brain.fitness = Random.Range(0, 100);
    }
}
