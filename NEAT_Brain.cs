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

    public float fitness;
    public int SpeciesID;

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

    //Copy Constructor
    public NEAT_Brain(NEAT_Brain brain)
    {
        inputSize = brain.inputSize;
        outputSize = brain.outputSize;
        hiddenSize = brain.hiddenSize;
        connectionPercentage = brain.connectionPercentage;

        nodes = new List<Node>();
        connections = new List<Connection>();

        foreach (Node node in brain.nodes)
        {
            nodes.Add(new Node(node));
        }

        foreach (Connection connection in brain.connections)
        {
            connections.Add(new Connection(connection));
        }
    }

    public void Initialise()
    {
        int nodeID = 0;
        // Create input nodes
        for (int i = 0; i < inputSize - 1; i++)
        {
            nodes.Add(new Node(i, 0, 0));
        }

        //add bias node
        Node BiasNode = new Node(inputSize - 1, 3, 0);
        BiasNode.sumInput = 1; //bias node always outputs 1
        nodes.Add(BiasNode);

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
                        connections.Add(new Connection(nodes[i].id, nodes[j + inputSize].id, Random.Range(-5f, 5f), true, false));
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

    public void LoadInput(float[] input)
    {
        for (int i = 0; i < input.Length; i++)
        {
            nodes[i].sumInput = input[i];
            nodes[i].sumOutput = input[i];
        }
    }

    public void RunNetwork()
    {
        for (int layer = 1; layer < 3; layer++)
        {
            for (int i = 0; i < nodes.Count; i++)
            {
                if (nodes[i].layer == layer)
                {
                    nodes[i].sumInput = 0;
                    for (int j = 0; j < connections.Count; j++)
                    {
                        if (connections[j].toNode == nodes[i].id)
                        {
                            nodes[i].sumInput += connections[j].weight * nodes[connections[j].fromNode].sumOutput;
                        }
                    }
                    nodes[i].sumOutput = activationFunction(nodes[i].sumInput);
                }
            }
        }
    }

    float activationFunction(float x)
    {
        return 1 / (1 + Mathf.Exp(-x));
    }

    public List<float> GetOutput()
    {
        List<float> output = new List<float>();
        for (int i = 0; i < nodes.Count; i++)
        {
            if (nodes[i].type == 2)
                output.Add(nodes[i].sumOutput);
        }
        return output;
    }
}
