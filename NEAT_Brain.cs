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

    public NEAT_Brain(int inputSize, int outputSize, int hiddenSize, float connectionPercentage)
    {
        this.inputSize = inputSize;
        this.outputSize = outputSize;
        this.hiddenSize = hiddenSize;
        this.connectionPercentage = connectionPercentage;

        nodes = new List<Node>();
        connections = new List<Connection>();

        Initialise();
    }

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
        if (hiddenSize != 0)
        {
            for (int i = 0; i < inputSize; i++)
            {
                for (int j = 0; j < hiddenSize; j++)
                {
                    if (Random.Range(0f, 1f) < connectionPercentage)
                    {
                        connections.Add(new Connection(nodes[i].id, nodes[j + inputSize].id, Random.Range(-1f, 1f), true, false));
                    }
                }
            }

            for (int i = 0; i < hiddenSize; i++)
            {
                for (int j = 0; j < outputSize; j++)
                {
                    if (Random.Range(0f, 1f) < connectionPercentage)
                    {
                        connections.Add(new Connection(nodes[i + inputSize].id, nodes[j + inputSize + hiddenSize].id, Random.Range(-1f, 1f), true, false));
                    }
                }
            }
        }
        else
            for (int i = 0; i < inputSize; i++)
            {
                for (int j = 0; j < outputSize; j++)
                {
                    if (Random.Range(0f, 1f) < connectionPercentage)
                    {
                        connections.Add(new Connection(nodes[i].id, nodes[j + inputSize].id, Random.Range(-1f, 1f), true, false));
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

    public void LoadInput(List<float> input)
    {
        for (int i = 0; i < input.Count; i++)
        {
            nodes[i].sumInput = input[i];
        }
    }

    public void RunNetwork()
    {

    }
}
