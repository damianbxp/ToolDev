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
    public bool closedPath = false;

    public string description;

    public GameObject FencePost;

    const int layer = 8;

    /// <summary>
    /// Creates new Waypoint at given position
    /// </summary>
    /// <param name="pos">Position to create waypoint</param>
    public void CreateWaypoint(Vector3 pos) {
        GameObject newWaypoint = new GameObject(name + " " + transform.childCount.ToString());
        newWaypoint.transform.parent = gameObject.transform;
        newWaypoint.transform.position = pos;
        newWaypoint.AddComponent<FenceWaypoint>();
        newWaypoint.layer = layer;

        PrefabUtility.InstantiatePrefab(FencePost, newWaypoint.transform);
    }
    /// <summary>
    /// Draws line beetween all waypoints
    /// </summary>
    public void DrawGizmosLines() {
        if(transform.childCount > 0) {
            Vector3[] points = new Vector3[closedPath ? transform.childCount + 1 : transform.childCount];
            for(int i = 0; i < transform.childCount; i++) {
                points[i] = transform.GetChild(i).position;
            }
            if(closedPath) points[transform.childCount] = transform.GetChild(0).position;
            Handles.color = Color.green;
            Handles.DrawAAPolyLine(2f, points);
            Handles.color = Color.white;
        }
    }
    /// <summary>
    /// Generates names of fence waypoints based on fence name by adding index
    /// </summary>
    public void FixNames() {
        for(int i = 0; i < transform.childCount; i++) {
            transform.GetChild(i).name = name + " " + i.ToString();
        }
    }
    /// <summary>
    /// Rebuilds Fence Meshes
    /// </summary>
    /// <param name="newPrefab">New prefab</param>
    public void Rebuild(GameObject newPrefab) {
        UpdatePrefabs(newPrefab);
        UpdateLayer(layer);
    }
    /// <summary>
    /// Deletes old prefab and spawns new if needed
    /// </summary>
    /// <param name="prefab">New prefab</param>
    public void UpdatePrefabs(GameObject prefab) {
        FencePost = prefab;
        UpdatePrefabs();
    }
    public void UpdatePrefabs() {
        
        for(int i = 0; i < transform.childCount; i++) {
            if(transform.GetChild(i).childCount > 0) {
                if(!GameObject.ReferenceEquals(FencePost, transform.GetChild(i).GetChild(0).gameObject)) {
                    DestroyImmediate(transform.GetChild(i).GetChild(0).gameObject);
                    PrefabUtility.InstantiatePrefab(FencePost, transform.GetChild(i).transform);
                }
            } else {
                PrefabUtility.InstantiatePrefab(FencePost, transform.GetChild(i).transform);
            }
        }
    }
    public void DrawHandles() {
        if(transform.childCount > 0) {
            for(int i = 0; i < transform.childCount; i++) {
                transform.GetChild(i).GetComponent<FenceWaypoint>().DrawPosHandle();
            }

        }
    }
    public void UpdateLayer(int newLayer) {
        gameObject.layer = newLayer;
        if(transform.childCount > 0) {
            for(int i = 0; i < transform.childCount; i++) {
                transform.GetChild(i).GetComponent<FenceWaypoint>().UpdateLayer(newLayer);
            }

        }
    }
    public void UpdateSettings(float groundScanRange, float groundScanHeightOffset) {
        if(transform.childCount > 0) {
            for(int i = 0; i < transform.childCount; i++) {
                transform.GetChild(i).GetComponent<FenceWaypoint>().GroundScanRange = groundScanRange;
                transform.GetChild(i).GetComponent<FenceWaypoint>().GroundScanHeightOffset = groundScanHeightOffset;
            }

        }
    }
}
