using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MirrorTool : EditorWindow {
    [MenuItem("Tools/Mirror Tool")]
    public static void OpenMirrorTool() => GetWindow<MirrorTool>("Mirror Tool");

    public List<GameObject> sourceObjects;
    public Transform mirrorTf;

    private List<GameObject> mirroredObjects;

    private bool sourceObjectsToogle;
    private Vector2 scrollPos;

    private bool mirrorX = false;
    private bool lastMirrorX = false;
    private bool mirrorY = false;
    private bool lastMirrorY = false;
    private bool mirrorZ = false;
    private bool lastMirrorZ = false;

    private bool advancedSettingsToggle = false;

    private bool advancedSettingsMirrorX = false;
    private bool advancedSettingsMirrorY = false;
    private bool advancedSettingsMirrorZ = false;

    private float gizmoRectHeight = 1f;
    private float gizmoRectWidth = 2f;

    private Color mirrorXColor = new Color(1f, 0f, 0f, 0.3f);
    private Color mirrorXColorOutline = new Color(1f, 0f, 0f, 0.8f);
    private Color mirrorYColor = new Color(0f, 1f, 0f, 0.3f);
    private Color mirrorYColorOutline = new Color(0f, 1f, 0f, 0.8f);
    private Color mirrorZColor = new Color(0f, 0f, 1f, 0.3f);
    private Color mirrorZColorOutline = new Color(0f, 0f, 1f, 0.8f);

    private List<int> toRemove;

    SerializedObject so;

    private void OnEnable() {
        so = new SerializedObject(this);


        Selection.selectionChanged += Repaint;
        SceneView.duringSceneGui += DuringSceneGUI;
    }
    private void OnDisable() {
        Selection.selectionChanged -= Repaint;
        SceneView.duringSceneGui -= DuringSceneGUI;
    }

    private void DuringSceneGUI(SceneView sceneView) {


        switch(Event.current.type) {
            case EventType.Repaint:
                DrawMirrorGizmos();
                break;
            default:
                break;
        }

    }

    private void OnGUI() {
        using(new GUILayout.VerticalScope(EditorStyles.helpBox)) {

            if(GUILayout.Button("MIRROR")) MirrorObjects();

            mirrorTf = EditorGUILayout.ObjectField("Mirror", mirrorTf, typeof(Transform), true) as Transform;

            lastMirrorX = mirrorX;
            lastMirrorY = mirrorY;
            lastMirrorZ = mirrorZ;

            mirrorX = EditorGUILayout.Toggle("X", mirrorX);
            mirrorY = EditorGUILayout.Toggle("Y", mirrorY);
            mirrorZ = EditorGUILayout.Toggle("Z", mirrorZ);

            if(mirrorX != lastMirrorX || mirrorY != lastMirrorY || mirrorZ != lastMirrorZ) {
                SceneView.RepaintAll();
            }

            advancedSettingsToggle = EditorGUILayout.Foldout(advancedSettingsToggle, "Advanced Settings");

            if(advancedSettingsToggle) {

                using(new GUILayout.VerticalScope(EditorStyles.helpBox)) {

                    gizmoRectHeight = EditorGUILayout.FloatField("Gizmo Height", gizmoRectHeight);
                    gizmoRectWidth = EditorGUILayout.FloatField("Gizmo Width", gizmoRectWidth);

                    advancedSettingsMirrorX = EditorGUILayout.Foldout(advancedSettingsMirrorX, "Mirror X Settings");
                    if(advancedSettingsMirrorX) {
                        mirrorXColor = EditorGUILayout.ColorField("Plane Color X", mirrorXColor);
                        mirrorXColorOutline = EditorGUILayout.ColorField("Outline Color X", mirrorXColorOutline);
                    }

                    advancedSettingsMirrorY = EditorGUILayout.Foldout(advancedSettingsMirrorY, "Mirror Y Settings");
                    if(advancedSettingsMirrorY) {
                        mirrorYColor = EditorGUILayout.ColorField("Plane Color Y", mirrorYColor);
                        mirrorYColorOutline = EditorGUILayout.ColorField("Outline Color Y", mirrorYColorOutline);
                    }

                    advancedSettingsMirrorZ = EditorGUILayout.Foldout(advancedSettingsMirrorZ, "Mirror Z Settings");
                    if(advancedSettingsMirrorZ) {
                        mirrorZColor = EditorGUILayout.ColorField("Plane Color Z", mirrorZColor);
                        mirrorZColorOutline = EditorGUILayout.ColorField("Outline Color Z", mirrorZColorOutline);
                    }

                    if(GUILayout.Button("Reset Values")) SetDefaultParams();
                }


            }



            if(GUILayout.Button("Add to Mirror")) AddSourceObjects();
            if(GUILayout.Button("Clean")) CleanMirror();


            using(new GUILayout.VerticalScope()) {

                sourceObjectsToogle = EditorGUILayout.Foldout(sourceObjectsToogle, "Mirrored Objects");

                if(sourceObjectsToogle) {


                    scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
                    if(toRemove.Count > 0)
                        toRemove.Clear();

                    for(int i = 0; i < sourceObjects.Count; i++) {
                        using(new GUILayout.HorizontalScope()) {
                            EditorGUILayout.LabelField(sourceObjects[i].name);
                            if(GUILayout.Button("X")) toRemove.Add(i);
                        }
                    }

                    if(toRemove.Count > 0) RemoveObjects();
                    EditorGUILayout.EndScrollView();

                }


            }
        }
    }

    private void MirrorObjects() {
        foreach(GameObject sourceObject in sourceObjects) {
            if(false) {
                GameObject mirroredObject = Instantiate<GameObject>(sourceObject, Vector3.zero, Quaternion.identity);
                mirroredObject.name += " (Mirrored)";

                mirroredObjects.Add(mirroredObject);
                Debug.Log(sourceObjects.Count + "|" + mirroredObjects.Count);
            }
        }
        for(int i = 0; i < mirroredObjects.Count; i++) {
            mirroredObjects[i].transform.position = GetMirroredPosition(sourceObjects[i].transform.position);
        }

        foreach(GameObject source in sourceObjects) {
            Debug.DrawLine(GetMirroredPosition(source.transform.position), source.transform.position, Color.green);
        }
    }

    private Vector3 GetMirroredPosition(Vector3 originalPos) {
        Vector3 mirrorPoint = mirrorTf.position;
        float lastDistance;
        float checkDelta = 0.1f;

        lastDistance = Vector3.Distance(mirrorPoint, originalPos);

        if(lastDistance > Vector3.Distance(mirrorPoint + mirrorTf.forward * checkDelta, originalPos)) {
            mirrorPoint = FindMirrorPoint(originalPos, 1, checkDelta);
        }
        else if(lastDistance > Vector3.Distance(mirrorPoint - mirrorTf.forward * checkDelta, originalPos)) {
            mirrorPoint = FindMirrorPoint(originalPos, -1, checkDelta);
        }

        mirrorPoint.y = originalPos.y;

        return 2 * mirrorPoint - originalPos;
        
    }

    private Vector3 FindMirrorPoint(Vector3 originalPos, int checkDirection, float checkDelta) {
        Vector3 mirrorPoint = mirrorTf.position;
        float lastDistance;

        do {
            lastDistance = Vector3.Distance(mirrorPoint, originalPos);
            mirrorPoint += mirrorTf.forward * checkDirection * checkDelta;

        } while(Vector3.Distance(mirrorPoint + mirrorTf.forward * checkDirection * checkDelta, originalPos) < lastDistance);

        return mirrorPoint;
    }

    private void SetDefaultParams() {
        gizmoRectHeight = 1f;
        gizmoRectWidth = 2f;

        mirrorXColor = new Color(1f, 0f, 0f, 0.1f);
        mirrorXColorOutline = new Color(1f, 0f, 0f, 0.8f);
        mirrorYColor = new Color(0f, 1f, 0f, 0.1f);
        mirrorYColorOutline = new Color(0f, 1f, 0f, 0.8f);
        mirrorZColor = new Color(0f, 0f, 1f, 0.1f);
        mirrorZColorOutline = new Color(0f, 0f, 1f, 0.8f);
        SceneView.RepaintAll();
    }

    private void AddSourceObjects() {
        foreach(GameObject sourceObject in Selection.objects) {
            if(!sourceObjects.Contains(sourceObject)) {
                sourceObjects.Add(sourceObject);
            }
        }
    }

    private void CleanMirror() {
        sourceObjects.Clear();
        mirroredObjects.Clear();
    }

    private void RemoveObjects() {
        foreach(int i in toRemove) {
            sourceObjects.RemoveAt(i);
            mirroredObjects.RemoveAt(i);
        }
    }

    private void DrawMirrorGizmos() {
        if(mirrorTf != null) {
            if(mirrorX) DrawMirrorGizmoX();
            if(mirrorY) DrawMirrorGizmoY();
            if(mirrorZ) DrawMirrorGizmoZ();
        } else {
            Debug.LogWarning("Mirror object Missing");
        }
    }

    private void DrawMirrorGizmoX() {
        Vector3[] rectVerts = new Vector3[] {
                mirrorTf.position + mirrorTf.forward * (gizmoRectWidth/2) + mirrorTf.up * (gizmoRectHeight/2),
                mirrorTf.position + mirrorTf.forward * (gizmoRectWidth/2) - mirrorTf.up * (gizmoRectHeight/2),
                mirrorTf.position - mirrorTf.forward * (gizmoRectWidth/2) - mirrorTf.up * (gizmoRectHeight/2),
                mirrorTf.position - mirrorTf.forward * (gizmoRectWidth/2) + mirrorTf.up * (gizmoRectHeight/2)
            };
        Handles.DrawSolidRectangleWithOutline(rectVerts, mirrorXColor, mirrorXColorOutline);
    }

    private void DrawMirrorGizmoY() {
        Vector3[] rectVerts = new Vector3[] {
                mirrorTf.position + mirrorTf.forward * (gizmoRectWidth/2) + mirrorTf.right * (gizmoRectHeight/2),
                mirrorTf.position + mirrorTf.forward * (gizmoRectWidth/2) - mirrorTf.right * (gizmoRectHeight/2),
                mirrorTf.position - mirrorTf.forward * (gizmoRectWidth/2) - mirrorTf.right * (gizmoRectHeight/2),
                mirrorTf.position - mirrorTf.forward * (gizmoRectWidth/2) + mirrorTf.right * (gizmoRectHeight/2)
            };

        Handles.DrawSolidRectangleWithOutline(rectVerts, mirrorYColor, mirrorYColorOutline);
    }

    private void DrawMirrorGizmoZ() {
        Vector3[] rectVerts = new Vector3[] {
                mirrorTf.position + mirrorTf.up * (gizmoRectWidth/2) + mirrorTf.right * (gizmoRectHeight/2),
                mirrorTf.position + mirrorTf.up * (gizmoRectWidth/2) - mirrorTf.right * (gizmoRectHeight/2),
                mirrorTf.position - mirrorTf.up * (gizmoRectWidth/2) - mirrorTf.right * (gizmoRectHeight/2),
                mirrorTf.position - mirrorTf.up * (gizmoRectWidth/2) + mirrorTf.right * (gizmoRectHeight/2)
            };

        Handles.DrawSolidRectangleWithOutline(rectVerts, mirrorZColor, mirrorZColorOutline);
    }

}
