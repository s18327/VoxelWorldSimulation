using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/* It's a helper class that contains methods that are used to get and set data in the terrain */
public static class TerrainHelper
{
    /// <summary>
    /// > It takes a terrain and a terrain voxel position and returns the chunk position of the chunk that
    /// contains that voxel
    /// </summary>
    /// <param name="terrain">The terrain object that contains the chunk</param>
    /// <param name="position">The position of the voxel in terrain space.</param>
    /// <returns>
    /// A Vector3Int
    /// </returns>
    public static Vector3Int GetChunkPositionFromCoordinates(Terrain terrain, Vector3Int position)
    {
        return new Vector3Int
        {
            x = Mathf.FloorToInt(position.x / (float)terrain.chunkSize) * terrain.chunkSize,
            y = Mathf.FloorToInt(position.y / (float)terrain.chunkHeight) * terrain.chunkHeight,
            z = Mathf.FloorToInt(position.z / (float)terrain.chunkSize) * terrain.chunkSize
        };
    }

    /// <summary>
    /// It returns a list of chunk positions that are within a certain range of the player
    /// </summary>
    /// <param name="terrain">The terrain object that contains terrain information.</param>
    /// <param name="playerPosition">The position of the player in the terrain.</param>
    /// <returns>
    /// A list of Vector3Ints.
    /// </returns>
    internal static List<Vector3Int> GetChunkPositionsAroundPlayer(Terrain terrain, Vector3Int playerPosition)
    {
        int startX = playerPosition.x - (terrain.chunkDrawingRange) * terrain.chunkSize;
        int startZ = playerPosition.z - (terrain.chunkDrawingRange) * terrain.chunkSize;
        int endX = playerPosition.x + (terrain.chunkDrawingRange) * terrain.chunkSize;
        int endZ = playerPosition.z + (terrain.chunkDrawingRange) * terrain.chunkSize;

        List<Vector3Int> listOfChunkPositionsAroundThePlayer = new List<Vector3Int>();
        for (int x = startX; x <= endX; x += terrain.chunkSize)
        {
            for (int z = startZ; z <= endZ; z += terrain.chunkSize)
            {
                Vector3Int chunkPositionFromVoxelCoordinates =
                    GetChunkPositionFromCoordinates(terrain, new Vector3Int(x, 0, z));
                listOfChunkPositionsAroundThePlayer.Add(chunkPositionFromVoxelCoordinates);
                if (x < playerPosition.x - terrain.chunkSize
                    || x > playerPosition.x + terrain.chunkSize
                    || z < playerPosition.z - terrain.chunkSize
                    || z > playerPosition.z + terrain.chunkSize) continue;

                for (int y = -terrain.chunkHeight;
                     y >= playerPosition.y - terrain.chunkHeight * 2;
                     y -= terrain.chunkHeight)
                {
                    chunkPositionFromVoxelCoordinates =
                        GetChunkPositionFromCoordinates(terrain, new Vector3Int(x, y, z));
                    listOfChunkPositionsAroundThePlayer.Add(chunkPositionFromVoxelCoordinates);
                }
            }
        }

        return listOfChunkPositionsAroundThePlayer;
    }

    /// <summary>
    /// Remove the chunk data at the given position from the terrain's chunk data dictionary
    /// </summary>
    /// <param name="terrain">The terrain that the chunk is in.</param>
    /// <param name="pos">The position of the chunk.</param>
    internal static void RemoveChunkData(Terrain terrain, Vector3Int pos)
    {
        terrain.terrainData.chunkDictionary.Remove(pos);
    }

    /// <summary>
    /// If the chunk exists, remove it from the terrain
    /// </summary>
    /// <param name="terrain">The terrain that the chunk is in.</param>
    /// <param name="pos">The position of the chunk you want to remove.</param>
    internal static void RemoveChunk(Terrain terrain, Vector3Int pos)
    {
        if (!terrain.terrainData.chunkRendererDictionary.TryGetValue(pos, out var chunk)) return;

        terrain.terrainRenderer.DeleteChunk(chunk);
        terrain.terrainData.chunkRendererDictionary.Remove(pos);
    }

