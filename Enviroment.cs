using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enviroment : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void EvaluateFitness(NEAT_Brain brain)
    {
        brain.fitness = Random.Range(0, 100);
    }
}
