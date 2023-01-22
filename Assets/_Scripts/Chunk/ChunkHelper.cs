using System;
using System.Collections.Generic;
using UnityEngine;


/* It's a class that contains all the functions that are used to manipulate chunks. */
public static class ChunkHelper
{

/// <summary>
/// "Loop through all the voxels in the chunk and perform the action passed in."
/// 
/// The first thing we do is create a for loop that loops through all the voxels in the chunk. We use
/// the `chunk.voxels.Length` to get the total number of voxels in the chunk
/// </summary>
/// <param name="chunk">The chunk data that you want to loop through.</param>
/// <param name="actionToPerform">This is the action that you want to perform on each voxel.</param>
    private static void IterateVoxels(Chunk chunk, Action<int, int, int> actionToPerform)
    {
        for (var index = 0; index < chunk.chunkVoxelStorage.Length; index++)
        {
            var position = GetPositionFromIndex(chunk, index);
            actionToPerform(position.x, position.y, position.z);
        }
    }

/// <summary>
/// `GetPositionFromIndex` takes a chunk data object and an index and returns a Vector3Int with the x,
/// y, and z coordinates of the voxel at that index
/// </summary>
/// <param name="chunk">The data of the chunk that is being generated.</param>
/// <param name="index">The index of the voxel in the chunk.</param>
/// <returns>
/// A Vector3Int with the x, y, and z coordinates of the voxel.
/// </returns>
    private static Vector3Int GetPositionFromIndex(Chunk chunk, int index)
    {
        var x = index % chunk.chunkSize;
        var y = (index / chunk.chunkSize) % chunk.chunkHeight;
        var z = index / (chunk.chunkSize * chunk.chunkHeight);
        return new Vector3Int(x, y, z);
    }

/// <summary>
/// "If the axisCoordinate is less than 0 or greater than or equal to the chunkSize, return false.
/// Otherwise, return true."
/// 
/// The function is used to determine if a coordinate is within the bounds of the chunk
/// </summary>
/// <param name="chunk">The chunk data that the voxel is in.</param>
/// <param name="coordinate">The coordinate of the voxel on the axis you're checking.</param>
/// <returns>
/// a boolean value.
/// </returns>
    private static bool IsInHorizontalRange(Chunk chunk, int coordinate)
{
    return coordinate >= 0 && coordinate < chunk.chunkSize;
}

/// <summary>
/// > Returns true if the y coordinate is within the height of the chunk
/// </summary>
/// <param name="chunk">The chunk data that the voxel is in.</param>
/// <param name="coordinate">The y coordinate of the voxel you want to check.</param>
/// <returns>
/// The return value is a boolean.
/// </returns>
    private static bool IsInVerticalRange(Chunk chunk, int coordinate)
{
    return coordinate >= 0 && coordinate < chunk.chunkHeight;
}

/// <summary>
/// > Get the voxel type at the given chunk coordinates
/// </summary>
/// <param name="chunk">The chunk data that you want to get the voxel from.</param>
/// <param name="chunkCoordinates">The chunk coordinates of the voxel you want to get.</param>
/// <returns>
/// A voxelType
/// </returns>
    public static VoxelType GetVoxelFromChunkCoordinates(Chunk chunk, Vector3Int chunkCoordinates)
    {
        return GetVoxelFromChunkCoordinates(chunk, chunkCoordinates.x, chunkCoordinates.y, chunkCoordinates.z);
    }

/// <summary>
/// If the voxel is in the chunk, return it, otherwise, ask the terrain for it
/// </summary>
/// <param name="chunk">The chunk that the voxel is in</param>
/// <param name="x">The x position of the voxel in the chunk</param>
/// <param name="y">The y position of the voxel in the chunk</param>
/// <param name="z">The z position of the voxel in the chunk</param>
/// <returns>
/// The voxel type at the given coordinates.
/// </returns>
    private static VoxelType GetVoxelFromChunkCoordinates(Chunk chunk, int x, int y, int z)
    {
        if (!IsInHorizontalRange(chunk, x) || !IsInVerticalRange(chunk, y) || !IsInHorizontalRange(chunk, z))
            return chunk.terrainReference.GetVoxelFromCoordinates( chunk.terrainPosition.x + x,
                  chunk.terrainPosition.y + y, chunk.terrainPosition.z + z);
        
        return chunk.chunkVoxelStorage[GetIndexFromPosition(chunk, x, y, z)];

    }

/// <summary>
/// If the voxel is in the chunk, set it. If it's not, set it in the terrain
/// </summary>
/// <param name="chunk">The chunk that the voxel is in</param>
/// <param name="localPosition">The position of the voxel in the chunk</param>
/// <param name="voxel">The type of voxel you want to set.</param>
    public static void SetVoxel(Chunk chunk, Vector3Int localPosition, VoxelType voxel)
    {
        if (IsInHorizontalRange(chunk, localPosition.x) && IsInVerticalRange(chunk, localPosition.y) && IsInHorizontalRange(chunk, localPosition.z))
        {

            int index = GetIndexFromPosition(chunk, localPosition.x, localPosition.y, localPosition.z);
            chunk.chunkVoxelStorage[index] = voxel;
        }
        else
        {
            TerrainHelper.SetVoxel(chunk.terrainReference, localPosition + chunk.terrainPosition, voxel);
        }
    }

/// <summary>
/// > Get the index of a voxel in a chunk from its x, y, and z coordinates
/// </summary>
/// <param name="chunk">The chunk data that the voxel is in.</param>
/// <param name="x">The x position of the voxel in the chunk.</param>
/// <param name="y">The y position of the voxel in the chunk.</param>
/// <param name="z">The z position of the voxel in the chunk.</param>
/// <returns>
/// The index of the voxel in the chunk.
/// </returns>
    private static int GetIndexFromPosition(Chunk chunk, int x, int y, int z)
    {
        return x + chunk.chunkSize * y + chunk.chunkSize * chunk.chunkHeight * z;
    }


/// <summary>
/// "Get the position of a voxel in a chunk, given the position of the voxel in the terrain."
/// 
/// The function takes in a chunk and a position in the terrain, and returns the position of the voxel in
/// the chunk.
/// 
/// The function is used in the following function:
/// </summary>
/// <param name="chunk">The chunk that the voxel is in.</param>
/// <param name="position">The position of the voxel in terrain coordinates.</param>
/// <returns>
/// A Vector3Int
/// </returns>
    public static Vector3Int GetVoxelPosInChunkCoordinates(Chunk chunk, Vector3Int position) //TODO: Get Relative position from chunk coordinate?
    {
        return new Vector3Int
        {
            x = position.x - chunk.terrainPosition.x,
            y = position.y - chunk.terrainPosition.y,
            z = position.z - chunk.terrainPosition.z
        };
    }

/// <summary>
/// Loop through all the voxels in the chunk and add their mesh data to the mesh data of the chunk
/// </summary>
/// <param name="chunk">The chunk data that contains the voxels</param>
/// <returns>
/// The mesh is being returned.
/// </returns>
    public static Mesh GetChunkMeshData(Chunk chunk)
    {
        Mesh mesh = new Mesh(true);

        IterateVoxels(chunk, (x, y, z) => mesh = VoxelHelper.GetMeshData(chunk, x, y, z, mesh, chunk.chunkVoxelStorage[GetIndexFromPosition(chunk, x, y, z)]));


        return mesh;
    }

/// <summary>
/// `ChunkPositionFromVoxelCoordinates` takes a terrain, and a voxel position, and returns the chunk position
/// that contains that voxel
/// </summary>
/// <param name="terrain">The terrain object that contains the chunk</param>
/// <param name="x">The x coordinate of the voxel</param>
/// <param name="y">The y-coordinate of the voxel.</param>
/// <param name="z">The z coordinate of the voxel.</param>
/// <returns>
/// A Vector3Int
/// </returns>
    internal static Vector3Int ChunkPositionFromVoxelCoordinates(Terrain terrain, int x, int y, int z)
    {
        return new Vector3Int
        {
            x = Mathf.FloorToInt(x / (float)terrain.chunkSize) * terrain.chunkSize,
            y = Mathf.FloorToInt(y / (float)terrain.chunkHeight) * terrain.chunkHeight,
            z = Mathf.FloorToInt(z / (float)terrain.chunkSize) * terrain.chunkSize
        };
    }

