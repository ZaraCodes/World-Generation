using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShapeGenerator
{
    ShapeSettings currentSettings;
    SimpleNoiseFilter[] noiseFilters;

    Vector3 position;
    Vector3 rotation;
    Vector3 scale;

    public void UpdateShapeSettings(ShapeSettings _newSettings, Vector3 _position, Vector3 _rotation, Vector3 _scale)
    {
        currentSettings = _newSettings;

        position = _position;
        rotation = _rotation;
        scale = _scale;

        noiseFilters = new SimpleNoiseFilter[_newSettings.NoiseLayers.Length];
        for (int i = 0; i < noiseFilters.Length; i++)
        {
            noiseFilters[i] = new SimpleNoiseFilter(_newSettings.NoiseLayers[i].NoiseSettings);
        }
    }

    public Vector3 TransformCubeToSpherePos(Vector3 _cubeVertPos, bool _useFancySphere)
    {
        if (!_useFancySphere)
            return _cubeVertPos.normalized;

        float x2 = _cubeVertPos.x * _cubeVertPos.x;
        float y2 = _cubeVertPos.y * _cubeVertPos.y;
        float z2 = _cubeVertPos.z * _cubeVertPos.z;

        float xPrime = _cubeVertPos.x * Mathf.Sqrt(1 - (y2 + z2) / 2 + (y2 * z2) / 3);
        float yPrime = _cubeVertPos.y * Mathf.Sqrt(1 - (x2 + z2) / 2 + (x2 * z2) / 3);
        float zPrime = _cubeVertPos.z * Mathf.Sqrt(1 - (x2 + y2) / 2 + (x2 * y2) / 3);

        return new Vector3(xPrime, yPrime, zPrime);
    }

    public Vector3 CalculatePointOnPlanet(Vector3 _spherePos)
    {
        Vector3 planetPos = _spherePos;

        float elevation = 0f;
        float firstLayerElevation = 0f;

        if (noiseFilters.Length > 0)
        {
            firstLayerElevation = noiseFilters[0].Evaluate(_spherePos);

            if (currentSettings.NoiseLayers[0].Enabled)
                elevation = firstLayerElevation;
        }

        //Start at idx 1 because first layer is already evaluated
        float mask = 0f;
        for (int i = 1; i < noiseFilters.Length; i++)
        {
            if (currentSettings.NoiseLayers[i].Enabled)
            {
                mask = currentSettings.NoiseLayers[i].UseFirstLayerAsMask ? firstLayerElevation : 1f;

                elevation += noiseFilters[i].Evaluate(_spherePos) * mask;
            }
        }

        return planetPos * currentSettings.PlanetRadius * (1 + elevation);
    }

    public Vector3 TransformPointWithOwnTransformMatrix(Vector3 _basePos)
    {
        Matrix4x4 transformMatrix = GetCurrTransformMatrix();

        Vector4 basePos4 = new Vector4(_basePos.x, _basePos.y, _basePos.z, 1);
        basePos4 = transformMatrix * basePos4;

        return new Vector3(basePos4.x, basePos4.y, basePos4.z);
    }

    private Matrix4x4 GetCurrTransformMatrix()
    {
        float xRotRad = rotation.x * Mathf.Deg2Rad;
        Matrix4x4 rotMatX = new Matrix4x4();
        rotMatX.SetRow(0, new Vector4(1, 0, 0, 0));
        rotMatX.SetRow(1, new Vector4(0, Mathf.Cos(xRotRad), -Mathf.Sin(xRotRad), 0));
        rotMatX.SetRow(2, new Vector4(0, Mathf.Sin(xRotRad), Mathf.Cos(xRotRad), 0));
        rotMatX.SetRow(3, new Vector4(0, 0, 0, 1));

        float yRotRad = rotation.y * Mathf.Deg2Rad;
        Matrix4x4 rotMatY = new Matrix4x4();
        rotMatY.SetRow(0, new Vector4(Mathf.Cos(yRotRad), 0, Mathf.Sin(yRotRad), 0));
        rotMatY.SetRow(1, new Vector4(0, 1, 0, 0));
        rotMatY.SetRow(2, new Vector4(-Mathf.Sin(yRotRad), 0, Mathf.Cos(yRotRad), 0));
        rotMatY.SetRow(3, new Vector4(0, 0, 0, 1));

        float zRotRad = rotation.z * Mathf.Deg2Rad;
        Matrix4x4 rotMatZ = new Matrix4x4();
        rotMatZ.SetRow(0, new Vector4(Mathf.Cos(zRotRad), -Mathf.Sin(zRotRad), 0, 0));
        rotMatZ.SetRow(1, new Vector4(Mathf.Sin(zRotRad), Mathf.Cos(zRotRad), 0, 0));
        rotMatZ.SetRow(2, new Vector4(0, 0, 1, 0));
        rotMatZ.SetRow(3, new Vector4(0, 0, 0, 1));

        //Base Matrix mit Rotation
        Matrix4x4 transformMatrix = rotMatX * rotMatY * rotMatZ;

        //Skalierung
        transformMatrix.m00 *= scale.x;
        transformMatrix.m11 *= scale.y;
        transformMatrix.m22 *= scale.z;

        //Translation
        transformMatrix.m03 = position.x;
        transformMatrix.m13 = position.y;
        transformMatrix.m23 = position.z;

        return transformMatrix;
    }
}
