using System;
using UnityEngine;

public class Road : MonoBehaviour
{
    [SerializeField] private Node node01;
    [SerializeField] private Node node02 ;
    [SerializeField] private NodeConnection nodeConnection01;
    [SerializeField] private NodeConnection nodeConnection02;

    [SerializeField] private GameObject startNodeGO;
    [SerializeField] private GameObject endNodeGO;
    private RoadNetworkControl networkControl;

    private void Start()
    {
        networkControl = GameObject.FindAnyObjectByType<RoadNetworkControl>();


        node01 = startNodeGO.GetComponent<Node>();
        networkControl.RegisterNode(node01);

        node02 = endNodeGO.GetComponent<Node>();
        networkControl.RegisterNode(node02);

        nodeConnection01 = networkControl.CreateNodeConnection(node01, node02);
        nodeConnection02 = networkControl.CreateNodeConnection(node02, node01);

    }
}

