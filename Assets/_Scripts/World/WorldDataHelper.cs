using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/* It's a helper class that contains methods that are used to get and set data in the world */
public static class WorldDataHelper
{
    /// <summary>
    /// > It takes a world and a world block position and returns the chunk position of the chunk that
    /// contains that block
    /// </summary>
    /// <param name="world">The world object that contains the chunk</param>
    /// <param name="worldBlockPosition">The position of the block in world space.</param>
    /// <returns>
    /// A Vector3Int
    /// </returns>
    public static Vector3Int GetChunkPositionFromVoxelCoordinates(World world, Vector3Int worldBlockPosition)
    {
        return new Vector3Int
        {
            x = Mathf.FloorToInt(worldBlockPosition.x / (float)world.chunkSize) * world.chunkSize,
            y = Mathf.FloorToInt(worldBlockPosition.y / (float)world.chunkHeight) * world.chunkHeight,
            z = Mathf.FloorToInt(worldBlockPosition.z / (float)world.chunkSize) * world.chunkSize
        };
    }

    /// <summary>
    /// It returns a list of chunk positions that are within a certain range of the player
    /// </summary>
    /// <param name="world">The world object that contains world information.</param>
    /// <param name="playerPosition">The position of the player in the world.</param>
    /// <returns>
    /// A list of Vector3Ints.
    /// </returns>
    internal static List<Vector3Int> GetChunkPositionsAroundPlayer(World world, Vector3Int playerPosition)
    {
        int startX = playerPosition.x - (world.chunkDrawingRange) * world.chunkSize;
        int startZ = playerPosition.z - (world.chunkDrawingRange) * world.chunkSize;
        int endX = playerPosition.x + (world.chunkDrawingRange) * world.chunkSize;
        int endZ = playerPosition.z + (world.chunkDrawingRange) * world.chunkSize;

        List<Vector3Int> listOfChunkPositionsAroundThePlayer = new List<Vector3Int>();
        for (int x = startX; x <= endX; x += world.chunkSize)
        {
            for (int z = startZ; z <= endZ; z += world.chunkSize)
            {
                Vector3Int chunkPositionFromBlockCoordinates =
                    GetChunkPositionFromVoxelCoordinates(world, new Vector3Int(x, 0, z));
                listOfChunkPositionsAroundThePlayer.Add(chunkPositionFromBlockCoordinates);
                if (x < playerPosition.x - world.chunkSize
                    || x > playerPosition.x + world.chunkSize
                    || z < playerPosition.z - world.chunkSize
                    || z > playerPosition.z + world.chunkSize) continue;

                for (int y = -world.chunkHeight; y >= playerPosition.y - world.chunkHeight * 2; y -= world.chunkHeight)
                {
                    chunkPositionFromBlockCoordinates =
                        GetChunkPositionFromVoxelCoordinates(world, new Vector3Int(x, y, z));
                    listOfChunkPositionsAroundThePlayer.Add(chunkPositionFromBlockCoordinates);
                }
            }
        }

        return listOfChunkPositionsAroundThePlayer;
    }

    /// <summary>
    /// Remove the chunk data at the given position from the world's chunk data dictionary
    /// </summary>
    /// <param name="world">The world that the chunk is in.</param>
    /// <param name="pos">The position of the chunk.</param>
    internal static void RemoveChunkData(World world, Vector3Int pos)
    {
        world.gameData.chunkDataDictionary.Remove(pos);
    }

    /// <summary>
    /// If the chunk exists, remove it from the world
    /// </summary>
    /// <param name="world">The world that the chunk is in.</param>
    /// <param name="pos">The position of the chunk you want to remove.</param>
    internal static void RemoveChunk(World world, Vector3Int pos)
    {
        if (!world.gameData.chunkDictionary.TryGetValue(pos, out var chunk)) return;

        world.worldRenderer.DeleteChunk(chunk);
        world.gameData.chunkDictionary.Remove(pos);
    }

