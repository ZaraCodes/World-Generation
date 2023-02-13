using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GeneratorSettings", menuName = "Create Generator Settings")]
public class GeneratorSettings : ScriptableObject
{
    public float NoiseStrength;
    public float Frequency;
    public int Octaves;
    public float Lacunarity;
    public float Persistence;

    public float HillinessFrequency;
    public float BaseHeightFrequency;
    public float BaseHeightMultiplier;
    public float BaseHeight;

    public int ChunkResolution;
    public int ChunkSize;
}
