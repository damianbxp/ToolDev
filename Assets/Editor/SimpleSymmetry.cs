using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SimpleSymmetry : EditorWindow {
    [MenuItem("Tools/Simple Symmetry")]
    public static void OpenSimpleSymmetry() => GetWindow<SimpleSymmetry>("Simple Symmetry");

    private List<MirrorPair> sourceObjects;

    private Transform mirror;

    private bool showSourceObjectsList = false;
    private Vector2 scrollPosition;

    private List<int> toRemove = new List<int>();

    private bool liveUpdate = false;

    private void OnEnable() {
        if(sourceObjects == null) sourceObjects = new List<MirrorPair>();
        SceneView.duringSceneGui += DuringSceneGUI;
    }

    private void OnDisable() {
        SceneView.duringSceneGui -= DuringSceneGUI;
    }

    private void DuringSceneGUI(SceneView sceneView) {
        if(liveUpdate) UpdateMirror();

    }

    private void OnGUI() {
        using(new GUILayout.VerticalScope(EditorStyles.helpBox)) {
            using(new GUILayout.HorizontalScope()) {
                if(GUILayout.Button("Update")) UpdateMirror();
                if(GUILayout.Button("Recopy")) RecopyObjects();
            }
            liveUpdate = EditorGUILayout.Toggle("Live Update", liveUpdate);
            EditorGUILayout.Space();
            if(GUILayout.Button("Add Selection")) AddSelection();
            if(GUILayout.Button("Clear")) ClearSourceObjects();

            mirror = EditorGUILayout.ObjectField("Mirror", mirror, typeof(Transform), true) as Transform;

            showSourceObjectsList = EditorGUILayout.Foldout(showSourceObjectsList, "Mirrored Objects");
            if(showSourceObjectsList) {
                using(new GUILayout.ScrollViewScope(scrollPosition, EditorStyles.helpBox)) {
                    if(toRemove.Count > 0)
                        toRemove.Clear();

                    for(int i = 0; i < sourceObjects.Count; i++) {
                        using(new GUILayout.HorizontalScope(EditorStyles.helpBox)) {
                            EditorGUILayout.LabelField(sourceObjects[i].source.name);
                            if(GUILayout.Button("X")) toRemove.Add(i);
                        }
                    }

                    if(toRemove.Count > 0) RemoveSelected();
                }
            }
        }
    }

    private void ClearSourceObjects() {
        foreach(MirrorPair mirrorPair in sourceObjects) {
            mirrorPair.DestroyCopy();
        }
        sourceObjects.Clear();
    }

    private void RemoveSelected() {
        foreach(int i in toRemove) {
            sourceObjects[i].DestroyCopy();
            sourceObjects.RemoveAt(i);
        }
    }

    private void AddSelection() {
        foreach(GameObject selectedObject in Selection.objects) {
            if(!sourceObjects.Contains(selectedObject)) {
                MirrorPair newMirrorPair = new MirrorPair(selectedObject);
                sourceObjects.Add(newMirrorPair);
            }
        }
    }

    private void UpdateMirror() {
        if(MirrorTfCheck()) {
            foreach(MirrorPair mirrorPair in sourceObjects) {
                mirrorPair.UpdateMirrorTf(mirror);
                mirrorPair.UpdateMirror();
            }
        }
    }
    private void RecopyObjects() {
        if(MirrorTfCheck()) {
            foreach(MirrorPair mirrorPair in sourceObjects) {
                mirrorPair.Recopy();
            }
        }
    }

    private bool MirrorTfCheck() {
       
        if( !(mirror == null)) {
            return true;
        } else {
            Debug.LogError("Mirror Not Added");
            return false;
        }
       
    }
}
