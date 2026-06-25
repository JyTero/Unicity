using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class ClickHandling : MonoBehaviour
{
    private DebugGizmos debugGizmos;
    private MouseStateQuick mouseStateMachine;
    // Start is called before the first frame update
    void Start()
    {
        this.debugGizmos = FindObjectOfType<DebugGizmos>();
        mouseStateMachine = FindObjectOfType<MouseStateQuick>();
    }
    void FixedUpdate()
    {

    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
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
    }
}