    /// <summary>
    /// It returns a list of chunk positions that are within a certain range of the player
    /// </summary>
    /// <param name="terrain">The terrain object that contains the chunk size and chunk height.</param>
    /// <param name="playerPosition">The position of the player in the terrain.</param>
    /// <returns>
    /// A list of Vector3Ints.
    /// </returns>
    internal static List<Vector3Int> GetDataPositionsAroundPlayer(Terrain terrain, Vector3Int playerPosition)
    {
        int startX = playerPosition.x - (terrain.chunkDrawingRange + 1) * terrain.chunkSize;
        int startZ = playerPosition.z - (terrain.chunkDrawingRange + 1) * terrain.chunkSize;
        int endX = playerPosition.x + (terrain.chunkDrawingRange + 1) * terrain.chunkSize;
        int endZ = playerPosition.z + (terrain.chunkDrawingRange + 1) * terrain.chunkSize;

        List<Vector3Int> chunkDataPositionsToCreate = new List<Vector3Int>();
        for (int x = startX; x <= endX; x += terrain.chunkSize)
        {
            for (int z = startZ; z <= endZ; z += terrain.chunkSize)
            {
                Vector3Int chunkPos = GetChunkPositionFromCoordinates(terrain, new Vector3Int(x, 0, z));
                chunkDataPositionsToCreate.Add(chunkPos);
                if (x < playerPosition.x - terrain.chunkSize
                    || x > playerPosition.x + terrain.chunkSize
                    || z < playerPosition.z - terrain.chunkSize
                    || z > playerPosition.z + terrain.chunkSize) continue;

                for (int y = -terrain.chunkHeight;
                     y >= playerPosition.y - terrain.chunkHeight * 2;
                     y -= terrain.chunkHeight)
                {
                    chunkPos = GetChunkPositionFromCoordinates(terrain, new Vector3Int(x, y, z));
                    chunkDataPositionsToCreate.Add(chunkPos);
                }
            }
        }

        return chunkDataPositionsToCreate;
    }

    /// <summary>
    /// If the chunk dictionary contains the key, return the chunk. Otherwise, return null
    /// </summary>
    /// <param name="terrainReference">The terrain that the chunk is in.</param>
    /// <param name="terrainPosition">This is a struct that contains 3 integers. It's used to store the position
    /// of a chunk in the terrain.</param>
    /// <returns>
    /// A ChunkRenderer object.
    /// </returns>
    internal static ChunkRenderer GetChunk(Terrain terrainReference, Vector3Int terrainPosition)
    {
        return terrainReference.terrainData.chunkRendererDictionary.ContainsKey(terrainPosition)
            ? terrainReference.terrainData.chunkRendererDictionary[terrainPosition]
            : null;
    }

    /// <summary>
    /// It takes a position, a voxel type, and a dictionary of positions and integers. It then finds the
    /// durability of the voxel type and adds it to the dictionary
    /// </summary>
    /// <param name="pos">The position of the voxel</param>
    /// <param name="voxelType">The type of voxel you want to get the durability of.</param>
    /// <param name="Durability">A dictionary that stores the durability of each voxel.</param>
    private static void GetDurability(Vector3Int pos, VoxelType voxelType, Dictionary<Vector3Int, int> Durability)
    {
        GameManager gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        foreach (var voxel in gameManager.voxel.voxelDataList)
        {
            if (voxel.voxelType != voxelType) continue;
            if (!Durability.ContainsKey(pos))
            {
                Durability.Add(pos, 0);
                Durability[pos] = voxel.durability;
            }

            break;
        }
    }


    private static void SetDurability(Chunk chunk, Vector3Int localPosition, Vector3Int pos,
        VoxelType voxelType, Dictionary<Vector3Int, int> Durability)
    {
        GetDurability(pos, voxelType, Durability);
        if (Durability.TryGetValue(pos, out var durability))
        {
            durability--;
            if (durability <= 0)
            {
                if (
                    ChunkHelper.GetVoxelFromChunkCoordinates(chunk, localPosition + new Vector3Int(1, 0, 0)) ==
                    VoxelType.Water ||
                    ChunkHelper.GetVoxelFromChunkCoordinates(chunk, localPosition + new Vector3Int(0, 1, 0)) ==
                    VoxelType.Water ||
                    ChunkHelper.GetVoxelFromChunkCoordinates(chunk, localPosition + new Vector3Int(0, 0, 1)) ==
                    VoxelType.Water ||
                    ChunkHelper.GetVoxelFromChunkCoordinates(chunk, localPosition + new Vector3Int(-1, 0, 0)) ==
                    VoxelType.Water ||
                    ChunkHelper.GetVoxelFromChunkCoordinates(chunk, localPosition + new Vector3Int(0, 0, -1)) ==
                    VoxelType.Water)
                {
                    ChunkHelper.SetVoxel(chunk, localPosition, VoxelType.Water);
                    Durability.Remove(pos);
                }
                else
                {
                    ChunkHelper.SetVoxel(chunk, localPosition, VoxelType.Air);
                    Durability.Remove(pos);
                }
            }
            else
            {
                Durability[pos] = durability;
            }
        }
        else Durability.Add(pos, durability);
    }


