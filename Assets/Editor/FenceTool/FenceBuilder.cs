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

    private GameObject fencesMaster;

    private string newFenceName;
    private int editFenceId;
    private Vector2 scrollPos;
    private const string fenceMasterName = "Fences Master";

    private void OnEnable() {
        fencesMaster = GameObject.Find(fenceMasterName);
        SceneView.duringSceneGui += DuringSceneGUI;
    }
    private void OnDisable() {
        SceneView.duringSceneGui -= DuringSceneGUI;
    }

    private void DuringSceneGUI(SceneView sceneView) {
        GetFence(editFenceId).DrawGizmosLines();
    }

    private void OnGUI() {
        using(new GUILayout.HorizontalScope(EditorStyles.helpBox)) {
            newFenceName = EditorGUILayout.TextField(newFenceName);
            if(GUILayout.Button("Create Fence")) CreateFence(newFenceName);
        }

        using(new GUILayout.VerticalScope()) {
            editFenceId = EditorGUILayout.Popup(editFenceId, GetFencesNames());
            if(GUILayout.Button("Create Waypoint")) CreateWaypoint(GetFence(editFenceId), Vector3.zero);
        }


        scrollPos = GUILayout.BeginScrollView(scrollPos);
        for(int i = 0; i < fencesMaster.transform.childCount; i++) {
            GetFence(i).expandDisplay = EditorGUILayout.Foldout(GetFence(i).expandDisplay, GetFenceName(i));

        }
        GUILayout.EndScrollView();

    }
    /// <summary>
    /// Create new Fence. Create new fenceMaster if needed. 
    /// </summary>
    /// <param name="name">Name of new fence</param>
    /// <param name="pos">Position of new fence root</param>
    public void CreateFence(string name, Vector3 pos) {
        if(fencesMaster != null && GameObject.Find(fenceMasterName) != null) {
            fencesMaster = GameObject.Find(fenceMasterName);
        } else {
            fencesMaster = new GameObject(fenceMasterName);
            fencesMaster.isStatic = true;
        }

        GameObject newFence = new GameObject(name);
        newFence.transform.SetParent(fencesMaster.transform);
        newFence.AddComponent<Fence>();
        newFence.transform.position = pos;

    }

    /// <summary>
    /// Create new Fence. Create new fenceMaster if needed. 
    /// </summary>
    /// <param name="name">Name of new fence</param>
    public void CreateFence(string name) => CreateFence(name, Vector3.zero);

    /// <summary>
    /// Creates new waypoint in given fence
    /// </summary>
    /// <param name="fence">Specify which fence should create waypoint</param>
    /// <param name="pos">Position for new waypoint</param>
    public void CreateWaypoint(Fence fence, Vector3 pos) {
        fence.CreateWaypoint(pos);
    }

    /// <summary>
    /// Get fence reference
    /// </summary>
    /// <param name="i">Fence Id</param>
    /// <returns>Fence at given id</returns>
    public Fence GetFence(int i) => fencesMaster.transform.GetChild(i).GetComponent<Fence>();

    /// <summary>
    /// Get name of fence at Id
    /// </summary>
    /// <param name="i">Fence Id</param>
    /// <returns>Name of fence</returns>
    public string GetFenceName(int i) => fencesMaster.transform.GetChild(i).name;

    /// <summary>
    /// Returns names of all fences
    /// </summary>
    /// <returns>Names of all fences</returns>
    public string[] GetFencesNames() {
        string[] fenceNames = new string[fencesMaster.transform.childCount];

        for(int i = 0; i < fencesMaster.transform.childCount; i++) {
            fenceNames[i] = GetFenceName(i);
        }

        return fenceNames;
    }
}
