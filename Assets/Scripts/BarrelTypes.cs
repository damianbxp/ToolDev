using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class BarrelTypes : ScriptableObject
{
    [Range(0f, 10f)]
    public float radius;

    public Color color;
}
