using System;
using System.Collections.Generic;
using UnityEngine;


/* It's a class that contains all the functions that are used to manipulate chunks. */
public static class Chunk
{

/// <summary>
/// "Loop through all the blocks in the chunk and perform the action passed in."
/// 
/// The first thing we do is create a for loop that loops through all the blocks in the chunk. We use
/// the `chunkData.blocks.Length` to get the total number of blocks in the chunk
/// </summary>
/// <param name="chunkData">The chunk data that you want to loop through.</param>
/// <param name="actionToPerform">This is the action that you want to perform on each block.</param>
    private static void IterateTheBlocks(ChunkData chunkData, Action<int, int, int> actionToPerform)
    {
        for (var index = 0; index < chunkData.chunkVoxelStorage.Length; index++)
        {
            var position = GetPositionFromIndex(chunkData, index);
            actionToPerform(position.x, position.y, position.z);
        }
    }

/// <summary>
/// `GetPositionFromIndex` takes a chunk data object and an index and returns a Vector3Int with the x,
/// y, and z coordinates of the voxel at that index
/// </summary>
/// <param name="chunkData">The data of the chunk that is being generated.</param>
/// <param name="index">The index of the block in the chunk.</param>
/// <returns>
/// A Vector3Int with the x, y, and z coordinates of the block.
/// </returns>
    private static Vector3Int GetPositionFromIndex(ChunkData chunkData, int index)
    {
        int x = index % chunkData.chunkSize;
        int y = (index / chunkData.chunkSize) % chunkData.chunkHeight;
        int z = index / (chunkData.chunkSize * chunkData.chunkHeight);
        return new Vector3Int(x, y, z);
    }

/// <summary>
/// "If the axisCoordinate is less than 0 or greater than or equal to the chunkSize, return false.
/// Otherwise, return true."
/// 
/// The function is used to determine if a coordinate is within the bounds of the chunk
/// </summary>
/// <param name="chunkData">The chunk data that the block is in.</param>
/// <param name="coordinate">The coordinate of the block on the axis you're checking.</param>
/// <returns>
/// a boolean value.
/// </returns>
    private static bool InRangeHorizontal(ChunkData chunkData, int coordinate)
{
    return coordinate >= 0 && coordinate < chunkData.chunkSize;
}

/// <summary>
/// > Returns true if the y coordinate is within the height of the chunk
/// </summary>
/// <param name="chunkData">The chunk data that the block is in.</param>
/// <param name="yCord">The y coordinate of the block you want to check.</param>
/// <returns>
/// The return value is a boolean.
/// </returns>
    private static bool InRangeVertical(ChunkData chunkData, int yCord)
{
    return yCord >= 0 && yCord < chunkData.chunkHeight;
}

/// <summary>
/// > Get the block type at the given chunk coordinates
/// </summary>
/// <param name="chunkData">The chunk data that you want to get the block from.</param>
/// <param name="chunkCoordinates">The chunk coordinates of the block you want to get.</param>
/// <returns>
/// A BlockType
/// </returns>
    public static VoxelType GetBlockFromChunkCoordinates(ChunkData chunkData, Vector3Int chunkCoordinates)
    {
        return GetBlockFromChunkCoordinates(chunkData, chunkCoordinates.x, chunkCoordinates.y, chunkCoordinates.z);
    }

/// <summary>
/// If the block is in the chunk, return it, otherwise, ask the world for it
/// </summary>
/// <param name="chunkData">The chunk that the block is in</param>
/// <param name="x">The x position of the block in the chunk</param>
/// <param name="y">The y position of the block in the chunk</param>
/// <param name="z">The z position of the block in the chunk</param>
/// <returns>
/// The block type at the given coordinates.
/// </returns>
    private static VoxelType GetBlockFromChunkCoordinates(ChunkData chunkData, int x, int y, int z)
    {
        if (!InRangeHorizontal(chunkData, x) || !InRangeVertical(chunkData, y) || !InRangeHorizontal(chunkData, z))
            return chunkData.worldReference.GetBlockFromChunkCoordinates(chunkData, chunkData.worldPosition.x + x,
                  chunkData.worldPosition.y + y, chunkData.worldPosition.z + z);
        
        return chunkData.chunkVoxelStorage[GetIndexFromPosition(chunkData, x, y, z)];

    }

/// <summary>
/// If the block is in the chunk, set it. If it's not, set it in the world
/// </summary>
/// <param name="chunkData">The chunk that the block is in</param>
/// <param name="localPosition">The position of the block in the chunk</param>
/// <param name="voxel">The type of block you want to set.</param>
    public static void SetBlock(ChunkData chunkData, Vector3Int localPosition, VoxelType voxel)
    {
        if (InRangeHorizontal(chunkData, localPosition.x) && InRangeVertical(chunkData, localPosition.y) && InRangeHorizontal(chunkData, localPosition.z))
        {

            int index = GetIndexFromPosition(chunkData, localPosition.x, localPosition.y, localPosition.z);
            chunkData.chunkVoxelStorage[index] = voxel;
        }
        else
        {
            WorldDataHelper.SetBlock(chunkData.worldReference, localPosition + chunkData.worldPosition, voxel);
        }
    }

/// <summary>
/// > Get the index of a voxel in a chunk from its x, y, and z coordinates
/// </summary>
/// <param name="chunkData">The chunk data that the block is in.</param>
/// <param name="x">The x position of the block in the chunk.</param>
/// <param name="y">The y position of the block in the chunk.</param>
/// <param name="z">The z position of the block in the chunk.</param>
/// <returns>
/// The index of the block in the chunk.
/// </returns>
    private static int GetIndexFromPosition(ChunkData chunkData, int x, int y, int z)
    {
        return x + chunkData.chunkSize * y + chunkData.chunkSize * chunkData.chunkHeight * z;
    }


/// <summary>
/// "Get the position of a block in a chunk, given the position of the block in the world."
/// 
/// The function takes in a chunk and a position in the world, and returns the position of the block in
/// the chunk.
/// 
/// The function is used in the following function:
/// </summary>
/// <param name="chunkData">The chunk that the block is in.</param>
/// <param name="pos">The position of the block in world coordinates.</param>
/// <returns>
/// A Vector3Int
/// </returns>
    public static Vector3Int GetVoxelInChunkCoordinates(ChunkData chunkData, Vector3Int pos)
    {
        return new Vector3Int
        {
            x = pos.x - chunkData.worldPosition.x,
            y = pos.y - chunkData.worldPosition.y,
            z = pos.z - chunkData.worldPosition.z
        };
    }

/// <summary>
/// Loop through all the blocks in the chunk and add their mesh data to the mesh data of the chunk
/// </summary>
/// <param name="chunkData">The chunk data that contains the blocks</param>
/// <returns>
/// The meshData is being returned.
/// </returns>
    public static MeshData GetChunkMeshData(ChunkData chunkData)
    {
        MeshData meshData = new MeshData(true);

        IterateTheBlocks(chunkData, (x, y, z) => meshData = VoxelHelper.GetMeshData(chunkData, x, y, z, meshData, chunkData.chunkVoxelStorage[GetIndexFromPosition(chunkData, x, y, z)]));


        return meshData;
    }

/// <summary>
/// `ChunkPositionFromBlockCoords` takes a world, and a block position, and returns the chunk position
/// that contains that block
/// </summary>
/// <param name="world">The world object that contains the chunk</param>
/// <param name="x">The x coordinate of the block</param>
/// <param name="y">The y-coordinate of the block.</param>
/// <param name="z">The z coordinate of the block.</param>
/// <returns>
/// A Vector3Int
/// </returns>
    internal static Vector3Int ChunkPositionFromBlockCoords(World world, int x, int y, int z)
    {
        return new Vector3Int
        {
            x = Mathf.FloorToInt(x / (float)world.chunkSize) * world.chunkSize,
            y = Mathf.FloorToInt(y / (float)world.chunkHeight) * world.chunkHeight,
            z = Mathf.FloorToInt(z / (float)world.chunkSize) * world.chunkSize
        };
    }

