using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
/// <summary>
/// Holds data about fence
/// </summary>
public class Fence : MonoBehaviour
{
    [HideInInspector]
    public bool expandDisplay = false;
    /// <summary>
    /// Creates new Waypoint at given position
    /// </summary>
    /// <param name="pos">Position to create waypoint</param>
    public void CreateWaypoint(Vector3 pos) {
        GameObject newWaypoint = new GameObject(name + " " + transform.childCount.ToString());
        newWaypoint.transform.parent = gameObject.transform;
        newWaypoint.transform.position = pos;
        newWaypoint.AddComponent<FenceWaypoint>();
    }
    /// <summary>
    /// Draws line beetween all waypoints
    /// </summary>
    public void DrawGizmosLines() {
        if(transform.childCount > 0) {
            Vector3[] points = new Vector3[transform.childCount];
            for(int i = 0; i < transform.childCount; i++) {
                points[i] = transform.GetChild(i).position;
            }
            //points[transform.childCount] = transform.GetChild(0).position;
            Handles.color = Color.green;
            Handles.DrawAAPolyLine(2f, points);
            Handles.color = Color.white;
        }
    }
}
