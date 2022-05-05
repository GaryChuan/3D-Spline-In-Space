using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class SplineSettings : ScriptableObject
{
    public Vector3 initialVelocity; 
    public Vector3 finalVelocity;
    public List<Vector3> points;
}
