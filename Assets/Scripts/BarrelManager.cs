using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class BarrelManager : MonoBehaviour {
    public static List<Barrel> barrels = new List<Barrel>();
    public static BarrelTypes displayedType;
    public static bool drawDamageArea;
    public static int displayMode;

    public static int drawPrecision;

    public static void UpdateAllBarrelsColors() {
        foreach(Barrel barrel in barrels) {
            barrel.TryApplyColor();
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected() {



        foreach(Barrel barrel in barrels) {
            
            if(barrel.type == displayedType || displayedType == null) { 
            
                Vector3 managerPos = transform.position;
                Vector3 barrelPos = barrel.transform.position;

                float halfHeight = (managerPos.y - barrelPos.y) * 0.5f;
                Vector3 offset = Vector3.up * halfHeight;

                Handles.DrawBezier(managerPos, barrelPos, managerPos - offset, barrelPos + offset, Color.green, EditorGUIUtility.whiteTexture, 1f);

                if(drawDamageArea) {
                    barrel.DrawDamageArea(displayMode);
                }
            }


        }
    }
#endif
}
