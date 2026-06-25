using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

public class DebugLandia : MonoBehaviour
{

    private RoadNetworkControl RNC;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        RNC = FindAnyObjectByType<RoadNetworkControl>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            Debug.Log($"RoadNetwork:");

            string output =  FindAnyObjectByType<RoadNetworkControl>().PrintRoadNetwork();
            Debug.Log(output);
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            List<Node> nodes = new List<Node>(RNC.GetAllNodes());
            string output = "";
            int nodeN = 0;
            int connectionN = 0;
            foreach (Node node in nodes) {
                output += $"Node:{nodeN}\n";

                foreach (NodeConnection connection in node.nodeConnections)
                {
                    output += $"Connection{connectionN}: {connection.node01} to {connection.node02}\n";
                    connectionN++;
                }
                connectionN = 0;
                nodeN++;
                output += "\n";
            }
            Debug.Log(output);
        }
    }
}