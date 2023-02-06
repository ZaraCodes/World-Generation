using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class Generator : MonoBehaviour
{
    [SerializeField] private GameObject waterPrefab;
    [SerializeField] private GameObject treePrefab;
    [SerializeField] private Material mDefaultMaterial;
    [SerializeField] private float mNoiseStrength;
    [SerializeField] private float frequency;

    [SerializeField] private float octaves;
    [SerializeField] private float lacunarity;
    [SerializeField] private float persistence;

    private SimplexNoise mainTerrainNoise;
    private SimplexNoise baseHeightNoise;
    private SimplexNoise hillinessNoise;
    private SimplexNoise woodinessNoise;

    private Mesh mesh;

    private void Awake()
    {
        int seed = Random.Range(int.MinValue, int.MaxValue);
        mainTerrainNoise = new SimplexNoise(seed);
        baseHeightNoise = new SimplexNoise(seed++);
        hillinessNoise = new SimplexNoise(seed++);
        woodinessNoise = new SimplexNoise(seed++);
    }

    public GameObject GenerateNoiseMeshObject(Vector3 rootPos, int resolution, float chunkSize, Transform parent)
    {
        GameObject generatedTile = new();
        generatedTile.transform.position = rootPos;
        generatedTile.transform.SetParent(parent);
        generatedTile.name = $"Chunk_{rootPos.x}/{rootPos.z}";

        MeshRenderer tileRenderer = generatedTile.AddComponent<MeshRenderer>();
        MeshFilter tileFilter = generatedTile.AddComponent<MeshFilter>();
        MeshCollider tileCollider = generatedTile.AddComponent<MeshCollider>();
        mesh = new();
        mesh.name = $"ChunkMesh_{rootPos.x}/{rootPos.z}";

        tileRenderer.material = mDefaultMaterial;
        tileFilter.sharedMesh = mesh;

        GenerateMesh(rootPos, resolution, chunkSize);
        tileCollider.sharedMesh = mesh;

        GameObject waterPlane = Instantiate(waterPrefab);
        waterPlane.transform.parent = generatedTile.transform;
        waterPlane.transform.position = new Vector3(generatedTile.transform.position.x, 2.5f, generatedTile.transform.position.z);
        waterPlane.transform.localScale = new(chunkSize, chunkSize, chunkSize);

        PlaceTrees(generatedTile, chunkSize);

        return generatedTile;
    }

    private void PlaceTrees(GameObject generatedTile, float chunkSize)
    {
        Vector3 cornerPos = new Vector3(-chunkSize / 2f, 0f, -chunkSize / 2f);
        for (int x = 0; x < chunkSize; x += 4)
        {
            for (int z = 0; z < chunkSize; z += 4)
            {
                Vector3 vertPosLocal = cornerPos + new Vector3(x, 0, z);
                Vector3 vertPosWorld = generatedTile.transform.position + vertPosLocal;

                float noiseResult = (woodinessNoise.Evaluate(new Vector3(vertPosWorld.x, 0, vertPosWorld.z) * 0.004f) + 1) / 2;

                if (noiseResult > 0.6f && Random.Range(0.6f, 1f) <= noiseResult)
                {
                    Vector3 treePos = new(vertPosWorld.x, EvaluateCoordinateHeight(new(vertPosWorld.x, 0f, vertPosWorld.z)), vertPosWorld.z);

                    if (treePos.y > 6 && treePos.y < 40)
                    {
                        GameObject tree = Instantiate(treePrefab);
                        tree.transform.parent = generatedTile.transform;
                        tree.transform.position = treePos;
                        tree.transform.Rotate(new(0, Random.Range(0f, 360f)));
                    }
                }
            }
        }
    }
    
    private void GenerateMesh(Vector3 rootPos, int resolution, float size)
    {
        Vector3[] verts = new Vector3[(resolution + 1) * (resolution + 1)];
        int[] tris = new int[resolution * resolution * 2 * 3];

        Vector3 cornerPos = new Vector3(-size / 2f, 0f, -size / 2f);

        for (int y = 0, vertIdx = 0, triIdx = 0; y <= resolution; y++)
        {
            for (int x = 0; x <= resolution; x++, vertIdx++)
            {
                Vector3 vertPosLocal = cornerPos + (new Vector3(x, 0, y) / resolution) * size;
                Vector3 vertPosWorld = rootPos + vertPosLocal;

                vertPosLocal.y = EvaluateCoordinateHeight(vertPosWorld);

                verts[vertIdx] = vertPosLocal;

                if (x < resolution && y < resolution)
                {
                    tris[triIdx + 0] = vertIdx;
                    tris[triIdx + 1] = vertIdx + (resolution + 1) + 1;
                    tris[triIdx + 2] = vertIdx + 1;

                    tris[triIdx + 3] = vertIdx;
                    tris[triIdx + 4] = vertIdx + (resolution + 1);
                    tris[triIdx + 5] = vertIdx + (resolution + 1) + 1;

                    triIdx += 6;
                }
            }
        }

        mesh.Clear();
        mesh.vertices = verts;
        mesh.triangles = tris;
        mesh.RecalculateNormals();
    }

    public float EvaluateCoordinateHeight(Vector3 vertPosWorld)
    {
        vertPosWorld /= 13;

        float noise = 0;
        float divisor = 0;
        for (int i = 0; i < octaves; i++)
        {
            noise += mainTerrainNoise.Evaluate(new(vertPosWorld.x * Mathf.Pow(lacunarity, i) * frequency, 0f, vertPosWorld.z * Mathf.Pow(lacunarity, i) * frequency)) * Mathf.Pow(persistence, i);
            divisor += Mathf.Pow(persistence, i);
        }
        // Mathf.PerlinNoise(vertPosWorld.x * frequency, vertPosWorld.z * frequency);
        noise /= divisor;
        float worldHeight = baseHeightNoise.Evaluate(new(vertPosWorld.x * 0.01f, 0f, vertPosWorld.z * 0.01f)) * 25f + noise * mNoiseStrength * hillinessNoise.Evaluate(new(vertPosWorld.z * 0.02f, 0f, vertPosWorld.x * 0.02f));

        worldHeight += 10f;
        return worldHeight;
    }
}
