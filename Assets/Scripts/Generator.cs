using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

public class Generator : MonoBehaviour
{
    [SerializeField] private bool threaded;
    [SerializeField] private int threadIterations;

    [SerializeField] private GameObject waterPrefab;
    [SerializeField] private GameObject treePrefab;
    [SerializeField] private Material mDefaultMaterial;
    [SerializeField] private float mNoiseStrength;
    [SerializeField] private float frequency;

    [SerializeField] private int octaves;
    [SerializeField] private float lacunarity;
    [SerializeField] private float persistence;

    [SerializeField] private float forestThreshhold;
    [SerializeField] private float forestFrequency;

    [SerializeField] private float hillinessFrequency;
    [SerializeField] private float baseHeightFequency;
    [SerializeField] private float baseHeightMultiplier;
    [SerializeField] private float baseHeight;

    private SimplexNoise mainTerrainNoise;
    private SimplexNoise baseHeightNoise;
    private SimplexNoise hillinessNoise;
    private SimplexNoise woodinessNoise;

    private Mesh mesh;

    private float minHeight;

    private void Awake()
    {
        if (GeneratorSettingsSingleton.Instance.GeneratorSettings != null)
        {
            if (GeneratorSettingsSingleton.Instance.useCustomSeed)
                SetSeed(GeneratorSettingsSingleton.Instance.seed);
            else
                SetSeed(Random.Range(int.MinValue, int.MaxValue));
            SetGeneratorSettings();
        }
        else SetSeed(Random.Range(int.MinValue, int.MaxValue));

    }

    private void SetGeneratorSettings()
    {
        mNoiseStrength = GeneratorSettingsSingleton.Instance.GeneratorSettings.NoiseStrength;
        frequency = GeneratorSettingsSingleton.Instance.GeneratorSettings.Frequency;
        octaves = GeneratorSettingsSingleton.Instance.GeneratorSettings.Octaves;
        lacunarity = GeneratorSettingsSingleton.Instance.GeneratorSettings.Lacunarity;
        persistence = GeneratorSettingsSingleton.Instance.GeneratorSettings.Persistence;
        forestFrequency = GeneratorSettingsSingleton.Instance.GeneratorSettings.ForestFrequency;
        forestThreshhold= GeneratorSettingsSingleton.Instance.GeneratorSettings.ForestThreshhold;
        hillinessFrequency = GeneratorSettingsSingleton.Instance.GeneratorSettings.HillinessFrequency;
        baseHeightFequency = GeneratorSettingsSingleton.Instance.GeneratorSettings.BaseHeightFrequency;
        baseHeightMultiplier = GeneratorSettingsSingleton.Instance.GeneratorSettings.BaseHeightMultiplier;
        baseHeight = GeneratorSettingsSingleton.Instance.GeneratorSettings.BaseHeight;

    }

