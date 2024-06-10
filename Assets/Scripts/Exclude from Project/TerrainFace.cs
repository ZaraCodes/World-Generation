using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainFace
{
    private ShapeGenerator shapeGenerator;

    private Mesh mesh;
    private int currentResolution;
    private bool useFancySphere;

    private Vector3 localUpVector;
    private Vector3 axisA;
    private Vector3 axisB;

    public TerrainFace(Mesh _mesh, ShapeGenerator _shapeGenerator, int _resolution, Vector3 _localUp, bool _useFancySphere)
    {
        shapeGenerator = _shapeGenerator;

        mesh = _mesh;
        currentResolution = _resolution;
        useFancySphere = _useFancySphere;

        localUpVector = _localUp;
        axisA = new Vector3(localUpVector.y, localUpVector.z, localUpVector.x);
        axisB = Vector3.Cross(axisA, localUpVector);
    }

    public void GenerateMesh()
    {
        Vector3[] verts = new Vector3[currentResolution * currentResolution];
        int[] tris = new int[(currentResolution - 1) * (currentResolution - 1) * 2 * 3];

        Vector3 rootPos = localUpVector;
        Vector2 currPercent = Vector2.zero;
        int triIdx = 0;
        for (int y = 0, i = 0; y < currentResolution; y++)
        {
            for (int x = 0; x < currentResolution; x++, i++)
            {
                currPercent = new Vector2(x, y) / (currentResolution - 1);
                currPercent.x -= 0.5f;
                currPercent.y -= 0.5f;

                //2f weil... das Cube-Face hat immer eine Seitenlänge von 2m
                Vector3 cubeVertPos = rootPos + axisA * 2f * currPercent.x + axisB * 2f * currPercent.y;
                Vector3 sphereVertPos = shapeGenerator.TransformCubeToSpherePos(cubeVertPos, useFancySphere);
                Vector3 planetVertPos = shapeGenerator.CalculatePointOnPlanet(sphereVertPos);
                Vector3 transformedPos = shapeGenerator.TransformPointWithOwnTransformMatrix(planetVertPos);

                verts[i] = transformedPos;

                if (x < currentResolution - 1 && y < currentResolution - 1)
                {
                    tris[triIdx + 0] = i;
                    tris[triIdx + 1] = i + currentResolution + 1;
                    tris[triIdx + 2] = i + 1;

                    tris[triIdx + 3] = i;
                    tris[triIdx + 4] = i + currentResolution;
                    tris[triIdx + 5] = i + currentResolution + 1;

                    triIdx += 6;
                }
            }
        }

        mesh.Clear();
        mesh.vertices = verts;
        mesh.triangles = tris;
        mesh.RecalculateNormals();
    }
}
