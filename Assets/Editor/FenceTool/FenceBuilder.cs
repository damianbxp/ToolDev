using System;
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
    public GameObject postPrefab;

    int pickedTool = 0;
    string[] tools = { "Edit", "Create" };

    public int editFenceId = 0;
    private Fence editFence;

    private string newFenceName;
    private Vector2 scrollPos;
    private const string fenceMasterName = "Fences Master";

    private bool settingsDisplay = false;
    public float groundScanRange = 5f;
    public float groundScanHeightOffset = 2f;

    SerializedObject so;
    SerializedProperty editFenceIdProp;
    SerializedProperty postPrefabProp;

    SerializedProperty groundScanRangeProp;
    SerializedProperty groundScanHeightOffsetProp;

    #region 
    private int lastEditFenceId;
    private bool lastClosedPath;
    #endregion
    private void OnEnable() {
        so = new SerializedObject(this);
        editFenceIdProp = so.FindProperty("editFenceId");
        postPrefabProp = so.FindProperty("postPrefab");
        groundScanRangeProp = so.FindProperty("groundScanRange");
        groundScanHeightOffsetProp = so.FindProperty("groundScanHeightOffset");
        fencesMaster = GameObject.Find(fenceMasterName);
        UpdateEditFence();

        SceneView.duringSceneGui += DuringSceneGUI;
    }
    private void OnDisable() {
        SceneView.duringSceneGui -= DuringSceneGUI;
    }

    private void DuringSceneGUI(SceneView sceneView) {
        if(editFence == null) UpdateEditFence();

        bool LMBClick = false;
        if(Event.current.type == EventType.MouseDown && Event.current.button == 0) LMBClick = true;

        Ray mouseRay = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
        if(Physics.Raycast(mouseRay, out RaycastHit hit)) {

            switch(pickedTool) {
                case 0: {//edit
                    
                    EditTool(hit.point);
                    break;
                }
                case 1: {//create

                    break;
                }
                default: {
                    Debug.LogError("Can't run picked tool");
                    break;
                }
            }
        }


    }

    private void OnGUI() {
        so.Update();

        using(new GUILayout.HorizontalScope(EditorStyles.helpBox)) {
            pickedTool = GUILayout.Toolbar(pickedTool, tools);
        }

        using(new GUILayout.HorizontalScope(EditorStyles.helpBox)) {
            newFenceName = EditorGUILayout.TextField(newFenceName);
            if(GUILayout.Button("Create Fence")) CreateFence(newFenceName);
        }

        using(new GUILayout.VerticalScope()) {
            editFenceIdProp.intValue = EditorGUILayout.Popup(editFenceIdProp.intValue, GetFencesNames());
            if(GUILayout.Button("Create Waypoint")) CreateWaypoint(editFence, Vector3.zero);
            using(new GUILayout.HorizontalScope()) {
                if(GUILayout.Button("Fix Naming")) FixNames(editFence);
                if(GUILayout.Button("Rebuild")) RebuildFence(editFence);
            }
            EditorGUILayout.PropertyField(postPrefabProp);
            lastClosedPath = editFence.closedPath;
            editFence.closedPath = EditorGUILayout.Toggle("Closed Path", editFence.closedPath);
            if(lastClosedPath != editFence.closedPath) SceneView.RepaintAll();
        }

        settingsDisplay = EditorGUILayout.Foldout(settingsDisplay, "Settings");
        if(settingsDisplay) {
            using(new GUILayout.VerticalScope(EditorStyles.helpBox)) {
                EditorGUILayout.PropertyField(groundScanRangeProp);
                EditorGUILayout.PropertyField(groundScanHeightOffsetProp);
            }
        }

        scrollPos = GUILayout.BeginScrollView(scrollPos);
        for(int i = 0; i < fencesMaster.transform.childCount; i++) {
            GetFence(i).expandDisplay = EditorGUILayout.Foldout(GetFence(i).expandDisplay, GetFenceName(i));
            if(GetFence(i).expandDisplay) {
                UpdateEditFence();
            }
        }
        GUILayout.EndScrollView();
        if(so.ApplyModifiedProperties()) {
            UpdateEditFence();
            UpdateSettings();
            Repaint();
        }
        //True if Undo or Redo used
        if(Event.current.type == EventType.ValidateCommand)
            if(Event.current.commandName == "UndoRedoPerformed") {
                UpdateEditFence();
                Repaint();
            }
        
    }

    private void UpdateSettings() => editFence.UpdateSettings(groundScanRange, groundScanHeightOffset);

    /// <summary>
    /// Create new Fence. Create new fenceMaster if needed. 
    /// </summary>
    /// <param name="name">Name of new fence</param>
    /// <param name="pos">Position of new fence root</param>
    private void CreateFence(string name, Vector3 pos) {
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
    private void CreateFence(string name) => CreateFence(name, Vector3.zero);

    /// <summary>
    /// Creates new waypoint in given fence
    /// </summary>
    /// <param name="fence">Specify which fence should create waypoint</param>
    /// <param name="pos">Position for new waypoint</param>
    private void CreateWaypoint(Fence fence, Vector3 pos) {
        fence.CreateWaypoint(pos);
    }

    /// <summary>
    /// Get fence reference
    /// </summary>
    /// <param name="i">Fence Id</param>
    /// <returns>Fence at given id</returns>
    private Fence GetFence(int i) => fencesMaster.transform.GetChild(i).GetComponent<Fence>();

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
   
    /// <summary>
    /// Generates names of fence waypoints based on fence name by adding index
    /// </summary>
    private void FixNames(Fence fence) => fence.FixNames();
   
    private void RebuildFence(Fence fence) => fence.Rebuild(postPrefabProp.objectReferenceValue as GameObject);
    private void UpdateEditFence() {
        editFence = GetFence(editFenceId);
        SceneView.RepaintAll();
    }
    private void EditTool( Vector3 mouseWorldPos) {
        editFence.DrawHandles();
        editFence.DrawGizmosLines();
    }
    private void CreateTool() {

    }

}
