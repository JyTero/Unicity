using System.Collections.Generic;
using UnityEngine;

public class RoadRendering : MonoBehaviour
{
    private List<GameObject> activeLineRendererGOs = new();
    private List<GameObject> pooledLinerendererGos = new();

    [SerializeField]
    private Transform lineRendererHierarchyParent;

    private RoadNetworkControl RNC;
    private Dictionary<Node, List<NodeConnection>> RoadNetwork;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        RNC = FindAnyObjectByType<RoadNetworkControl>();
    }
    void FixedUpdate()
    {
        DrawConnectionLineRenderers();
    }

    private void DrawConnectionLineRenderers()
    {
        RoadNetwork = RNC.RoadNetwork;

        if (RoadNetwork.Count < 2)
            return;

        foreach (Node node in RoadNetwork.Keys)
        {
            node.DrawConnections();
        }

    }

    public GameObject GetLineRendererGO()
    {
        GameObject LRGO;
        if (pooledLinerendererGos.Count == 0)
        {
            LRGO = new GameObject("Connection");
            LRGO.transform.SetParent(lineRendererHierarchyParent);
            LRGO.AddComponent<LineRenderer>();

            activeLineRendererGOs.Add(LRGO);
            return LRGO;
        }
        else
        {
            LRGO = pooledLinerendererGos[0];
            pooledLinerendererGos.Remove(LRGO);

            activeLineRendererGOs.Add(LRGO);
            return LRGO;
        }
    }

    public void ReturnLineRendererGO(GameObject LRGO)
    {
        LineRenderer lineRenderer = LRGO.GetComponent<LineRenderer>();

        lineRenderer.SetPosition(0, new Vector3(-5, -5, -5));
        lineRenderer.SetPosition(1, new Vector3(-6, -5, -5));

        activeLineRendererGOs.Remove(LRGO);
        pooledLinerendererGos.Add(LRGO);
    }
}