    /// <summary>
    /// It returns a list of chunk positions that are within a certain range of the player
    /// </summary>
    /// <param name="world">The world object that contains the chunk size and chunk height.</param>
    /// <param name="playerPosition">The position of the player in the world.</param>
    /// <returns>
    /// A list of Vector3Ints.
    /// </returns>
    internal static List<Vector3Int> GetDataPositionsAroundPlayer(World world, Vector3Int playerPosition)
    {
        int startX = playerPosition.x - (world.chunkDrawingRange + 1) * world.chunkSize;
        int startZ = playerPosition.z - (world.chunkDrawingRange + 1) * world.chunkSize;
        int endX = playerPosition.x + (world.chunkDrawingRange + 1) * world.chunkSize;
        int endZ = playerPosition.z + (world.chunkDrawingRange + 1) * world.chunkSize;

        List<Vector3Int> chunkDataPositionsToCreate = new List<Vector3Int>();
        for (int x = startX; x <= endX; x += world.chunkSize)
        {
            for (int z = startZ; z <= endZ; z += world.chunkSize)
            {
                Vector3Int chunkPos = GetChunkPositionFromVoxelCoordinates(world, new Vector3Int(x, 0, z));
                chunkDataPositionsToCreate.Add(chunkPos);
                if (x < playerPosition.x - world.chunkSize
                    || x > playerPosition.x + world.chunkSize
                    || z < playerPosition.z - world.chunkSize
                    || z > playerPosition.z + world.chunkSize) continue;

                for (int y = -world.chunkHeight; y >= playerPosition.y - world.chunkHeight * 2; y -= world.chunkHeight)
                {
                    chunkPos = GetChunkPositionFromVoxelCoordinates(world, new Vector3Int(x, y, z));
                    chunkDataPositionsToCreate.Add(chunkPos);
                }
            }
        }

        return chunkDataPositionsToCreate;
    }

    /// <summary>
    /// If the chunk dictionary contains the key, return the chunk. Otherwise, return null
    /// </summary>
    /// <param name="worldReference">The world that the chunk is in.</param>
    /// <param name="worldPosition">This is a struct that contains 3 integers. It's used to store the position
    /// of a chunk in the world.</param>
    /// <returns>
    /// A ChunkRenderer object.
    /// </returns>
    internal static ChunkRenderer GetChunk(World worldReference, Vector3Int worldPosition)
    {
        return worldReference.gameData.chunkDictionary.ContainsKey(worldPosition)
            ? worldReference.gameData.chunkDictionary[worldPosition]
            : null;
    }

