using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;



/* It's a class that holds a dictionary of chunk data, a dictionary of chunk renderers, a json string
of the chunk data dictionary, the chunk size, and the chunk height */
[Serializable]
public class TerrainData
{
    public Dictionary<Vector3Int, Chunk> chunkDictionary;
    public Dictionary<Vector3Int, ChunkRenderer> chunkRendererDictionary;
    public string jsonChunkDataDictionary;
}


/* It's a class that holds all the data that is needed to create a terrain */
[Serializable]
public class TerrainParameters
{
    public int chunkSize;
    public int chunkHeight;
    public int chunkDrawRange;
    public Vector2Int mapSeedOffset;

    public TerrainParameters()
    {
        chunkSize = 1;
        chunkHeight = 1;
        chunkDrawRange = 1;
        mapSeedOffset = Vector2Int.zero;
    }
}


/* This class is used to store the player's position and rotation */
[Serializable]
public class PlayerData
{
    public Vector3 playerPosition;
    public Vector3 spawnPos;
    public Quaternion playerRotation;

    public PlayerData()
    {
        spawnPos = new Vector3(15, 15, 15);
        playerPosition = spawnPos;
        playerRotation = Quaternion.identity;
    }
}


/* It's a class that holds data about a texture */
[Serializable]
public class VoxelData
{
    public VoxelType voxelType;
    public Vector2Int up, down, side;
    public bool isSolid = true;
    public bool isGeneratingCollider = true;
    public int durability = 1;
    public bool isPlaceable = true;
}


/* It's a struct that holds a list of chunk positions to create, a list of chunk data positions to
create, a list of chunk positions to remove, a list of chunk data to remove, and a list of chunk
positions to update. */
[Serializable]
public struct TerrainGenerationData
{
    public List<Vector3Int> chunkPositionsToCreate;
    public List<Vector3Int> chunkDataPositionsToCreate;
    public List<Vector3Int> chunkPositionsToRemove;
    public List<Vector3Int> chunkDataToRemove;
}
