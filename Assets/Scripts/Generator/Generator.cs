using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

public class Generator : MonoBehaviour
{
    /// <summary>Bool that decides if chunks get generated in a threaded way or not</summary>
    [SerializeField] private bool threaded;

    /// <summary>How many iterations of the outer for loop for generating a mesh will get calculated inside a thread instead</summary>
    [SerializeField] private int threadIterations;

    /// <summary>Reference to the water prefab</summary>
    [SerializeField] private GameObject waterPrefab;

    /// <summary>Reference to the tree prefab</summary>
    [SerializeField] private GameObject treePrefab;

    /// <summary>Reference to the material that gets applied to the mesh</summary>
    [SerializeField] private Material mDefaultMaterial;

    /// <summary>The strength that the resulting noise gets multiplied by</summary>
    [SerializeField] private float mNoiseStrength;

    /// <summary>The frequency of the noise</summary>
    [SerializeField] private float frequency;

    /// <summary>How often a noise gets layered to get a more varied result</summary>
    [SerializeField] private int octaves;

    /// <summary>Factor that scales each octave by this value down</summary>
    [SerializeField] private float lacunarity;

    /// <summary>Factor that reduces each octaves influence on the resulting noise</summary>
    [SerializeField] private float persistence;

    /// <summary>
    /// The noise for forests returns a value between 0 and 1. This value therefore has to be between these two values and can 
    /// make a tree appear if the calculated value is bigger than this threshhold.
    /// </summary>
    [SerializeField] private float forestThreshhold;

    /// <summary>Frequency of the forest noise</summary>
    [SerializeField] private float forestFrequency;

    /// <summary>The hilliness noise decides how much of the base noise gets applied. The hilliness frequency decides how big hilly and flat areas are.</summary>
    [SerializeField] private float hillinessFrequency;

    /// <summary>Frequency of the base height noise</summary>
    [SerializeField] private float baseHeightFequency;

    /// <summary>Sets how high the base height noise will be</summary>
    [SerializeField] private float baseHeightMultiplier;

    /// <summary>Base height that all other noise gets added to</summary>
    [SerializeField] private float baseHeight;

    /// <summary>Noise for the main terrain</summary>
    private SimplexNoise mainTerrainNoise;
    
    /// <summary>Noise for the base height</summary>
    private SimplexNoise baseHeightNoise;

    /// <summary>Noise for the hilliness</summary>
    private SimplexNoise hillinessNoise;

    /// <summary>Noise for forests</summary>
    private SimplexNoise woodinessNoise;

    /// <summary>Generated mesh</summary>
    private Mesh mesh;


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

    /// <summary>Initializes the generator</summary>
    private void SetGeneratorSettings()
    {
        mNoiseStrength = GeneratorSettingsSingleton.Instance.GeneratorSettings.NoiseStrength;
        frequency = GeneratorSettingsSingleton.Instance.GeneratorSettings.Frequency;
        octaves = GeneratorSettingsSingleton.Instance.GeneratorSettings.Octaves;
        lacunarity = GeneratorSettingsSingleton.Instance.GeneratorSettings.Lacunarity;
        persistence = GeneratorSettingsSingleton.Instance.GeneratorSettings.Persistence;
        forestFrequency = GeneratorSettingsSingleton.Instance.GeneratorSettings.ForestFrequency;
        forestThreshhold = GeneratorSettingsSingleton.Instance.GeneratorSettings.ForestThreshhold;
        hillinessFrequency = GeneratorSettingsSingleton.Instance.GeneratorSettings.HillinessFrequency;
        baseHeightFequency = GeneratorSettingsSingleton.Instance.GeneratorSettings.BaseHeightFrequency;
        baseHeightMultiplier = GeneratorSettingsSingleton.Instance.GeneratorSettings.BaseHeightMultiplier;
        baseHeight = GeneratorSettingsSingleton.Instance.GeneratorSettings.BaseHeight;

    }

    /// <summary>Sets the seeds for the different noises</summary>
    /// <param name="seed">the seed duh</param>
    private void SetSeed(int seed)
    {
        GeneratorSettingsSingleton.Instance.seed = seed;
        mainTerrainNoise = new SimplexNoise(seed);
        baseHeightNoise = new SimplexNoise(seed++);
        hillinessNoise = new SimplexNoise(seed++);
        woodinessNoise = new SimplexNoise(seed++);
    }

    /// <summary>Generates a chunk</summary>
    /// <param name="rootPos">Root position of the chunk</param>
    /// <param name="resolution">Resolution of the chunk</param>
    /// <param name="chunkSize">Size of the chunk</param>
    /// <param name="parent">The object this chunk will get attached to</param>
    /// <returns>The generated chunk</returns>
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
        waterPlane.transform.position = new(generatedTile.transform.position.x, GeneratorSettingsSingleton.Instance.GeneratorSettings.WaterHeight, generatedTile.transform.position.z);
        waterPlane.transform.localScale = new(chunkSize, chunkSize, chunkSize);