    /// <summary>
    /// It returns a list of chunks that are neighbours of the chunk that contains the given world
    /// position
    /// </summary>
    /// <param name="chunkData">The chunk that the block is in</param>
    /// <param name="worldPosition">The world position of the block you want to get the neighbours
    /// of.</param>
    /// <returns>
    /// A list of ChunkData objects.
    /// </returns>
    internal static List<ChunkData> GetEdgeNeighbourChunk(ChunkData chunkData, Vector3Int worldPosition)
    {
        var chunkPosition = GetVoxelInChunkCoordinates(chunkData, worldPosition);
        var neighboursToUpdate = new List<ChunkData>();
        if (chunkPosition.x == 0)
        {
            neighboursToUpdate.Add(WorldDataHelper.GetChunkData(chunkData.worldReference, worldPosition - Vector3Int.right));
        }
        if (chunkPosition.x == chunkData.chunkSize - 1)
        {
            neighboursToUpdate.Add(WorldDataHelper.GetChunkData(chunkData.worldReference, worldPosition + Vector3Int.right));
        }
        if (chunkPosition.y == 0)
        {
            neighboursToUpdate.Add(WorldDataHelper.GetChunkData(chunkData.worldReference, worldPosition - Vector3Int.up));
        }
        if (chunkPosition.y == chunkData.chunkHeight - 1)
        {
            neighboursToUpdate.Add(WorldDataHelper.GetChunkData(chunkData.worldReference, worldPosition + Vector3Int.up));
        }
        if (chunkPosition.z == 0)
        {
            neighboursToUpdate.Add(WorldDataHelper.GetChunkData(chunkData.worldReference, worldPosition - Vector3Int.forward));
        }
        if (chunkPosition.z == chunkData.chunkSize - 1)
        {
            neighboursToUpdate.Add(WorldDataHelper.GetChunkData(chunkData.worldReference, worldPosition + Vector3Int.forward));
        }
        return neighboursToUpdate;
    }

/// <summary>
/// If the block is on the edge of the chunk, return true
/// </summary>
/// <param name="chunkData">The chunk data that the block is in.</param>
/// <param name="worldPosition">The position of the block in world coordinates.</param>
/// <returns>
/// A boolean value.
/// </returns>
    internal static bool IsOnEdge(ChunkData chunkData, Vector3Int worldPosition)
    {
        var chunkPosition = GetVoxelInChunkCoordinates(chunkData, worldPosition);
        return chunkPosition.x == 0 || chunkPosition.x == chunkData.chunkSize - 1 ||
               chunkPosition.y == 0 || chunkPosition.y == chunkData.chunkHeight - 1 ||
               chunkPosition.z == 0 || chunkPosition.z == chunkData.chunkSize - 1;
    }
}