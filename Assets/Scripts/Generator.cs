using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Generator : MonoBehaviour
{
    [SerializeField] private GameObject mChunkPrefab;
    [SerializeField] private Material mDefaultMaterial;
    [SerializeField] private float mNoiseStrength;
    [SerializeField] private float frequency;

    [SerializeField] private float octaves;
    [SerializeField] private float lacunarity;
    [SerializeField] private float persistence;

    private SimplexNoise mainTerrainNoise;
    private SimplexNoise baseHeightNoise;
    private SimplexNoise hillinessNoise;


    private Mesh mesh;

    private void Awake()
    {
        int seed = Random.Range(int.MinValue, int.MaxValue);
        mainTerrainNoise = new SimplexNoise(seed);
        baseHeightNoise = new SimplexNoise(seed++);
        hillinessNoise = new SimplexNoise(seed++);
    }

    private void Start()
    {

    }

    public GameObject GenerateNewChunk(Vector3 rootPos, Transform parent)
    {
        GameObject newChunkGo = Instantiate(mChunkPrefab, parent);
        newChunkGo.transform.position = rootPos;

        return newChunkGo;
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
        mesh = new Mesh();
        mesh.name = $"ChunkMesh_{rootPos.x}/{rootPos.z}";

        tileRenderer.material = mDefaultMaterial;
        tileFilter.sharedMesh = mesh;

        GenerateMesh(rootPos, resolution, chunkSize);
        tileCollider.sharedMesh = mesh;

        return generatedTile;
    }
    
    private void GenerateMesh(Vector3 rootPos, int resolution, float size)
    {
        Vector3[] verts = new Vector3[(resolution + 1) * (resolution + 1)];
        int[] tris = new int[resolution * resolution * 2 * 3];

        Vector3 cornerPos = new Vector3(-0.5f, 0f, -0.5f);

        for (int y = 0, vertIdx = 0, triIdx = 0; y <= resolution; y++)
        {
            for (int x = 0; x <= resolution; x++, vertIdx++)
            {
                Vector3 vertPosLocal = cornerPos + (new Vector3(x, 0, y) / resolution) * size;
                Vector3 vertPosWorld = rootPos + vertPosLocal;
                vertPosWorld /= 13;

                vertPosLocal.y = EvaluateCoordinate(vertPosWorld);

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

    public float EvaluateCoordinate(Vector3 vertPosWorld)
    {
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
