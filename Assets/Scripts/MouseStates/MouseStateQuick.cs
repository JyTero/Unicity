using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class MouseStateQuick : MonoBehaviour
{

    public enum MouseState { Default, NodePlacement, NodeConnection }

    public MouseState currentState = MouseState.Default;

    [SerializeField]
    private GameObject nodePrefab;

    private Node connection01;
    private Node connection02;

    [SerializeField]
    private LayerMask groundLayer;
    [SerializeField]
    private LayerMask characterLayer;
    [SerializeField]
    private LayerMask nodeLayer;

    private RoadNetworkControl rnc;

    [System.Obsolete]
    private void Start()
    {
        rnc = FindAnyObjectByType<RoadNetworkControl>();
    }
    private void Update()
    {
        RaycastHit hit;
        switch (currentState)
        {
            case MouseState.Default:
                if (Input.GetMouseButtonDown(0))
                {
                    if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
                    {
                        Debug.Log(hit.collider.gameObject.name);
                        if (EventSystem.current.IsPointerOverGameObject())
                        {
                            Debug.Log("On UI");
                            return;
                        }
                    }
                }
                return;
            case MouseState.NodePlacement:
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity, groundLayer))
                        {
                            //if (hit.collider.gameObject.layer == groundLayer.value)
                            GameObject nodeGO = rnc.InstantiateNode(hit.point); //Instantiate(nodePrefab, hit.point, Quaternion.identity);
                            rnc.NodeHandpickPlacement(nodeGO.GetComponent<Node>());
                            //else
                            //Debug.Log($"Did not hit ground layer object {hit.collider.gameObject.layer} vs {groundLayer.value}");
                        }
                    }
                }
                return;
            case MouseState.NodeConnection:
                return;
        }
    }



    public void OnMouseClick()
    {

    }

    [SerializeField]
    private Button placeNodesButton;
    public void PlaceNodesButton()
    {
        if (currentState != MouseState.NodePlacement)
        {
            currentState = MouseState.NodePlacement;
            placeNodesButton.image.color = Color.blue;
        }
        else
        {
            currentState = MouseState.Default;
            placeNodesButton.image.color = Color.white;
        }

    }
}

