using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[SelectionBaseFixed]
public class FenceWaypoint : MonoBehaviour
{
    private float groundScanRange = 5f;
    private float groundScanHeightOffset = 2f;

    public float GroundScanHeightOffset { get => groundScanHeightOffset; set => groundScanHeightOffset =  value ; }
    public float GroundScanRange { get => groundScanRange; set => groundScanRange =  value ; }
    /// <summary>
    /// Draws Position Handle. Allows to move Fence Waypoint and snap it to ground
    /// </summary>
    public void DrawPosHandle() {
        
        Vector3 newPos = Handles.PositionHandle(transform.position, transform.rotation);

        if(Physics.Raycast(transform.position + transform.up * GroundScanHeightOffset, -transform.up, out RaycastHit hit, GroundScanRange==0 ? 0 : GroundScanRange + GroundScanHeightOffset, ~LayerMask.GetMask("Fence"))) {
            Debug.DrawLine(hit.point, transform.position, Color.green);
            newPos.y = hit.point.y;
        } else {
            Debug.DrawLine(transform.position, transform.position - transform.up*100, Color.red);
        }

        if(newPos != transform.position) {
            Undo.RecordObject(gameObject.transform, "Change Post Position");
            transform.position = newPos;
        }
    }

    public void UpdateLayer(int newLayer) => gameObject.layer = newLayer;
}
