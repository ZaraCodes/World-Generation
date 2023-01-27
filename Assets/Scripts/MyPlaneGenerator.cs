using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter)), RequireComponent(typeof(MeshRenderer))]
public class MyPlaneGenerator : MonoBehaviour
{
    [Header("Mesh Setting")]
    private Mesh mesh;
    private MeshFilter filter;
    private MeshRenderer meshRenderer;

    [SerializeField]
    private bool mGenerateModularPlane;

    [SerializeField]
    private Material mMeshMat;
    [SerializeField, Range(2, 256)]
    private int mResolution;
    [SerializeField]
    private float mMeshScale;
    [SerializeField]
    private Vector3 mMeshRoot;

    [Header("Noise Setting")]
    [SerializeField]
    private float mNoiseStrength;
    [SerializeField]
    private float mNoiseScale;
    [SerializeField]
    private Vector3 mBaseNoiseOffset;


    private void Awake()
    {
        filter = GetComponent<MeshFilter>();
        if (filter == null)
            filter = this.gameObject.AddComponent<MeshFilter>();

        meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer == null)
            meshRenderer = this.gameObject.AddComponent<MeshRenderer>();

        mesh = new Mesh();
        mesh.name = "My Plane";
        filter.sharedMesh = mesh;

        meshRenderer.sharedMaterial = mMeshMat;
    }

    private void Update()
    {
        if (mGenerateModularPlane)
            GeneratePlane(mResolution);
        else
            GeneratePrimitiveMesh();
    }

    private void GeneratePrimitiveMesh()
    {
        Vector3[] verts = new Vector3[]
        {
            Vector3.right,  //0
            Vector3.back,   //1
            Vector3.left,   //2
            Vector3.forward //3
        };

        int[] tris = new int[]
        {
            0,
            1,
            2,

            0,
            2,
            3
        };

        mesh.Clear();
        mesh.vertices = verts;
        mesh.triangles = tris;
        mesh.RecalculateNormals();
    }

    private void GeneratePlane(int _resolution)
    {
        Vector3[] verts = new Vector3[_resolution * _resolution];
        int[] tris = new int[(_resolution - 1) * (_resolution - 1) * 2 * 3];

        Vector3 rootPos = mMeshRoot;
        Vector2 currPercent = Vector2.zero;
        int triIdx = 0;
        for (int y = 0, i = 0; y < _resolution; y++)
        {
            for (int x = 0; x < _resolution; x++, i++)
            {
                currPercent = new Vector2(x, y) / (_resolution - 1);
                currPercent.x -= 0.5f;
                currPercent.y -= 0.5f;

                Vector3 baseVertPos = rootPos + Vector3.right * mMeshScale * currPercent.x + Vector3.forward * mMeshScale * currPercent.y;
                Vector3 noisePos = baseVertPos * mNoiseScale + mBaseNoiseOffset;

                baseVertPos += Vector3.up * mNoiseStrength * Mathf.PerlinNoise(noisePos.x, noisePos.z);

                verts[i] = baseVertPos;

                if (x < _resolution - 1 && y < _resolution - 1)
                {
                    tris[triIdx + 0] = i;
                    tris[triIdx + 1] = i + _resolution + 1;
                    tris[triIdx + 2] = i + 1;

                    tris[triIdx + 3] = i;
                    tris[triIdx + 4] = i + _resolution;
                    tris[triIdx + 5] = i + _resolution + 1;

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