    internal static void SetNewVoxel(Terrain terrainReference, Vector3Int pos, VoxelType voxelType,
        Dictionary<Vector3Int, int> durability, bool place)
    {
        Chunk chunk = GetChunkData(terrainReference, pos);
        if (chunk == null) return;
        Vector3Int localPosition = ChunkHelper.GetVoxelPosInChunkCoordinates(chunk, pos);
        VoxelType localType = ChunkHelper.GetVoxelFromChunkCoordinates(chunk, localPosition);


        switch (localType)
        {
            case VoxelType.Grass when place == false:
                SetDurability(chunk, localPosition, pos, VoxelType.Grass, durability);
                break;
            case VoxelType.Dirt when place == false:
                SetDurability(chunk, localPosition, pos, VoxelType.Dirt, durability);
                break;
            case VoxelType.Stone when place == false:
                SetDurability(chunk, localPosition, pos, VoxelType.Stone, durability);
                break;
            case VoxelType.TreeTrunk when place == false:
                SetDurability(chunk, localPosition, pos, VoxelType.TreeTrunk, durability);
                break;
            case VoxelType.TreeLeaves when place == false:
                SetDurability(chunk, localPosition, pos, VoxelType.TreeLeaves, durability);
                break;
            case VoxelType.Water when place == false:
                SetDurability(chunk, localPosition, pos, VoxelType.Water, durability);
                break;
            case VoxelType.Sand when place == false:
                SetDurability(chunk, localPosition, pos, VoxelType.Sand, durability);
                break;
            default:
                ChunkHelper.SetVoxel(chunk, localPosition, voxelType);
                break;
        }
    }

    /// <summary>
    /// "Get the chunk data for the chunk that contains the given position, and if it exists, set the voxel
    /// at that position to the given voxel type."
    /// 
    /// The first line of the function is a comment. Comments are ignored by the compiler, and are used to
    /// explain what the code is doing
    /// </summary>
    /// <param name="terrainReference">The terrain reference.</param>
    /// <param name="pos">The position of the voxel you want to set.</param>
    /// <param name="voxelType">The type of voxel you want to set.</param>
    internal static void SetVoxel(Terrain terrainReference, Vector3Int pos, VoxelType voxelType)
    {
        Chunk chunk = GetChunkData(terrainReference, pos);
        if (chunk == null) return;
        Vector3Int localPosition = ChunkHelper.GetVoxelPosInChunkCoordinates(chunk, pos);
        ChunkHelper.SetVoxel(chunk, localPosition, voxelType);
    }

    public static Chunk GetChunkData(Terrain terrainReference, Vector3Int terrainVoxelPosition)
    {
        Vector3Int chunkPosition = GetChunkPositionFromCoordinates(terrainReference, terrainVoxelPosition);

        terrainReference.terrainData.chunkDictionary.TryGetValue(chunkPosition, out var containerChunk);

        return containerChunk;
    }

    internal static List<Vector3Int> GetUnnecessaryData(TerrainData terrainData,
        List<Vector3Int> allChunkDataPositionsNeeded)
    {
        return terrainData.chunkDictionary.Keys
            .Where(pos => allChunkDataPositionsNeeded.Contains(pos) == false &&
                          terrainData.chunkDictionary[pos].isPlayerModified == false)
            .ToList();
    }

    internal static List<Vector3Int> GetUnnecessaryChunks(TerrainData terrainData,
        List<Vector3Int> allChunkPositionsNeeded)
    {
        List<Vector3Int> positionToRemove = new List<Vector3Int>();
        foreach (var pos in terrainData.chunkRendererDictionary.Keys
                     .Where(pos => allChunkPositionsNeeded.Contains(pos) == false))
        {
            if (terrainData.chunkRendererDictionary.ContainsKey(pos))
            {
                positionToRemove.Add(pos);
            }
        }

        return positionToRemove;
    }

    internal static List<Vector3Int> SelectPositionsToCreate(TerrainData terrainData,
        List<Vector3Int> allChunkPositionsNeeded, Vector3Int playerPosition)
    {
        return allChunkPositionsNeeded
            .Where(pos => terrainData.chunkRendererDictionary.ContainsKey(pos) == false)
            .OrderBy(pos => Vector3.Distance(playerPosition, pos))
            .ToList();
    }

    internal static List<Vector3Int> SelectDataPositionsToCreate(TerrainData terrainData,
        List<Vector3Int> allChunkDataPositionsNeeded, Vector3Int playerPosition)
    {
        return allChunkDataPositionsNeeded
            .Where(pos => terrainData.chunkDictionary.ContainsKey(pos) == false)
            .OrderBy(pos => Vector3.Distance(playerPosition, pos))
            .ToList();
    }
}