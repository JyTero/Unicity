using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class NodeConnection
{


    private LineRenderer lineRenderer;

    public Node node01;
    public Node node02;

    public float connectionLenght;

    public int NodeTravelCost = 1;


    public NodeConnection(Node sn, Node en)
    {
        node01 = sn;
        node02 = en;
        connectionLenght = Vector3.Distance(node01.transform.position, node02.transform.position);
        NodeTravelCost += (int)connectionLenght / 2;
    }

    public void InitialiseConnection()
    {
    }

    public void DrawConnection()
    {
        if ((node01 == null) || (node02 == null))
            return;
        lineRenderer.SetPosition(0, node01.transform.position);
        lineRenderer.SetPosition(1, node02.transform.position);
    }
}

