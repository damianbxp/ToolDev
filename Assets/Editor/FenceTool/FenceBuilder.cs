using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Fence Builder Window. Allows to create and manage fences in scene
/// </summary>

public class FenceBuilder : EditorWindow
{
    /// <summary>
    /// Method used to open window
    /// </summary>
    [MenuItem("Tools/Fence Builder")]
    public static void OpenFenceBuilder() => GetWindow<FenceBuilder>("Fence Builder");

    private List<Fence> fences;
    
    private GameObject fencesMaster;

    private const string fenceMasterName = "Fences Master";

    private void OnEnable() {
        fencesMaster = GameObject.Find(fenceMasterName);
        SceneView.duringSceneGui += DuringSceneGUI;
    }
    private void OnDisable() {
        SceneView.duringSceneGui -= DuringSceneGUI;
    }
    /// <summary>
    /// Being called whem active scene redraws
    /// </summary>
    private void DuringSceneGUI(SceneView sceneView) {

    }

    private void OnGUI() {
        using( new GUILayout.VerticalScope(EditorStyles.helpBox)) {
            if(GUILayout.Button("Create Fence")) CreateFence();
        }
    }
    /// <summary>
    /// Create new Fence
    /// </summary>
    public void CreateFence() {
        if(fencesMaster != null && GameObject.Find(fenceMasterName) != null) {
            fencesMaster = GameObject.Find(fenceMasterName);
        } else {
            fencesMaster = new GameObject(fenceMasterName);
            fencesMaster.isStatic = true;
        }


    }
}
