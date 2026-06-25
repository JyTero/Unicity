using System.Collections.Generic;
using UnityEngine;

public class AStar : MonoBehaviour
{
    private Dictionary<Traveller, List<NodeConnectionPathfindScoring>> openLists = new ();

    public void BeginPathfindForTraveller(Traveller traveller, Node destination)
    {
        traveller.OnTravellerPathfindBegin(destination);


    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private class NodeConnectionPathfindScoring
    {
        public NodeConnection connection;

        public int distance;
        public int travelCostToHere;

        public NodeConnectionPathfindScoring(NodeConnection nc, Node destination)
        {
            connection = nc;
            SetNodeDistanceToDestination(destination);
        }

        public void SetNodeDistanceToDestination(Node destination)
        {
            distance = (int)Vector3.Distance(connection.node02.transform.position, destination.transform.position);
        }
    }
}
