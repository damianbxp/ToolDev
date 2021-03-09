using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Fence Builder Window
/// </summary>

public class FenceBuilder : EditorWindow
{
    [MenuItem("Tools/Fence Builder")]
    public static void OpenFenceBuilder() => GetWindow<FenceBuilder>("Fence Builder");
    /// <summary>
    /// Stores all fences
    /// </summary>
    private List<Fence> fences;
    
    private GameObject fencesMaster;
    /// <value>
    /// Name of fence master
    /// </value>
    private const string fenceMasterName = "Fences Master";

    private void OnEnable() {
        fencesMaster = GameObject.Find(fenceMasterName);
        SceneView.duringSceneGui += DuringSceneGUI;
    }
    private void OnDisable() {
        SceneView.duringSceneGui -= DuringSceneGUI;
    }

    private void DuringSceneGUI(SceneView sceneView) {

    }

    private void OnGUI() {
        using( new GUILayout.VerticalScope(EditorStyles.helpBox)) {
            if(GUILayout.Button("Create Fence")) CreateFence();
        }
    }
    /// <summary>
    /// Creates New Fence. Adds new Fence Master if needed
    /// </summary>
    private void CreateFence() {
        if(fencesMaster != null && GameObject.Find(fenceMasterName) != null) {
            fencesMaster = GameObject.Find(fenceMasterName);
        } else {
            fencesMaster = new GameObject(fenceMasterName);
            fencesMaster.isStatic = true;
        }


    }
}
