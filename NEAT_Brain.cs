using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NEAT_Brain
{
    public int inputSize;
    public int outputSize;
    public int hiddenSize;
    public float connectionPercentage;
    public float weightRange = 20f;

    public float connectionMutationRate = 0.8f;
    public float connectionRandomConnectionMutationRate = 0.1f;
    public float connectionWeightMutationRange = 0.2f;
    public float connectionReanableRate = 0.25f;

    public float addNodeMutationRate = 0.1f;

    public float addConnectionMutationRate = 0.2f;

    public List<Node> nodes;
    public List<Connection> connections;

    public float fitness;
    public int SpeciesID;

    public int n_layers = 3;

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

        n_layers = brain.n_layers;
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
                        connections.Add(new Connection(nodes[i].id, nodes[j + inputSize].id, Random.Range(-weightRange, weightRange), true, false));
                    }
                }
            }

            for (int i = 0; i < hiddenSize; i++)
            {
                for (int j = 0; j < outputSize; j++)
                {
                    if (Random.Range(0f, 1f) < connectionPercentage)
                    {
                        connections.Add(new Connection(nodes[i + inputSize].id, nodes[j + inputSize + hiddenSize].id, Random.Range(-weightRange, weightRange), true, false));
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
        if (connections.Count == 0)
        {
            return;
        }

        //Choose a random connection and disable it
        int connectionIndex = Random.Range(0, connections.Count);
        connections[connectionIndex].enabled = false;

        //Create new node
        Node newNode = new Node(nodes.Count, 1, nodes[connections[connectionIndex].fromNode].layer + 1);

        //Create 2 new connections
        connections.Add(new Connection(connections[connectionIndex].fromNode, newNode.id, 1, true, false));
        connections.Add(new Connection(newNode.id, connections[connectionIndex].toNode, connections[connectionIndex].weight, true, false));

        //Add new node to list
        nodes.Add(newNode);

        //Adjust layers
        AdjustLayers(newNode.id, newNode.layer);

        //Adjust output layer
        AdjustOuputLayer();
    }

    void AdjustLayers(int nodeID, int layer)
    {
        foreach (Connection connection in connections)
        {
            if (connection.fromNode == nodeID)
            {
                if (nodes[connection.toNode].layer <= layer)
                {
                    nodes[connection.toNode].layer = layer + 1;
                    AdjustLayers(connection.toNode, layer + 1);
                }
            }
        }
    }

    void AdjustOuputLayer()
    {
        int maxLayer = 0;
        bool isOutput = false;
        foreach (Node node in nodes)
        {
            if (node.layer > maxLayer)
            {
                maxLayer = node.layer;
                isOutput = node.type == 2;
            }
        }

        foreach (Node node in nodes)
        {
            if (node.type == 2)
            {
                node.layer = maxLayer + (isOutput ? 0 : 1);
            }
        }

        n_layers = maxLayer + 1 + (isOutput ? 0 : 1);
    }


    public void AddConnection()
    {
        //Choose 2 random nodes
        int node1 = Random.Range(0, nodes.Count);
        int node2 = Random.Range(0, nodes.Count);

        while (nodes[node1].layer == nodes[node2].layer)
        {
            node2 = Random.Range(0, nodes.Count);
        }

        //if node1 is after node2, swap them
        if (nodes[node1].layer > nodes[node2].layer)
        {
            int temp = node1;
            node1 = node2;
            node2 = temp;
        }

        // Check if connection already exists
        bool connectionExists = false;
        int connectionIndex = 0;
        for (connectionIndex = 0; connectionIndex < connections.Count; connectionIndex++)
        {
            if (connections[connectionIndex].fromNode == nodes[node1].id && connections[connectionIndex].toNode == nodes[node2].id)
            {
                connectionExists = true;
                break;
            }
        }

        if (!connectionExists)
        {
            connections.Add(new Connection(nodes[node1].id, nodes[node2].id, Random.Range(-weightRange, weightRange), true, false));
        }
        else if (!connections[connectionIndex].enabled && Random.Range(0f, 1f) < connectionReanableRate)
        {
            connections[connectionIndex].enabled = true;
        }

    }

    public void Mutate()
    {
        //Mutate Connections
        for (int i = 0; i < connections.Count; i++)
        {
            if (Random.Range(0f, 1f) < connectionMutationRate)
            {
                if (Random.Range(0f, 1f) < connectionRandomConnectionMutationRate)
                {
                    connections[i].weight = Random.Range(-weightRange, weightRange);
                }
                else
                {
                    connections[i].weight *= 1 + Random.Range(-connectionWeightMutationRange, connectionWeightMutationRange);
                }
            }
        }

        //Add connection
        if (Random.Range(0f, 1f) < addConnectionMutationRate)
        {
            AddConnection();
        }

        //Add node
        if (Random.Range(0f, 1f) < addNodeMutationRate)
        {
            AddNode();
        }
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
        for (int layer = 1; layer < n_layers; layer++)
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
