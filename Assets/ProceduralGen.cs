using UnityEngine;
using System.Collections.Generic;

public class ProceduralGen : MonoBehaviour {

    public const float maxViewDst = 16;

    public static Transform viewer;
    public static Vector2 viewerPosition;

    public static GameObject chunk;

    private static int chunkSize;
    private int chunksVisibleInViewDst;

    private Dictionary<Vector2, TerrainChunk> terrainChunkDictionary = new Dictionary<Vector2, TerrainChunk>();
    private List<TerrainChunk> terrainChunksVisibleLastUpdate = new List<TerrainChunk>();
    
    void Start()
    {
        viewer = transform;
        viewerPosition = new Vector2(viewer.position.x, viewer.position.z);
        
        chunkSize = 16;
        chunksVisibleInViewDst = Mathf.RoundToInt(maxViewDst / chunkSize);
        
        chunk = Resources.Load("chunkGen") as GameObject;
    }
    
    void Update() 
    {
        viewerPosition = new Vector2 (viewer.position.x, viewer.position.z);
        UpdateVisibleChunks ();
    }
        
    void UpdateVisibleChunks() 
    {
        for (int i = 0; i < terrainChunksVisibleLastUpdate.Count; i++) {
            terrainChunksVisibleLastUpdate[i].SetVisible (false);
        }
        terrainChunksVisibleLastUpdate.Clear ();
            
        int currentChunkCoordX = Mathf.RoundToInt (viewerPosition.x / chunkSize);
        int currentChunkCoordY = Mathf.RoundToInt (viewerPosition.y / chunkSize);

        for (int yOffset = -chunksVisibleInViewDst; yOffset <= chunksVisibleInViewDst; yOffset++) {
            for (int xOffset = -chunksVisibleInViewDst; xOffset <= chunksVisibleInViewDst; xOffset++) {
                Vector2 viewedChunkCoord = new Vector2 (currentChunkCoordX + xOffset, currentChunkCoordY + yOffset);

                if (terrainChunkDictionary.ContainsKey (viewedChunkCoord)) {
                    terrainChunkDictionary [viewedChunkCoord].UpdateTerrainChunk ();
                    if (terrainChunkDictionary [viewedChunkCoord].IsVisible ()) {
                        terrainChunksVisibleLastUpdate.Add (terrainChunkDictionary [viewedChunkCoord]);
                    }
                } else {
                    terrainChunkDictionary.Add (viewedChunkCoord, new TerrainChunk (viewedChunkCoord, chunkSize, transform));
                }
            }
        }
    }

    public class TerrainChunk {

        GameObject meshObject;
        Vector2 position;
        Bounds bounds;

        private int nbOfSharks;
        
        public TerrainChunk(Vector2 coord, int size, Transform parent) {
            position = coord * size;
            bounds = new Bounds(position,Vector2.one * size);
            Vector3 positionV3 = new Vector3(position.x, parent.position.y,position.y);

            meshObject = Instantiate(chunk, positionV3, Quaternion.identity);
            meshObject.GetComponent<chunkGeneration>().chunkPosition = positionV3;
            meshObject.GetComponent<chunkGeneration>().Size = size;
            meshObject.transform.position = positionV3;
            meshObject.transform.parent = parent;
        }

        public void UpdateTerrainChunk() {
            float viewerDstFromNearestEdge = Mathf.Sqrt(bounds.SqrDistance (viewerPosition));
            bool visible = viewerDstFromNearestEdge <= maxViewDst;
            SetVisible (visible);
        }

        public void SetVisible(bool visible) {
            meshObject.SetActive (visible);
        }

        public bool IsVisible() {
            return meshObject.activeSelf;
        }
    }
}