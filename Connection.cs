using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Connection
{
    static int innovationIDCounter = 0;
    static List<int> innovationIDs = new List<int>();
    public int innovationID;
    public int fromNode;
    public int toNode;
    public float weight;
    public bool enabled;
    public bool isRecurrent;

    public Connection(int fromNode, int toNode, float weight, bool enabled, bool isRecurrent)
    {
        this.innovationID = getInnovationID(fromNode, toNode);
        this.fromNode = fromNode;
        this.toNode = toNode;
        this.weight = weight;
        this.enabled = enabled;
        this.isRecurrent = isRecurrent;
    }

    //copy constructor
    public Connection(Connection connection)
    {
        this.innovationID = connection.innovationID;
        this.fromNode = connection.fromNode;
        this.toNode = connection.toNode;
        this.weight = connection.weight;
        this.enabled = connection.enabled;
        this.isRecurrent = connection.isRecurrent;
    }

    int CantorMap(int x, int y)
    {
        return (x + y) * (x + y + 1) / 2 + y;
    }

    int getInnovationID(int fromNode, int toNode)
    {
        int index = CantorMap(fromNode, toNode);

        if (innovationIDs.Count > index)
        {
            if (innovationIDs[index] == -1)
            {
                innovationIDs[index] = innovationIDCounter;
                innovationIDCounter++;
            }
        }
        else
        {
            while (innovationIDs.Count < index)
            {
                innovationIDs.Add(-1);
            }
            innovationIDs.Add(innovationIDCounter);
            innovationIDCounter++;
        }

        return innovationIDs[index];
    }
}
