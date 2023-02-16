using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyPlanetGenerator : MonoBehaviour
{
    private TerrainFace[] terrainFaces;
    private MeshFilter[] terrainFilters;

    [SerializeField]
    private ShapeSettings shapeSettings;
    public ShapeSettings ShapeSettings => shapeSettings;
    [HideInInspector]
    public bool ShapeSettingsFoldout;

    private ShapeGenerator shapeGenerator = new ShapeGenerator();

    private static Vector3[] DIRECTIONS = new Vector3[]
    {
            Vector3.forward,
            Vector3.back,
            Vector3.right,
            Vector3.left,
            Vector3.up,
            Vector3.down
    };

    [SerializeField]
    private bool mAutoUpdatePlanet;

    [SerializeField]
    private Vector3 mPosition;
    [SerializeField]
    private Vector3 mRotation;
    [SerializeField]
    private Vector3 mScale;

    [SerializeField]
    private Material mMeshMat;
    [SerializeField, Range(2, 256)]
    private int mResolution;

    public void GeneratePlanet()
    {
        Initialize();
        GenerateMesh();
    }

    private void Initialize()
    {
        shapeGenerator = new ShapeGenerator();
        shapeGenerator.UpdateShapeSettings(ShapeSettings, mPosition, mRotation, mScale);

        terrainFaces = new TerrainFace[6];      //6 weil... ein Würfel hat 6 Seiten

        if (terrainFilters == null || terrainFilters.Length != 6)
            terrainFilters = new MeshFilter[6]; //6 weil... ein Würfel hat 6 Seiten

        GameObject newFaceObj;
        for (int i = 0; i < terrainFaces.Length; i++)
        {
            if (terrainFilters[i] == null)
            {
                newFaceObj = new GameObject($"TerrainFace_{i}");
                newFaceObj.transform.SetParent(this.transform);

                MeshRenderer newRenderer = newFaceObj.AddComponent<MeshRenderer>();
                newRenderer.sharedMaterial = mMeshMat;
                terrainFilters[i] = newFaceObj.AddComponent<MeshFilter>();

                Mesh newFaceMesh = new Mesh();
                newFaceMesh.name = $"TerrainFace_{i}";

                terrainFilters[i].sharedMesh = newFaceMesh;
            }

            terrainFaces[i] = new TerrainFace(terrainFilters[i].sharedMesh, shapeGenerator, mResolution, DIRECTIONS[i], shapeSettings.UseFancySphere);
        }
    }

    private void GenerateMesh()
    {
        for (int i = 0; i < terrainFaces.Length; i++)
        {
            terrainFaces[i].GenerateMesh();
        }
    }

    public void OnBaseInfoUpdate()
    {
        if (mAutoUpdatePlanet)
        {
            GeneratePlanet();
        }
    }

    public void OnShapeSettingsUpdate()
    {
        if (mAutoUpdatePlanet)
        {
            GeneratePlanet();
        }
    }
}
