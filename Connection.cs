using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Connection
{
    public int innovationID;
    public int fromNode;
    public int toNode;
    public float weight;
    public bool enabled;
    public bool isRecurrent;

    public Connection(int fromNode, int toNode, float weight, bool enabled, bool isRecurrent)
    {
        this.innovationID = CantorMap(fromNode, toNode);
        this.fromNode = fromNode;
        this.toNode = toNode;
        this.weight = weight;
        this.enabled = enabled;
        this.isRecurrent = isRecurrent;
    }

    int CantorMap(int x, int y)
    {
        return (x + y) * (x + y + 1) / 2 + y;
    }
}
