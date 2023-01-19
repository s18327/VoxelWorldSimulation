using UnityEngine;
using System;
using Newtonsoft.Json;
using UnityEngine.Serialization;


/* It's a class that holds the data of a chunk */
[Serializable]
public class ChunkData
{

    public VoxelType[] chunkVoxelStorage;
    public int chunkSize;
    public int chunkHeight;
    [JsonIgnore]
    public World worldReference;
    public Vector3Int worldPosition;
    public bool modifiedByThePlayer;
    public TreeData treeData;

    public ChunkData(int chunkSize, int chunkHeight, World world, Vector3Int worldPosition)
    {
        this.chunkHeight = chunkHeight;
        this.chunkSize = chunkSize;
        worldReference = world;
        this.worldPosition = worldPosition;
        chunkVoxelStorage = new VoxelType[chunkSize * chunkHeight * chunkSize];
    }

}