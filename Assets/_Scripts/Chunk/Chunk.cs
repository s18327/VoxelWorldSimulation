using UnityEngine;
using System;
using Newtonsoft.Json;

/* It's a class that holds the data of a chunk */
[Serializable]
public class Chunk
{

    public VoxelType[] chunkVoxelStorage;
    public int chunkSize;
    public int chunkHeight;
    [JsonIgnore]
    public Terrain terrainReference;
    public Vector3Int terrainPosition;
    public bool isPlayerModified;
    public TreeData treeData;

    public Chunk(int chunkSize, int chunkHeight, Terrain terrain, Vector3Int terrainPosition)
    {
        this.chunkHeight = chunkHeight;
        this.chunkSize = chunkSize;
        terrainReference = terrain;
        this.terrainPosition = terrainPosition;
        chunkVoxelStorage = new VoxelType[chunkSize * chunkHeight * chunkSize];
    }

}