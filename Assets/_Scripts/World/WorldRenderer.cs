using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* It's a class that renders world chuks */
public class WorldRenderer : MonoBehaviour
{
    public GameObject chunkPrefab;
    public GameObject chunkStorage;
    public Queue<ChunkRenderer> chunkQueue = new();

    /// <summary>
    /// If there's a chunk in the pool, use it, otherwise create a new one
    /// </summary>
    /// <param name="gameData">This is the class that holds all the data for the game.</param>
    /// <param name="position">The position of the chunk</param>
    /// <param name="meshData">A struct that contains a list of vertices, triangles, and uvs.</param>
    /// <returns>
    /// A ChunkRenderer object.
    /// </returns>
    internal ChunkRenderer RenderChunk(GameData gameData, Vector3Int position, MeshData meshData)
    {
        ChunkRenderer chunk;
        if (chunkQueue.Count > 0)
        {
            chunk = chunkQueue.Dequeue();
            chunk.transform.position = position;
        }
        else
        {
            GameObject objectInstance = Instantiate(chunkPrefab, position, Quaternion.identity, chunkStorage.transform);
            objectInstance.layer = 6;
            chunk = objectInstance.GetComponent<ChunkRenderer>();
        }

        chunk.InitializeChunk(gameData.chunkDataDictionary[position]);
        chunk.UpdateChunk(meshData);
        chunk.gameObject.SetActive(true);
        return chunk;
    }

    /// <summary>
    /// > Takes a chunk and deactivates it.
    /// </summary>
    /// <param name="chunk">The chunk renderer that is being removed.</param>
    public void DeleteChunk(ChunkRenderer chunk)
    {
        chunk.gameObject.SetActive(false);
        chunkQueue.Enqueue(chunk);
    }
}