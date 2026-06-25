using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Traveller : MonoBehaviour
{

    [SerializeField] public Node SpawnNode;

    private Node currentNode;
    private Node destinationNode;

    private RoadNetworkControl RNC;

    // Start is called before the first frame update
    void Start()
    {
        this.gameObject.transform.position = transform.position;
        RNC = GameObject.FindAnyObjectByType<RoadNetworkControl>();
        currentNode = SpawnNode;
    }

    public void OnTravellerPathfindBegin(Node destination)
    {
        destinationNode = destination;
    }

}
