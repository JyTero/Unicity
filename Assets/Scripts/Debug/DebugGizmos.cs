using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugGizmos : MonoBehaviour
{
    private List<Vector3> positions = new();

    private Vector3 mouseStart;
    private Vector3 mouseEnd;

    public void AddPositionPair(Vector3 p01, Vector3 p02)
    {
        positions.Add(p01);
        positions.Add(p02);
    }

    void OnDrawGizmos()
    {
        for (int i = 0; i < positions.Count; i++) {
            Vector3 p01 = positions[i];
            i++;
            Vector3 p02 = positions[i];

            Gizmos.color = Color.green; // Set line color
            Gizmos.DrawLine(p01, p02);
        }

        positions.Clear();

        Gizmos.color= Color.red;
        Gizmos.DrawLine (mouseStart, mouseEnd);
    }

    public void OnMouseClick(Vector3 start, Vector3 end)
    {
        this.mouseStart = start;   
        this.mouseEnd = end;
    }
}
