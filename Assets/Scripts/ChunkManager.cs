using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class ChunkManager : MonoBehaviour
{
    [SerializeField] private Generator mGenerator;
    [SerializeField] private Transform mViewer;

    [SerializeField] private int mRenderDistance;
    [SerializeField] private float mChunkSize;
    [SerializeField] private int mResolution;

    private Dictionary<Vector2Int, Chunk> allChunkDic;
    private List<Vector2Int> activeChunks;


    private Vector2 ViewerPos => new (mViewer.position.x, mViewer.position.z);

    private void UpdateChunks()
    {
        List<Vector2Int> newActiveChunks = new();

        Vector2 currViewerPos = ViewerPos / mChunkSize;
        Vector2Int currentViewerChunkCoord = new(
            Mathf.RoundToInt(currViewerPos.x),
            Mathf.RoundToInt(currViewerPos.y));

        for (int y = -mRenderDistance; y <= mRenderDistance; y++)
        {
            for (int x = -mRenderDistance; x <= mRenderDistance; x++)
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
                        Vector3 currChunkWorldPos = new(currChunkCoord.x * mChunkSize, 0, currChunkCoord.y * mChunkSize);

                        // await Task.Run(() => mGenerator.GenerateNoiseMeshObject(currChunkWorldPos, mResolution, mChunkSize, transform, allChunkDic, currChunkCoord));

                        // await mGenerator.GenerateNoiseMeshObject(currChunkWorldPos, mResolution, mChunkSize, transform, allChunkDic, currChunkCoord);
                        GameObject newChunkObj = mGenerator.GenerateNoiseMeshObject(currChunkWorldPos, mResolution, mChunkSize, transform);

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

    public class Chunk
    {
        GameObject chunkObj;
        public Vector3 position;

        private Vector3 Position => position;

        public Chunk(GameObject chunkObj)
        {
            this.chunkObj = chunkObj;
            position = chunkObj.transform.position;
        }

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
            mChunkSize = GeneratorSettingsSingleton.Instance.GeneratorSettings.ChunkSize;
            mResolution = GeneratorSettingsSingleton.Instance.GeneratorSettings.ChunkResolution;
        }
    }

    private void Update()
    {
        UpdateChunks();
    }

    private void OnDrawGizmos()
    {
        if (allChunkDic != null)
        {
            Gizmos.color = Color.green;
            foreach (KeyValuePair<Vector2Int, Chunk> entry in allChunkDic)
            {
                Chunk c = entry.Value;
                Gizmos.DrawWireCube(c.position, new Vector3(mChunkSize, 0, mChunkSize));
            }
        } 
    }
}
