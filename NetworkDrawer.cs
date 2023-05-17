using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkDrawer : MonoBehaviour
{
    public NEAT_Brain brain;

    [Header("General Settings")]
    [SerializeField] Vector2 padding = Vector2.zero;

    [Header("Nodes Settings")]
    [SerializeField] float nodeRadius = 0.1f;
    [SerializeField] int nodeSteps = 10;
    [SerializeField] Color nodeColor = Color.black;
    [SerializeField] Color biasNodeColor = Color.black;

    [Header("Connections Settings")]
    [SerializeField] float connectionWidth = 0.05f;
    [SerializeField] Color positiveConnectionColor = Color.red;
    [SerializeField] Color negativeConnectionColor = Color.blue;
    [SerializeField] Color disabledConnectionColor = Color.gray;
    [SerializeField] bool drawDisabledConnections = true;

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
        //Compute layers x positions
        float[] layersX = new float[brain.n_layers];
        for (int i = 0; i < brain.n_layers; i++)
        {
            layersX[i] = (float)(i) / (brain.n_layers - 1) - 0.5f;
            layersX[i] *= width / height;
        }

        nodePositions.Clear();
        int[] nodePerLayer = new int[brain.n_layers];
        for (int i = 0; i < brain.n_layers; i++)
            nodePerLayer[i] = 0;

        foreach (Node node in brain.nodes)
        {
            nodePerLayer[node.layer]++;
        }

        int[] nodePerLayerFound = new int[brain.n_layers];
        for (int i = 0; i < brain.n_layers; i++)
            nodePerLayerFound[i] = 0;

        for (int i = 0; i < brain.nodes.Count; i++)
        {
            Node node = brain.nodes[i];
            float x = layersX[node.layer];
            float y = nodePerLayer[node.layer] > 1 ? (float)(nodePerLayerFound[node.layer]) / (nodePerLayer[node.layer] - 1) - 0.5f : 0;
            Vector2 pos = new Vector2(x, y);
            nodePositions.Add(pos);
            nodePerLayerFound[node.layer]++;
        }
    }

    void DrawNodes()
    {
        for (int i = 0; i < brain.nodes.Count; i++)
        {
            Circle(nodePositions[i], nodeRadius, nodeSteps, brain.nodes[i].type == 3 ? biasNodeColor : nodeColor);
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
