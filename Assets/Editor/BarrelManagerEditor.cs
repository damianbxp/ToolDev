using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[CustomEditor(typeof(BarrelManager))]
public class BarrelManagerEditor : Editor
{

    List<BarrelTypes> barrelTypes;
    string[] barrels;
    int index;

    string[] damageAreaDisplayMode = { "See through sphere", "WireSphere", "WireDisc", "Line on ground" };



    public override void OnInspectorGUI() {

        UpdateBarrelTypesArray();
        int lastIndex = index;
        bool lastDrawDamageArea;
        int lastDisplayMode;

        using(new GUILayout.VerticalScope(EditorStyles.helpBox)) {
            index = EditorGUILayout.Popup(index, barrels);
            using(new GUILayout.HorizontalScope()) {
                GUILayout.Label("Draw Damage Sphere");
                lastDrawDamageArea = BarrelManager.drawDamageArea;
                BarrelManager.drawDamageArea = EditorGUILayout.Toggle(BarrelManager.drawDamageArea);
            }

            using(new GUILayout.HorizontalScope()) {
                GUILayout.Label("Draw Type");
                lastDisplayMode = BarrelManager.displayMode;
                BarrelManager.displayMode = EditorGUILayout.Popup(BarrelManager.displayMode, damageAreaDisplayMode);
            }
        }


        if(lastIndex != index || lastDrawDamageArea != BarrelManager.drawDamageArea || lastDisplayMode != BarrelManager.displayMode) {
            if(index == 0) BarrelManager.displayedType = null;
            else {
                BarrelManager.displayedType = barrelTypes[index-1];
            }
            SceneView.RepaintAll();
        }
    }

    public void UpdateBarrelTypesArray() {
        barrelTypes = FindAssetsByType<BarrelTypes>();
        List<string> barrelTypesNames = new List<string>();
        barrelTypesNames.Add("All");
        foreach(BarrelTypes barrel in barrelTypes) {
            barrelTypesNames.Add(barrel.name);
        }
        barrels = barrelTypesNames.ToArray();
    }


    public static List<T> FindAssetsByType<T>() where T : UnityEngine.Object {
        List<T> assets = new List<T>();
        string[] guids = AssetDatabase.FindAssets(string.Format("t:{0}", typeof(T).ToString().Replace("UnityEngine.", "")));
        for(int i = 0; i < guids.Length; i++) {
            string assetPath = AssetDatabase.GUIDToAssetPath(guids[i]);
            T asset = AssetDatabase.LoadAssetAtPath<T>(assetPath);
            if(asset != null) {
                assets.Add(asset);
            }
        }
        return assets;
    }
}