    /// <summary>
    /// It takes a position, a block type, and a dictionary of positions and integers. It then finds the
    /// durability of the block type and adds it to the dictionary
    /// </summary>
    /// <param name="pos">The position of the block</param>
    /// <param name="voxelType">The type of block you want to get the durability of.</param>
    /// <param name="Durability">A dictionary that stores the durability of each block.</param>
    private static void GetDurability(Vector3Int pos, VoxelType voxelType, Dictionary<Vector3Int, int> Durability)
    {
        GameManager gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
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


    private static void SetDurability(ChunkData chunkData, Vector3Int localPosition, Vector3Int pos,
        VoxelType voxelType, Dictionary<Vector3Int, int> Durability)
    {
        GetDurability(pos, voxelType, Durability);
        if (Durability.TryGetValue(pos, out var durability))
        {
            durability--;
            if (durability <= 0)
            {
                if (
                    Chunk.GetBlockFromChunkCoordinates(chunkData, localPosition + new Vector3Int(1, 0, 0)) ==
                    VoxelType.Water ||
                    Chunk.GetBlockFromChunkCoordinates(chunkData, localPosition + new Vector3Int(0, 1, 0)) ==
                    VoxelType.Water ||
                    Chunk.GetBlockFromChunkCoordinates(chunkData, localPosition + new Vector3Int(0, 0, 1)) ==
                    VoxelType.Water ||
                    Chunk.GetBlockFromChunkCoordinates(chunkData, localPosition + new Vector3Int(-1, 0, 0)) ==
                    VoxelType.Water ||
                    Chunk.GetBlockFromChunkCoordinates(chunkData, localPosition + new Vector3Int(0, 0, -1)) ==
                    VoxelType.Water)
                {
                    Chunk.SetBlock(chunkData, localPosition, VoxelType.Water);
                    Durability.Remove(pos);
                }
                else
                {
                    Chunk.SetBlock(chunkData, localPosition, VoxelType.Air);
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


    internal static void SetNewBlock(World worldReference, Vector3Int pos, VoxelType voxelType,
        Dictionary<Vector3Int, int> durability, bool place)
    {
        ChunkData chunkData = GetChunkData(worldReference, pos);
        if (chunkData == null) return;
        Vector3Int localPosition = Chunk.GetVoxelInChunkCoordinates(chunkData, pos);
        VoxelType localType = Chunk.GetBlockFromChunkCoordinates(chunkData, localPosition);


        switch (localType)
        {
            case VoxelType.Grass when place == false:
                SetDurability(chunkData, localPosition, pos, VoxelType.Grass, durability);
                break;
            case VoxelType.Dirt when place == false:
                SetDurability(chunkData, localPosition, pos, VoxelType.Dirt, durability);
                break;
            case VoxelType.Stone when place == false:
                SetDurability(chunkData, localPosition, pos, VoxelType.Stone, durability);
                break;
            case VoxelType.TreeTrunk when place == false:
                SetDurability(chunkData, localPosition, pos, VoxelType.TreeTrunk, durability);
                break;
            case VoxelType.TreeLeaves when place == false:
                SetDurability(chunkData, localPosition, pos, VoxelType.TreeLeaves, durability);
                break;
            case VoxelType.Water when place == false:
                SetDurability(chunkData, localPosition, pos, VoxelType.Water, durability);
                break;
            case VoxelType.Sand when place == false:
                SetDurability(chunkData, localPosition, pos, VoxelType.Sand, durability);
                break;
            default:
                Chunk.SetBlock(chunkData, localPosition, voxelType);
                break;
        }
    }

    /// <summary>
    /// "Get the chunk data for the chunk that contains the given position, and if it exists, set the block
    /// at that position to the given block type."
    /// 
    /// The first line of the function is a comment. Comments are ignored by the compiler, and are used to
    /// explain what the code is doing
    /// </summary>
    /// <param name="worldReference">The world reference.</param>
    /// <param name="pos">The position of the block you want to set.</param>
    /// <param name="voxelType">The type of block you want to set.</param>
    internal static void SetBlock(World worldReference, Vector3Int pos, VoxelType voxelType)
    {
        ChunkData chunkData = GetChunkData(worldReference, pos);
        if (chunkData == null) return;
        Vector3Int localPosition = Chunk.GetVoxelInChunkCoordinates(chunkData, pos);
        Chunk.SetBlock(chunkData, localPosition, voxelType);
    }

    public static ChunkData GetChunkData(World worldReference, Vector3Int worldBlockPosition)
    {
        Vector3Int chunkPosition = GetChunkPositionFromVoxelCoordinates(worldReference, worldBlockPosition);

        worldReference.gameData.chunkDataDictionary.TryGetValue(chunkPosition, out var containerChunk);

        return containerChunk;
    }

    internal static List<Vector3Int> GetUnnecessaryData(GameData gameData, List<Vector3Int> allChunkDataPositionsNeeded)
    {
        return gameData.chunkDataDictionary.Keys
            .Where(pos => allChunkDataPositionsNeeded.Contains(pos) == false &&
                          gameData.chunkDataDictionary[pos].modifiedByThePlayer == false)
            .ToList();
    }

    internal static List<Vector3Int> GetUnnecessaryChunks(GameData gameData, List<Vector3Int> allChunkPositionsNeeded)
    {
        List<Vector3Int> positionToRemove = new List<Vector3Int>();
        foreach (var pos in gameData.chunkDictionary.Keys
                     .Where(pos => allChunkPositionsNeeded.Contains(pos) == false))
        {
            if (gameData.chunkDictionary.ContainsKey(pos))
            {
                positionToRemove.Add(pos);
            }
        }

        return positionToRemove;
    }

    internal static List<Vector3Int> SelectPositionsToCreate(GameData gameData,
        List<Vector3Int> allChunkPositionsNeeded, Vector3Int playerPosition)
    {
        return allChunkPositionsNeeded
            .Where(pos => gameData.chunkDictionary.ContainsKey(pos) == false)
            .OrderBy(pos => Vector3.Distance(playerPosition, pos))
            .ToList();
    }

    internal static List<Vector3Int> SelectDataPositionsToCreate(GameData gameData,
        List<Vector3Int> allChunkDataPositionsNeeded, Vector3Int playerPosition)
    {
        return allChunkDataPositionsNeeded
            .Where(pos => gameData.chunkDataDictionary.ContainsKey(pos) == false)
            .OrderBy(pos => Vector3.Distance(playerPosition, pos))
            .ToList();
    }
}