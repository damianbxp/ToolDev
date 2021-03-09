using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MirrorPair {
    public GameObject source;
    public GameObject copy;

    private Transform mirrorTf;

    public MirrorPair(GameObject source_) {
        source = source_;
    }

    public void UpdateMirror() {
        Vector3 sourceRelPos = mirrorTf.InverseTransformDirection(source.transform.position - mirrorTf.position);
        sourceRelPos.z = 0;

        Vector3 mirrorPoint = source.transform.position - sourceRelPos.x * 2 * mirrorTf.right;

        Debug.DrawLine(source.transform.position, mirrorPoint, Color.green);

        if(copy == null)
            CreateNewMirrorObject();

        TransformMirrorObject(mirrorPoint);
    }

    private void CreateNewMirrorObject() {
        copy = MonoBehaviour.Instantiate(source);
        
    }
    private void TransformMirrorObject(Vector3 newPos) {
        copy.transform.position = newPos;
    }

    public void UpdateMirrorTf(Transform mirrorTf_) {
        mirrorTf = mirrorTf_;
    }

    public void DestroyCopy() {
        MonoBehaviour.DestroyImmediate(copy);
    }

    public void Recopy() {
        DestroyCopy();
        CreateNewMirrorObject();
    }
}
