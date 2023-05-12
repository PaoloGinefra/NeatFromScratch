using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NEAT_Brain
{
    public int inputSize;
    public int outputSize;
    public int hiddenSize;
    public float connectionPercentage;

    public List<Node> nodes;
    public List<Connection> connections;

    public void Initialise()
    {
        int nodeID = 0;
        // Create input nodes
        for (int i = 0; i < inputSize; i++)
        {
            nodes.Add(new Node(i, 0, 0));
        }
        nodeID += inputSize;

        // Create hidden nodes
        for (int i = 0; i < hiddenSize; i++)
        {
            nodes.Add(new Node(nodeID, 1, 1));
            nodeID++;
        }

        // Create output nodes
        for (int i = 0; i < outputSize; i++)
        {
            nodes.Add(new Node(nodeID, 2, 2));
            nodeID++;
        }

        // Create Connections
        for (int i = 0; i < inputSize; i++)
        {
            for (int j = 0; j < hiddenSize; j++)
            {
                if (Random.Range(0f, 1f) < connectionPercentage)
                {
                    nodes[i].connections.Add(new Connection(connectionID, nodes[i].id, nodes[j + inputSize].id, Random.Range(-1f, 1f), true, false));
                }
            }
        }

    }

    public void AddNode()
    {

    }

    public void AddConnection()
    {

    }

    public void Mutate()
    {

    }

    public void LoadInput()
    {

    }

    public void RunNetwork()
    {

    }
}
