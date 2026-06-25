using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class Node : MonoBehaviour
{

    public List<NodeConnection> nodeConnections = new List<NodeConnection>();

    private RoadNetworkControl networkControl;
    private RoadRendering roadRendering;

    private Material lineMaterial;
    private float lineWidth = 0.1f;
    private Transform roadVisualParent;
    //private LineRenderer lineRenderer;
    private Dictionary<NodeConnection, LineRenderer> connectionRenderers = new();

    public string debug = "Dbg";
    private void Awake()
    {
        networkControl = GameObject.FindAnyObjectByType<RoadNetworkControl>();
        networkControl.RegisterNode(this);

        roadRendering = FindAnyObjectByType<RoadRendering>();
    }

    public void AddConnection(NodeConnection connection)
    {
        nodeConnections.Add(connection);
        //networkControl.AddConnection(this, connection);

    }
    public void RemoveConnection(NodeConnection connection)
    {
        if (connectionRenderers.ContainsKey(connection))
            roadRendering.ReturnLineRendererGO(connectionRenderers[connection].gameObject);

        nodeConnections.Remove(connection);
        connectionRenderers.Remove(connection);
    }
    public void InitialiseConnection(NodeConnection connection, Material lm, float lw, Transform rvp)
    {

        //VISUALS
        lineMaterial = lm;
        lineWidth = lw;
        roadVisualParent = rvp;

        GameObject lineObj = roadRendering.GetLineRendererGO();
        LineRenderer lineRenderer = lineObj.GetComponent<LineRenderer>();
        connectionRenderers.Add(connection, lineRenderer);

        lineRenderer.material = lineMaterial;

        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;
        lineRenderer.positionCount = 2;

        AddConnection(connection);

        //DraawConnection(connection);
        //DrawConnections();
    }

    public void DrawConnections()
    {
        foreach (NodeConnection nc in connectionRenderers.Keys)
        {

            connectionRenderers[nc].SetPosition(0, this.transform.position);
            connectionRenderers[nc].SetPosition(1, nc.node02.transform.position);
        }

    }

}
