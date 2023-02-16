using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NoiseSettings
{
    public float Strength;

    [Range(1, 8)]
    public int LayerCount;
    public float Amplitude;
    public float Persistence;

    public float Frequency;
    public float Roughness;

    public Vector3 NoiseCenter;

    public float GroundLevel;
}