    private void SetSeed(int seed)
    {
        Debug.Log($"Seed: {seed}");
        GeneratorSettingsSingleton.Instance.seed = seed;
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

        /* if (minHeight < 2.5f)
        { */
            GameObject waterPlane = Instantiate(waterPrefab);
            waterPlane.transform.parent = generatedTile.transform;
            waterPlane.transform.position = new(generatedTile.transform.position.x, GeneratorSettingsSingleton.Instance.GeneratorSettings.WaterHeight, generatedTile.transform.position.z);
            waterPlane.transform.localScale = new(chunkSize, chunkSize, chunkSize);
        /*}*/

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

                float noiseResult = (woodinessNoise.Evaluate(new Vector3(vertPosWorld.x, 0, vertPosWorld.z) * forestFrequency) + 1) / 2;

                if (noiseResult > forestThreshhold && Random.Range(forestThreshhold, 1f) <= noiseResult)
                {
                    Vector3 treePos = new(Random.Range(vertPosWorld.x - 1.3f, vertPosWorld.x + 1.3f), 0f, Random.Range(vertPosWorld.z - 1.3f, vertPosWorld.z + 1.3f));
                    treePos.y = EvaluateCoordinateHeight(treePos);

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
        minHeight = 10000f;
        Vector3[] verts = new Vector3[(resolution + 1) * (resolution + 1)];
        int[] tris = new int[resolution * resolution * 2 * 3];

        Vector3 cornerPos = new(-size / 2f, 0f, -size / 2f);

        if (threaded)
        {
            List<Task> tasks = new();
            for (int y = 0, vertIdx = 0, triIdx = 0; y <= resolution; y += threadIterations, vertIdx += (resolution + 1) * threadIterations, triIdx += resolution * 6 * threadIterations)
            {
                tasks.Add(StartInnerForThread(rootPos, resolution, size, verts, tris, cornerPos, y, vertIdx, triIdx));
            }
            Task.WaitAll(tasks.ToArray());
        }
        else
        {
            for (int y = 0, vertIdx = 0, triIdx = 0; y <= resolution; y++, vertIdx += resolution + 1, triIdx += resolution * 6)
            {
                InnerForLoopNormal(rootPos, resolution, size, verts, tris, cornerPos, y, vertIdx, triIdx);
            }
        }

        mesh.Clear();
        mesh.vertices = verts;
        mesh.triangles = tris;
        mesh.RecalculateNormals();
    }

    private Task StartInnerForThread(Vector3 rootPos, int resolution, float size, Vector3[] verts, int[] tris, Vector3 cornerPos, int y, int vertIdx, int triIdx)
    {
        return Task.Run(() => InnerForLoopThreaded(rootPos, resolution, size, verts, tris, cornerPos, y, vertIdx, triIdx));
    }

    private void InnerForLoopNormal(Vector3 rootPos, int resolution, float size, Vector3[] verts, int[] tris, Vector3 cornerPos, int y, int vertIdx, int triIdx)
    {
        for (int x = 0; x <= resolution; x++, vertIdx++)
        {
            Vector3 vertPosLocal = cornerPos + (new Vector3(x, 0, y) / resolution) * size;
            Vector3 vertPosWorld = rootPos + vertPosLocal;

            vertPosLocal.y = EvaluateCoordinateHeight(vertPosWorld);
            // if (vertPosLocal.y < minHeight) minHeight = vertPosLocal.y;

            if (vertIdx < verts.Length) verts[vertIdx] = vertPosLocal;
            // else Debug.Log($"vIdx: {vertIdx} x: {x} y: {y}");

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

    private void InnerForLoopThreaded(Vector3 rootPos, int resolution, float size, Vector3[] verts, int[] tris, Vector3 cornerPos, int y, int vertIdx, int triIdx)
    {
        for (int i = 0; i < threadIterations; i++)
        {
            for (int x = 0; x <= resolution; x++, vertIdx++)
            {
                Vector3 vertPosLocal = cornerPos + (new Vector3(x, 0, y) / resolution) * size;
                Vector3 vertPosWorld = rootPos + vertPosLocal;

                vertPosLocal.y = EvaluateCoordinateHeight(vertPosWorld);
                // if (vertPosLocal.y < minHeight) minHeight = vertPosLocal.y;

                verts[vertIdx] = vertPosLocal;
                // else Debug.Log($"vIdx: {vertIdx} x: {x} y: {y}");

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

            y++;
            if (y > resolution) return;
        }
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

        noise /= divisor;
        float worldHeight = baseHeight + baseHeightNoise.Evaluate(new(vertPosWorld.x * frequency * baseHeightFequency, 0f, vertPosWorld.z * frequency * baseHeightFequency)) * baseHeightMultiplier;
        worldHeight += noise * mNoiseStrength * ((hillinessNoise.Evaluate(new(vertPosWorld.z * frequency * hillinessFrequency, 0f, vertPosWorld.x * frequency * hillinessFrequency)) + 1) / 2);

        return worldHeight;
    }
}
