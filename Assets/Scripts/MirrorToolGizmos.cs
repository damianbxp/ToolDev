using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MirrorToolGizmos : MonoBehaviour
{
    private void OnGUI() {
        DrawGizmos();
    }
    private void DrawGizmos() {
            /*Vector3[] rectVerts = new Vector3[] {
                transform.position + transform.forward * (gizmoRectWidth/2) + transform.up * (gizmoRectHeight/2),
                transform.position + transform.forward * (gizmoRectWidth/2) - transform.up * (gizmoRectHeight/2),
                transform.position - transform.forward * (gizmoRectWidth/2) - transform.up * (gizmoRectHeight/2),
                transform.position - transform.forward * (gizmoRectWidth/2) + transform.up * (gizmoRectHeight/2)
            };*/
            Vector3[] rectVerts = new Vector3[] {
                transform.position + transform.forward * (2/2) + transform.up * (1/2),
                transform.position + transform.forward * (2/2) - transform.up * (1/2),
                transform.position - transform.forward * (2/2) - transform.up * (1/2),
                transform.position - transform.forward * (2/2) + transform.up * (1/2)
            };

            /*Vector3[] rectVerts = new Vector3[] {
                new Vector3(1,0,1),
                new Vector3(-1,0,1),
                new Vector3(-1,0,-1),
                new Vector3(1,0,-1)
            };*/

            Handles.DrawSolidRectangleWithOutline(rectVerts, new Color(1f, 0f, 0f, 0.5f), new Color(1f, 0f, 0f, 0.9f));
            //Handles.RectangleHandleCap(0, transform.position, transform.rotation, 2, EventType.Repaint);

    }
   
}

