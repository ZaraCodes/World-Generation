using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleNoiseFilter
{
    NoiseSettings noiseSettings;
    SimplexNoise noise;

    public SimpleNoiseFilter(NoiseSettings _settings)
    {
        noiseSettings = _settings;
        noise = new SimplexNoise(69);
    }

    public float Evaluate(Vector3 _point)
    {
        float noiseValue = 0;
        float frequency = noiseSettings.Frequency;
        float amplitude = noiseSettings.Amplitude;

        for (int i = 0; i < noiseSettings.LayerCount; i++)
        {
            float v = noise.Evaluate(_point * frequency + noiseSettings.NoiseCenter);
            noiseValue += (v + 1) * 0.5f * amplitude;

            frequency *= noiseSettings.Roughness;
            amplitude *= noiseSettings.Persistence;
        }

        noiseValue = Mathf.Max(0, noiseValue - noiseSettings.GroundLevel);

        return noiseValue * noiseSettings.Strength;
    }
}
