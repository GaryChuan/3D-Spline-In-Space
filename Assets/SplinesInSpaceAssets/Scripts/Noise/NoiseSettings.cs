using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NoiseSettings
{
    public enum FilterType { Simple, Rigid };
    public FilterType filterType;

    [ConditionalHide("filterType", 0)]
    public SimpleNoiseSettings simpleNoiseSettings;

    [ConditionalHide("filterType", 1)]
    public RigidNoiseSettings rigidNoiseSettings;

    [System.Serializable]
    public class SimpleNoiseSettings
    {
        [Range(1, 8)]
        [TextArea]
        [Tooltip("Number of times the noise gets processed")]
        public int numLayer = 1;
        public float strength;
        public float baseRoughness = 1;
        [Tooltip("Factor of noise frequency per layer")]
        public float roughness = 2;
        [Tooltip("Factor of noise amplitude per layer")]
        public float persistence = .5f;
        [Tooltip("Clamps the final noise value")]
        public float minValue = 0;
        public Vector3 centre;
    }

    [System.Serializable]
    public class RigidNoiseSettings : SimpleNoiseSettings
    {
        public float weightMultipler = 0.8f;
    }
}
