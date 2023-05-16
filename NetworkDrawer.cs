using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkDrawer : MonoBehaviour
{
    public NEAT_Brain brain;

    [Header("General Settings")]
    [SerializeField] Vector2 padding = Vector2.zero;
    [SerializeField] bool drawDisabledConnections = true;

    [Header("Nodes Settings")]
    [SerializeField] float nodeRadius = 0.1f;
    [SerializeField] int nodeSteps = 10;
    [SerializeField] Color nodeColor = Color.black;

    [Header("Connections Settings")]
    [SerializeField] float connectionWidth = 0.05f;
    [SerializeField] Color positiveConnectionColor = Color.red;
    [SerializeField] Color negativeConnectionColor = Color.blue;
    [SerializeField] Color disabledConnectionColor = Color.gray;


    float width, height;
    List<Vector2> nodePositions = new List<Vector2>();

    // Start is called before the first frame update
    void Start()
    {
        brain = new NEAT_Brain(2, 3, 1, 1);

        //complement positive connection color
        negativeConnectionColor = new Color(1 - positiveConnectionColor.r, 1 - positiveConnectionColor.g, 1 - positiveConnectionColor.b);
        disabledConnectionColor = positiveConnectionColor * 0.5f + negativeConnectionColor * 0.5f;
    }

    // Update is called once per frame
    void Update()
    {
        width = transform.localScale.x - padding.x * 2;
        height = transform.localScale.y - padding.y * 2;
    }

    public void DrawNetwork()
    {
        ComputeNodesPositions();

        DrawConnections();

        DrawNodes();

    }

    void Line(float thickness, Vector2 start, Vector2 end, Color color)
    {
        GL.Begin(GL.QUADS);
        GL.Color(color);
        Vector2 perpendicular = (end - start).normalized * thickness / 2;
        perpendicular = new Vector2(-perpendicular.y, perpendicular.x);
        GL.Vertex(loc2Glob(start + perpendicular));
        GL.Vertex(loc2Glob(start - perpendicular));
        GL.Vertex(loc2Glob(end - perpendicular));
        GL.Vertex(loc2Glob(end + perpendicular));
        GL.End();
    }

    void Circle(Vector2 center, float radius, int steps, Color color)
    {
        GL.Begin(GL.TRIANGLE_STRIP);
        GL.Color(color);
        for (int i = 0; i <= steps; i++)
        {
            float angle = i * Mathf.PI * 2 / steps;
            GL.Vertex(loc2Glob(center + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius));
            GL.Vertex(loc2Glob(center));
        }
        GL.End();
    }

    Vector2 loc2Glob(Vector2 local)
    {
        return new Vector2(transform.position.x, transform.position.y) + local * height;
    }

    void ComputeNodesPositions()
    {
        nodePositions.Clear();
        for (int i = 0; i < brain.nodes.Count; i++)
        {
            Node node = brain.nodes[i];
            float x = (node.layer - 1) * width / height / 2;
            float y = 0;
            if (i < brain.inputSize)
            {
                if (brain.inputSize != 1)
                    y = (float)(i) / (brain.inputSize - 1) - 0.5f;
                else
                    y = 0;
            }
            else if (i < brain.inputSize + brain.hiddenSize)
            {
                if (brain.hiddenSize != 1)
                    y = (float)(i - brain.inputSize) / (brain.hiddenSize - 1) - 0.5f;
                else
                    y = 0;
            }
            else
            {
                if (brain.outputSize != 1)
                    y = (float)(i - brain.inputSize - brain.hiddenSize) / (brain.outputSize - 1) - 0.5f;
                else
                    y = 0;
            }
            Vector2 pos = new Vector2(x, y);
            nodePositions.Add(pos);
        }
    }

    void DrawNodes()
    {
        for (int i = 0; i < brain.nodes.Count; i++)
        {
            Circle(nodePositions[i], nodeRadius, nodeSteps, nodeColor);
        }
    }

    void DrawConnections()
    {
        for (int i = 0; i < brain.connections.Count; i++)
        {
            Connection connection = brain.connections[i];

            if (!drawDisabledConnections && !connection.enabled)
                continue;

            Color color = connection.enabled ? connection.weight > 0 ? positiveConnectionColor : negativeConnectionColor : disabledConnectionColor;

            color = Color.Lerp(color, disabledConnectionColor, Mathf.Atan(Mathf.Abs(connection.weight) / brain.weightRange) / Mathf.PI * 2);

            Line(connectionWidth, nodePositions[connection.fromNode], nodePositions[connection.toNode], color);
        }
    }

    private void OnDrawGizmos()
    {
        if (Application.isPlaying)
            DrawNetwork();
    }
}
