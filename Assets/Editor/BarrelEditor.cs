using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CanEditMultipleObjects]
[CustomEditor(typeof(BarrelTypes))]
public class BarrelEditor : Editor
{
    SerializedObject so;    

    SerializedProperty propRadius;
    SerializedProperty propColor;

    private void OnEnable() {
        so = serializedObject;
        propRadius = so.FindProperty("radius");
        propColor = so.FindProperty("color");
    }

    public override void OnInspectorGUI() {

        using(new GUILayout.VerticalScope(EditorStyles.helpBox)) {

            GUILayout.Label("Barrel", EditorStyles.boldLabel);
            GUILayout.Space(10);

            so.Update();
            EditorGUILayout.PropertyField(propRadius);
            EditorGUILayout.PropertyField(propColor);
            if(so.ApplyModifiedProperties()) {
                BarrelManager.UpdateAllBarrelsColors();
            }
        }

    }
}
