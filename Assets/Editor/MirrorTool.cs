using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MirrorTool : EditorWindow {
    [MenuItem("Tools/Mirror Tool")]
    public static void OpenMirrorTool() => GetWindow<MirrorTool>("Mirror Tool");

    public List<GameObject> sourceObjects;
    public Transform mirrorTf;

    private bool sourceObjectsToogle;
    private Vector2 scrollPos;

    private bool mirrorX = false;
    private bool mirrorY = false;
    private bool mirrorZ = false;

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



    private void OnEnable() {

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
                DrawMirrorGizmo();
                break;
            default:
                break;
        }

    }

    private void OnGUI() {
        using(new GUILayout.VerticalScope(EditorStyles.helpBox)) {

            mirrorTf = EditorGUILayout.ObjectField("Mirror", mirrorTf, typeof(Transform), true) as Transform;


            mirrorX = EditorGUILayout.Toggle("X", mirrorX);
            mirrorY = EditorGUILayout.Toggle("Y", mirrorY);
            mirrorZ = EditorGUILayout.Toggle("Z", mirrorZ);

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

    private void SetDefaultParams() {
        gizmoRectHeight = 1f;
        gizmoRectWidth = 2f;

        mirrorXColor = new Color(1f, 0f, 0f, 0.3f);
        mirrorXColorOutline = new Color(1f, 0f, 0f, 0.8f);
        mirrorYColor = new Color(0f, 1f, 0f, 0.3f);
        mirrorYColorOutline = new Color(0f, 1f, 0f, 0.8f);
        mirrorZColor = new Color(0f, 0f, 1f, 0.3f);
        mirrorZColorOutline = new Color(0f, 0f, 1f, 0.8f);
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
    }

    private void RemoveObjects() {
        foreach(int i in toRemove) {
            sourceObjects.RemoveAt(i);
        }
    }

    private void DrawMirrorGizmo() {
        if(mirrorTf != null) {
            if(mirrorX) DrawMirrorGizmosX();
            if(mirrorY) DrawMirrorGizmosY();
            if(mirrorZ) DrawMirrorGizmosZ();
        } else {
            Debug.LogWarning("Mirror object Missing");
        }
    }

    private void DrawMirrorGizmosX() {
        Vector3[] rectVerts = new Vector3[] {
                mirrorTf.position + mirrorTf.forward * (gizmoRectWidth/2) + mirrorTf.up * (gizmoRectHeight/2),
                mirrorTf.position + mirrorTf.forward * (gizmoRectWidth/2) - mirrorTf.up * (gizmoRectHeight/2),
                mirrorTf.position - mirrorTf.forward * (gizmoRectWidth/2) - mirrorTf.up * (gizmoRectHeight/2),
                mirrorTf.position - mirrorTf.forward * (gizmoRectWidth/2) + mirrorTf.up * (gizmoRectHeight/2)
            };
        Handles.DrawSolidRectangleWithOutline(rectVerts, mirrorXColor, mirrorXColorOutline);
    }

    private void DrawMirrorGizmosY() {
        Vector3[] rectVerts = new Vector3[] {
                mirrorTf.position + mirrorTf.forward * (gizmoRectWidth/2) + mirrorTf.right * (gizmoRectHeight/2),
                mirrorTf.position + mirrorTf.forward * (gizmoRectWidth/2) - mirrorTf.right * (gizmoRectHeight/2),
                mirrorTf.position - mirrorTf.forward * (gizmoRectWidth/2) - mirrorTf.right * (gizmoRectHeight/2),
                mirrorTf.position - mirrorTf.forward * (gizmoRectWidth/2) + mirrorTf.right * (gizmoRectHeight/2)
            };

        Handles.DrawSolidRectangleWithOutline(rectVerts, mirrorYColor, mirrorYColorOutline);
    }

    private void DrawMirrorGizmosZ() {
        Vector3[] rectVerts = new Vector3[] {
                mirrorTf.position + mirrorTf.up * (gizmoRectWidth/2) + mirrorTf.right * (gizmoRectHeight/2),
                mirrorTf.position + mirrorTf.up * (gizmoRectWidth/2) - mirrorTf.right * (gizmoRectHeight/2),
                mirrorTf.position - mirrorTf.up * (gizmoRectWidth/2) - mirrorTf.right * (gizmoRectHeight/2),
                mirrorTf.position - mirrorTf.up * (gizmoRectWidth/2) + mirrorTf.right * (gizmoRectHeight/2)
            };

        Handles.DrawSolidRectangleWithOutline(rectVerts, mirrorZColor, mirrorZColorOutline);
    }

}