    /// <summary>
    /// It returns a list of chunks that are neighbours of the chunk that contains the given terrain
    /// position
    /// </summary>
    /// <param name="chunk">The chunk that the voxel is in</param>
    /// <param name="terrainPosition">The terrain position of the voxel you want to get the neighbours
    /// of.</param>
    /// <returns>
    /// A list of Chunk objects.
    /// </returns>
    internal static List<Chunk> GetEdgeNeighbourChunk(Chunk chunk, Vector3Int terrainPosition)
    {
        var chunkPosition = GetVoxelPosInChunkCoordinates(chunk, terrainPosition);
        var neighboursToUpdate = new List<Chunk>();
        if (chunkPosition.x == 0)
        {
            neighboursToUpdate.Add(TerrainHelper.GetChunkData(chunk.terrainReference, terrainPosition - Vector3Int.right));
        }
        if (chunkPosition.x == chunk.chunkSize - 1)
        {
            neighboursToUpdate.Add(TerrainHelper.GetChunkData(chunk.terrainReference, terrainPosition + Vector3Int.right));
        }
        if (chunkPosition.y == 0)
        {
            neighboursToUpdate.Add(TerrainHelper.GetChunkData(chunk.terrainReference, terrainPosition - Vector3Int.up));
        }
        if (chunkPosition.y == chunk.chunkHeight - 1)
        {
            neighboursToUpdate.Add(TerrainHelper.GetChunkData(chunk.terrainReference, terrainPosition + Vector3Int.up));
        }
        if (chunkPosition.z == 0)
        {
            neighboursToUpdate.Add(TerrainHelper.GetChunkData(chunk.terrainReference, terrainPosition - Vector3Int.forward));
        }
        if (chunkPosition.z == chunk.chunkSize - 1)
        {
            neighboursToUpdate.Add(TerrainHelper.GetChunkData(chunk.terrainReference, terrainPosition + Vector3Int.forward));
        }
        return neighboursToUpdate;
    }

/// <summary>
/// If the voxel is on the edge of the chunk, return true
/// </summary>
/// <param name="chunk">The chunk data that the voxel is in.</param>
/// <param name="terrainPosition">The position of the voxel in terrain coordinates.</param>
/// <returns>
/// A boolean value.
/// </returns>
    internal static bool IsOnEdge(Chunk chunk, Vector3Int terrainPosition)
    {
        var chunkPosition = GetVoxelPosInChunkCoordinates(chunk, terrainPosition);
        return chunkPosition.x == 0 || chunkPosition.x == chunk.chunkSize - 1 ||
               chunkPosition.y == 0 || chunkPosition.y == chunk.chunkHeight - 1 ||
               chunkPosition.z == 0 || chunkPosition.z == chunk.chunkSize - 1;
    }
}