        PlaceTrees(generatedTile, chunkSize);

        return generatedTile;
    }

    /// <summary>Places trees on the chunk</summary>
    /// <param name="generatedTile">The chunk</param>
    /// <param name="chunkSize">The size of the chunk</param>
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
    
    /// <summary>Generates the chunk mesh</summary>
    /// <param name="rootPos">Root position of the chunk</param>
    /// <param name="resolution">Resolution of the chunk</param>
    /// <param name="size">Size of the chunk</param>
    private void GenerateMesh(Vector3 rootPos, int resolution, float size)
    {
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

    /// <summary>Starts a thread for threaded chunk generation</summary>
    /// <param name="rootPos">Root position of the chunk</param>
    /// <param name="resolution">Resolution of the chunk</param>
    /// <param name="size">Size of the chunk</param>
    /// <param name="verts">Array of all verts for this chunk</param>
    /// <param name="tris">Array of all tris for this chunk</param>
    /// <param name="cornerPos">Corner position of the chunk</param>
    /// <param name="y">y parameter of the outer for loop</param>
    /// <param name="vertIdx">vertIdx from the outer for loop</param>
    /// <param name="triIdx">triIdx from the outer for loop</param>
    /// <returns>A task</returns>
    private Task StartInnerForThread(Vector3 rootPos, int resolution, float size, Vector3[] verts, int[] tris, Vector3 cornerPos, int y, int vertIdx, int triIdx)
    {
        return Task.Run(() => InnerForLoopThreaded(rootPos, resolution, size, verts, tris, cornerPos, y, vertIdx, triIdx));
    }

    /// <summary>generates a chunk in a non threaded way</summary>
    /// <param name="rootPos">Root position of the chunk</param>
    /// <param name="resolution">Resolution of the chunk</param>
    /// <param name="size">Size of the chunk</param>
    /// <param name="verts">Array of all verts for this chunk</param>
    /// <param name="tris">Array of all tris for this chunk</param>
    /// <param name="cornerPos">Corner position of the chunk</param>
    /// <param name="y">y parameter of the outer for loop</param>
    /// <param name="vertIdx">vertIdx from the outer for loop</param>
    /// <param name="triIdx">triIdx from the outer for loop</param>
    private void InnerForLoopNormal(Vector3 rootPos, int resolution, float size, Vector3[] verts, int[] tris, Vector3 cornerPos, int y, int vertIdx, int triIdx)
    {
        for (int x = 0; x <= resolution; x++, vertIdx++)
        {
            Vector3 vertPosLocal = cornerPos + (new Vector3(x, 0, y) / resolution) * size;
            Vector3 vertPosWorld = rootPos + vertPosLocal;

            vertPosLocal.y = EvaluateCoordinateHeight(vertPosWorld);

            if (vertIdx < verts.Length) verts[vertIdx] = vertPosLocal;

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

    /// <summary>Generates a chunk in a threded way</summary>
    /// <param name="rootPos">Root position of the chunk</param>
    /// <param name="resolution">Resolution of the chunk</param>
    /// <param name="size">Size of the chunk</param>
    /// <param name="verts">Array of all verts for this chunk</param>
    /// <param name="tris">Array of all tris for this chunk</param>
    /// <param name="cornerPos">Corner position of the chunk</param>
    /// <param name="y">y parameter of the outer for loop</param>
    /// <param name="vertIdx">vertIdx from the outer for loop</param>
    /// <param name="triIdx">triIdx from the outer for loop</param>
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

    /// <summary>Calculates the height of the terrain at a certain point</summary>
    /// <param name="position">Position in the world</param>
    /// <returns>The height of the terrain</returns>
    public float EvaluateCoordinateHeight(Vector3 position)
    {
        position /= 13;

        float noise = 0;
        float divisor = 0;
        for (int i = 0; i < octaves; i++)
        {
            noise += mainTerrainNoise.Evaluate(new(position.x * Mathf.Pow(lacunarity, i) * frequency, 0f, position.z * Mathf.Pow(lacunarity, i) * frequency)) * Mathf.Pow(persistence, i);
            divisor += Mathf.Pow(persistence, i);
        }

        noise /= divisor;
        float worldHeight = baseHeight + baseHeightNoise.Evaluate(new(position.x * frequency * baseHeightFequency, 0f, position.z * frequency * baseHeightFequency)) * baseHeightMultiplier;
        worldHeight += noise * mNoiseStrength * ((hillinessNoise.Evaluate(new(position.z * frequency * hillinessFrequency, 0f, position.x * frequency * hillinessFrequency)) + 1) / 2);

        return worldHeight;
    }
}
