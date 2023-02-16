using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkManager : MonoBehaviour
{
    /// <summary>Reference to the generator</summary>
    [SerializeField] private Generator generator;

    /// <summary>Reference to the viewer transform which is used to determine the active chunks</summary>
    [SerializeField] private Transform viewerTransform;

    /// <summary>Determines how many chunks are visible</summary>
    [SerializeField] private int renderDistance;

    /// <summary>Sets how big a chunk is</summary>
    [SerializeField] private float chunkSize;

    /// <summary>The resolution determines how detailed a chunk mesh will be. The higher the value, the more detailed a chunk is</summary>
    [SerializeField] private int resolution;

    /// <summary>Dictionary of all the generated chunks</summary>
    private Dictionary<Vector2Int, Chunk> allChunkDic;

    /// <summary>This list keeps track of all active and visible chunks</summary>
    private List<Vector2Int> activeChunks;

    /// <summary>The position of the viewer transform</summary>
    private Vector2 ViewerPos => new (viewerTransform.position.x, viewerTransform.position.z);

    /// <summary>Updates the visible chunks each frame</summary>
    private void UpdateChunks()
    {
        List<Vector2Int> newActiveChunks = new();

        Vector2 currViewerPos = ViewerPos / chunkSize;
        Vector2Int currentViewerChunkCoord = new(
            Mathf.RoundToInt(currViewerPos.x),
            Mathf.RoundToInt(currViewerPos.y));

        for (int y = -renderDistance; y <= renderDistance; y++)
        {
            for (int x = -renderDistance; x <= renderDistance; x++)
            {
                Vector2Int currChunkCoord = currentViewerChunkCoord + new Vector2Int(x, y);

                if (activeChunks.Contains(currChunkCoord))
                {
                    /*Wir wissen:
                     *- er ist an
                     *- er ist generiert
                     */
                    activeChunks.Remove(currChunkCoord);
                }
                else
                {
                    if (!allChunkDic.ContainsKey(currChunkCoord))
                    {
                        Vector3 currChunkWorldPos = new(currChunkCoord.x * chunkSize, 0, currChunkCoord.y * chunkSize);
                        GameObject newChunkObj = generator.GenerateNoiseMeshObject(currChunkWorldPos, resolution, chunkSize, transform);
                        allChunkDic.Add(currChunkCoord, new Chunk(newChunkObj));
                    }
                    else allChunkDic[currChunkCoord].SetVisibility(true);
                }
                newActiveChunks.Add(currChunkCoord);
            }
        }
        foreach (Vector2Int chunkInx in activeChunks)
        {
            allChunkDic[chunkInx].SetVisibility(false);
        }
        activeChunks = newActiveChunks;
    }
    
    /// <summary>Private Chunk class that holds necessary chunk data</summary>
    private class Chunk
    {
        /// <summary>The Chunk GameObject</summary>
        GameObject chunkObj;

        /// <summary>Position of the chunk</summary>
        public Vector3 position;
        private Vector3 Position => position;

        /// <summary>Constructor of this Class</summary>
        /// <param name="chunkObj">The Chunk GameObject</param>
        public Chunk(GameObject chunkObj)
        {
            this.chunkObj = chunkObj;
            position = chunkObj.transform.position;
        }

        /// <summary>Makes a chunk visible or invisible</summary>
        /// <param name="isVisible">bool that sets this chunk's visibility</param>
        public void SetVisibility(bool isVisible)
        {
            chunkObj.SetActive(isVisible);
        }
    }

    private void Awake()
    {
        allChunkDic = new Dictionary<Vector2Int, Chunk>();
        activeChunks = new List<Vector2Int>();

        if (GeneratorSettingsSingleton.Instance.GeneratorSettings != null)
        {
            chunkSize = GeneratorSettingsSingleton.Instance.GeneratorSettings.ChunkSize;
            resolution = GeneratorSettingsSingleton.Instance.GeneratorSettings.ChunkResolution;
        }
    }

    private void Update()
    {
        UpdateChunks();
    }

    /// <summary>Draws outlines of all generated chunks in the editor</summary>
    private void OnDrawGizmos()
    {
        if (allChunkDic != null)
        {
            Gizmos.color = Color.green;
            foreach (KeyValuePair<Vector2Int, Chunk> entry in allChunkDic)
            {
                Chunk c = entry.Value;
                Gizmos.DrawWireCube(c.position, new Vector3(chunkSize, 0, chunkSize));
            }
        } 
    }
}
