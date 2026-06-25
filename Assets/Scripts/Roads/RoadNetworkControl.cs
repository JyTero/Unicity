using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class RoadNetworkControl : MonoBehaviour
{

    [SerializeField]
    private bool IsDebug;
    [SerializeField]
    private int NodeSearchRadius = 4;

    [SerializeField]
    private int roadNodeLayerIndex = 6;

    [SerializeField]
    private GameObject nodePrefab;

    //VISUALS
    [SerializeField]
    private Material lineMaterial;
    [SerializeField]
    private float lineWidth = 0.1f;
    [SerializeField]
    private Transform roadVisualParent;
    private List<Node> allNodes = new List<Node>();

    private List<Node> activeNodes = new List<Node>();  //Recently adjusted nodes, checking for connections
    private Dictionary<Node, List<NodeConnection>> roadNetwork = new Dictionary<Node, List<NodeConnection>>();
    public Dictionary<Node, List<NodeConnection>> RoadNetwork
    {
        get { return roadNetwork; }
    }

    private Dictionary<Node, List<NodeConnection>> roadNetworkQueue = new Dictionary<Node, List<NodeConnection>>();

    private Dictionary<Node, List<Node>> RNConnectionRemoveQueue = new Dictionary<Node, List<Node>>();
    private bool newConnection = true;
    private Node firstNodeInBuild;



    [SerializeField]
    private LayerMask roadNodeLayer;


    //DEBUG
    int nodeN = 0;
    int connectionN = 0;
    int intersectionN = 0;
    private void Start()
    {
        //roadNodeLayer = LayerMask.GetMask("6"); //LayerMask.GetMask(roadNodeLayerIndex.ToString());


    }

    void FixedUpdate()
    {
        //FindNearbyNodes();
        CheckConnectionRemovalQueue();
    }
    private void CheckConnectionRemovalQueue()
    {
        //if (roadNetwork.Count < 2)
        //    return;

        //foreach (Node node in roadNetwork.Keys)
        //{
        //    node.DrawConnections();
        //}

        //And hande node removal
        if (RNConnectionRemoveQueue.Count > 0)
        {
            foreach (Node node in RNConnectionRemoveQueue.Keys)
            {
                foreach (Node targetNode in RNConnectionRemoveQueue[node])
                {
                    NodeConnection nc = GetSpesificConnection(node, targetNode);
                    if (nc != null)
                        BreakNodeConnection(nc);
                }
            }
        }
    }


    public GameObject InstantiateNode(Vector3 pos)
    {
        GameObject nodeGO = Instantiate(nodePrefab, pos, Quaternion.identity);
        return nodeGO;

    }
    public void RegisterNode(Node node)
    {
        allNodes.Add(node);

        activeNodes.Add(node);
        //RoadNetwork.Add(node, new List<NodeConnection>());
        RoadNetworkAddNewNode(node);

    }

    private void RoadNetworkAddNewNode(Node node)
    {
        roadNetwork.Add(node, new List<NodeConnection>());


        //RoadNetworkQueue
        if (roadNetworkQueue.ContainsKey(node))
        {
            //foreach(NodeConnection connection in roadNetworkQueue[node])
            for (int i = 0; i < roadNetworkQueue[node].Count; i++)
            {
                roadNetwork[node].Add(roadNetworkQueue[node][i]);
            }
            roadNetworkQueue.Remove(node);
        }

        //ConnectionClearingQueue
        //if (RNConnectionRemoveQueue.ContainsKey(node))
        //{
        //    foreach (Node connectNode in RNConnectionRemoveQueue[node])
        //    {
        //        NodeConnection nc = GetSpesificConnection(node, connectNode);
        //        if (nc != null)
        //            BreakNodeConnection(nc);
        //        //RoadNetwork[connection.node01].Remove(connection);
        //        //connection.node01.RemoveConnection(connection);
        //    }
        //    RNConnectionRemoveQueue.Remove(node);
        //}
    }


    public void NodeHandpickPlacement(Node node)
    {
        node.gameObject.name = $"Node{nodeN}";
        nodeN++;
        //connection building
        if (newConnection)
        {
            newConnection = false;

            firstNodeInBuild = node;

        }
        else
        {
            newConnection = true;
            NodeConnection nc1 = CreateNodeConnection(firstNodeInBuild, node);
            nc1.InitialiseConnection();
            MakeConnectionBidirectional(nc1);
            HandleIntersectingConnections(nc1);

            //NodeConnection nc2 = CreateNodeConnection(node, firstNodeInBuild);
            //nc2.InitialiseConnection();
            //HandleIntersectingConnections(nc2);


            //nc.DrawLine(firstNodeInBuild.transform.position, node.transform.position);
        }
    }

    public List<NodeConnection> GetNodeConnections(Node node)
    {
        return roadNetwork[node];
    }


    private void HandleIntersectingConnections(NodeConnection newConnection)
    {
        //NEED A BETTER WAY TO FIND NEARBY NODES
        //Get all nearby nodes, based on given codes the new connection
        List<Node> nearbyNodes = new List<Node>();
        /*  Collider[] colliders01 = Physics.OverlapSphere(newConnection.node01.transform.position, newConnection.connectionLenght, roadNodeLayer);
          Collider[] colliders02 = Physics.OverlapSphere(newConnection.node02.transform.position, newConnection.connectionLenght, roadNodeLayer);

          foreach (Collider collider in colliders01)
          {
              var node = collider.gameObject.GetComponent<Node>();
              if (node != null)
                  if (!nearbyNodes.Contains(node))
                      nearbyNodes.Add(node);

          }

          foreach (Collider collider in colliders02)
          {
              var node = collider.gameObject.GetComponent<Node>();
              if (node != null)
                  if (!nearbyNodes.Contains(node))
                      nearbyNodes.Add(node);

          }*/

        //Gummy
        foreach (Node node in roadNetwork.Keys)
        {
            nearbyNodes.Add(node);
        }

        if (nearbyNodes.Count == 0)
            return;

        if (IsDebug)
            Debug.Log($"Found {nearbyNodes.Count} nearby nodes");

        //List<Vector3> intersectionPoints = new();
        Dictionary<Vector3, List<NodeConnection>> intersectionPoints = new();
        NodeConnection nearbyConnection = null;

        for (int i = 0; i < nearbyNodes.Count; i++)
        {
            Node node = nearbyNodes[i];

            List<NodeConnection> nearbyConnections = roadNetwork[node];
            for (int j = 0; j < nearbyConnections.Count; j++)
            {
                nearbyConnection = nearbyConnections[j];
                if (nearbyConnection == newConnection)
                    continue;

                //If Intersects return 0, parallel, will never cross
                float[] intersectionResults = ConnectionIntersection(newConnection, nearbyConnection);
                if (intersectionResults[0] == 0)
                    continue;
                Vector3 intersectPos = new Vector3(intersectionResults[0], newConnection.node01.transform.position.y, intersectionResults[1]);
                if (!intersectionPoints.ContainsKey(intersectPos))
                    intersectionPoints.Add(intersectPos, new List<NodeConnection> { newConnection, nearbyConnection });
            }
        }
        foreach (Vector3 intersectionPos in intersectionPoints.Keys)
        {
            NodeConnection nc01 = intersectionPoints[intersectionPos][0];
            NodeConnection nc02 = intersectionPoints[intersectionPos][1];
            //Instantiate(, intersectionPos, Quaternion.identity);
            //GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            //cube.transform.position = intersectionPos;
            //cube.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);

            //Create proper intersection node
            GameObject nodeGO = InstantiateNode(intersectionPos);
            nodeGO.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);

            Node newNode = nodeGO.GetComponent<Node>();


            //Create new Connections
            NodeConnection subConnection = CreateNodeConnection(newNode, nc01.node01);
            subConnection.InitialiseConnection();
            MakeConnectionBidirectional(subConnection);

            subConnection = CreateNodeConnection(newNode, nc02.node01);
            subConnection.InitialiseConnection();
            MakeConnectionBidirectional(subConnection);

            subConnection = CreateNodeConnection(newNode, nc01.node02);
            subConnection.InitialiseConnection();
            MakeConnectionBidirectional(subConnection);

            subConnection = CreateNodeConnection(newNode, nc02.node02);
            subConnection.InitialiseConnection();
            MakeConnectionBidirectional(subConnection);

            //break old connections

            //NodeConnection connectionToBreak = GetSpesificConnection(nc01.node01, nc01.node02);
            //if (connectionToBreak != null)
            //    BreakNodeConnection(connectionToBreak);
            //else
            //    AddToRNConnectionRemoveQueue(nc01.node01, nc01.node02);
            ////Both Directions
            //connectionToBreak = GetSpesificConnection(nc01.node02, nc01.node01);
            //if (connectionToBreak != null)
            //    BreakNodeConnection(connectionToBreak);
            //else
            //    AddToRNConnectionRemoveQueue(nc01.node01, nc01.node02);
                AddToRNConnectionRemoveQueue(nc01.node01, nc01.node02);
                AddToRNConnectionRemoveQueue(nc01.node02, nc01.node01);

            //connectionToBreak = GetSpesificConnection(nc02.node01, nc02.node02);
            //if (connectionToBreak != null)
            //    BreakNodeConnection(connectionToBreak);
            //else
            //    AddToRNConnectionRemoveQueue(nc02.node01, nc02.node02);
            ////Both Directions
            //connectionToBreak = GetSpesificConnection(nc02.node01, nc02.node02);
            //if (connectionToBreak != null)
            //    BreakNodeConnection(connectionToBreak);
            //else
            //    AddToRNConnectionRemoveQueue(nc02.node02, nc02.node01);
            AddToRNConnectionRemoveQueue(nc02.node01, nc02.node02);
                AddToRNConnectionRemoveQueue(nc02.node02, nc02.node01);


        }



    }


    private void AddToRNConnectionRemoveQueue(Node n01, Node n02)
    {
        if (RNConnectionRemoveQueue.ContainsKey(n01))
            RNConnectionRemoveQueue[n01].Add(n02);
        else
            RNConnectionRemoveQueue.Add(n01, new List<Node> { n02 });
    }

    float[] ConnectionIntersection(NodeConnection nc1, NodeConnection nc2)
    {
        Vector2 p1 = new Vector2(nc1.node01.transform.position.x, nc1.node01.transform.position.z);
        Vector2 p2 = new Vector2(nc1.node02.transform.position.x, nc1.node02.transform.position.z);
        Vector2 p3 = new Vector2(nc2.node01.transform.position.x, nc2.node01.transform.position.z);
        Vector2 p4 = new Vector2(nc2.node02.transform.position.x, nc2.node02.transform.position.z);

        float[] returnees = { 0f, 0f, };
        //float d1 = (p4.x - p3.y) * (p2.x - p1.x) - (p4.x - p3.x) * (p2.y - p1.y);
        float d1 = (p4.y - p3.y) * (p2.x - p1.x) - (p4.x - p3.x) * (p2.y - p1.y);
        if (d1 == 0)
            return returnees; // Parallel

        float ua = ((p4.x - p3.x) * (p1.y - p3.y) - (p4.y - p3.y) * (p1.x - p3.x)) / d1; //Point of intersection on NC1 (
        float ub = ((p2.x - p1.x) * (p1.y - p3.y) - (p2.y - p1.y) * (p1.x - p3.x)) / d1;
        //return ua >= 0 && ua <= 1 && ub >= 0 && ub <= 1;
        //returnees[0] = ua;
        //returnees[1] = ub;

        if ((0 <= ua && ua <= 1) && (0 <= ub && ub <= 1))
        {
            Vector2 intersectPos = p1 + ua * (p2 - p1);
            returnees[0] = intersectPos.x;
            returnees[1] = intersectPos.y;
        }

        return returnees;
    }
    //bool Intersects(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4)
    //{
    //    float d1 = (p4.y - p3.y) * (p2.x - p1.x) - (p4.x - p3.x) * (p2.y - p1.y);
    //    if (d1 == 0) return false; // Parallel
    //    float ua = ((p4.x - p3.x) * (p1.y - p3.y) - (p4.y - p3.y) * (p1.x - p3.x)) / d1;
    //    float ub = ((p2.x - p1.x) * (p1.y - p3.y) - (p2.y - p1.y) * (p1.x - p3.x)) / d1;
    //    return ua >= 0 && ua <= 1 && ub >= 0 && ub <= 1;
    //}
    public NodeConnection CreateNodeConnection(Node sn, Node en)
    {
        NodeConnection nc;
        if (!IsExistingConnection(sn, en))
        {

            nc = new NodeConnection(sn, en);
            sn.InitialiseConnection(nc, lineMaterial, lineWidth, roadVisualParent);

            if (roadNetwork.ContainsKey(sn))
                roadNetwork[sn].Add(nc);
            else
            {
                if (roadNetworkQueue.ContainsKey(sn))
                    roadNetworkQueue[sn].Add(nc);
                else
                {
                    roadNetworkQueue.Add(sn, new List<NodeConnection>());
                    roadNetworkQueue[sn].Add(nc);

                }

            }

            return nc;
        }
        else
        {
            return GetSpesificConnection(sn, en);
        }


    }
    private void MakeConnectionBidirectional(NodeConnection connection)
    {
        NodeConnection nc1 = CreateNodeConnection(connection.node02, connection.node01);
        nc1.InitialiseConnection();
        // HandleIntersectingConnections(nc1);

    }

    private bool IsExistingConnection(Node n01, Node n02)
    {
        bool b;
        if (roadNetwork.ContainsKey(n01))
        {
            b = roadNetwork[n01].Contains(new NodeConnection(n01, n02));

            return b;
        }
        return false;
    }
    private NodeConnection GetSpesificConnection(Node n01, Node n02)
    {
        if (roadNetwork.ContainsKey(n01))
        {
            foreach (NodeConnection connection in roadNetwork[n01])
            {
                if (connection.node02 == n02)
                    return connection;
            }
        }
        return null;
    }

    private void BreakNodeConnection(NodeConnection connection)
    {
        if (roadNetwork.ContainsKey(connection.node01) && roadNetwork.ContainsKey(connection.node02))
        {
            roadNetwork[connection.node01].Remove(connection);
            connection.node01.RemoveConnection(connection);
            roadNetwork[connection.node02].Remove(connection);
            connection.node02.RemoveConnection(connection);
            if (IsDebug)
                Debug.Log($"Destroyed connection between {connection.node01} ({connection.node01.gameObject.name}) and {connection.node02} ({connection.node02.gameObject.name})");
        }
        else
        {
            // Debug.LogWarning("Wanted to queue node connection breaking due to either node not found in RoadNetwork");

            //if (RNConnectionRemoveQueue.ContainsKey(connection.node01))
            //    RNConnectionRemoveQueue[connection.node01].Add(connection.node02);
            //else
            AddToRNConnectionRemoveQueue(connection.node01, connection.node02);

            //if (RNConnectionRemoveQueue.ContainsKey(connection.node02))
            //    RNConnectionRemoveQueue[connection.node02].Add(connection.node01);
            //else
            ///AddToRNConnectionRemoveQueue(connection.node02, connection.node01);

        }

    }

    //DEBUG
    public string PrintRoadNetwork()
    {
        string output = "";
        int nodeN = 0;
        int connectionN = 0;
        foreach (Node node in roadNetwork.Keys)
        {
            output += $"Node:{nodeN}\n";
            foreach (NodeConnection connection in roadNetwork[node])
            {
                output += $"Connection{connectionN}: {connection.node01} to {connection.node02}\n";
                connectionN++;
            }
            connectionN = 0;
            nodeN++;
            output += "\n";
        }
        return output;
    }

    public List<Node> GetAllNodes()
    {
        return new List<Node>(roadNetwork.Keys);
    }

    public void UnregisterNode(Node node)
    {
        allNodes.Remove(node);
        activeNodes.Remove(node);
    }
}
