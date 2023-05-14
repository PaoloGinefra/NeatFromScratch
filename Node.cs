using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public int id;
    public int type; // 0 = input, 1 = hidden, 2 = output, 3 = bias
    public int layer;
    public float sumInput = 0;
    public float sumOutput = 0;

    public Node(int id, int type, int layer)
    {
        this.id = id;
        this.type = type;
        this.layer = layer;
    }

    //copy constructor
    public Node(Node node)
    {
        this.id = node.id;
        this.type = node.type;
        this.layer = node.layer;
    }
}
