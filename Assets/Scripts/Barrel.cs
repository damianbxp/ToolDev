using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteAlways]
[SelectionBase]
public class Barrel : MonoBehaviour {
    
    public BarrelTypes type;
    public Mesh sphereMesh;
    public Material material;
    MaterialPropertyBlock mpb;
    public MaterialPropertyBlock Mpb {
        get {
            if(mpb == null) {
                mpb = new MaterialPropertyBlock();
            }
            return mpb;
        }
    }

    static readonly int shPropColor = Shader.PropertyToID("_Color");

    public void TryApplyColor() {

        if(type == null)
            return;
        MeshRenderer meshRenderer = GetComponentInChildren<MeshRenderer>();
        Mpb.SetColor(shPropColor, type.color);
        meshRenderer.SetPropertyBlock(Mpb);
    }

    private void OnValidate() {
        TryApplyColor();
    }

    private void OnEnable() {
        BarrelManager.barrels.Add(this);
    }

    private void OnDisable() {
        BarrelManager.barrels.Remove(this);
    }

    private void OnDrawGizmosSelected() {
#if UNITY_EDITOR
        //DrawDamageArea();
#endif
    }

    public void DrawDamageArea(int drawMode) {
        if(type == null)
            return;

        switch(drawMode) {
            case 0:
                Color sphereColor = type.color;
                sphereColor.a = 0.2f;
                material.color = sphereColor;

                material.SetPass(0);
                Graphics.DrawMeshNow(sphereMesh, Matrix4x4.TRS(transform.position, Quaternion.identity, Vector3.one * type.radius * 2));
                break;
            case 1:
                Gizmos.DrawWireSphere(transform.position, type.radius);
                break;
            case 2:
                Handles.DrawWireDisc(transform.position, Vector3.up, type.radius);
                break;
            case 3:
                int rotDelta = 10;
                Vector3 raycastDir = Vector3.up;
                for(int i = 0; i < 180; i += rotDelta) {
                raycastDir = Quaternion.Euler(rotDelta, 0, 0) * raycastDir;
                    for(int j = 0; j < 360; j += rotDelta) {
                        raycastDir = Quaternion.Euler(0, rotDelta, 0) * raycastDir;
                        if(Physics.Raycast(transform.position, raycastDir, out RaycastHit hit, type.radius)) {
                        //Debug.DrawLine(transform.position, hit.point);
                        Gizmos.DrawSphere(hit.point, .05f);

                        }
                    }
                }
                break;
                
            default:
                Debug.LogWarning("Invalid Damage Draw Mode");
            break;
        }

        
        
        
        //Graphics.DrawMesh(sphereMesh, transform.position, Quaternion.identity, material, 0);
    }
}

