using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeLayerHandler : LayerHandler
{
    public float terrainHeightLimit = 25;

/* A list of coordinates for the leaves of the tree. */
    private static readonly List<Vector3Int> treeLeavesStaticLayout = new ()
    {
        new (-2, 0, -2),
        new (-2, 0, -1),
        new (-2, 0, 0),
        new (-2, 0, 1),
        new (-2, 0, 2),
        new (-1, 0, -2),
        new (-1, 0, -1),
        new (-1, 0, 0),
        new (-1, 0, 1),
        new (-1, 0, 2),
        new (0, 0, -2),
        new (0, 0, -1),
        new (0, 0, 0),
        new (0, 0, 1),
        new (0, 0, 2),
        new (1, 0, -2),
        new (1, 0, -1),
        new (1, 0, 0),
        new (1, 0, 1),
        new (1, 0, 2),
        new (2, 0, -2),
        new (2, 0, -1),
        new (2, 0, 0),
        new (2, 0, 1),
        new (2, 0, 2),
        new (-1, 1, -1),
        new (-1, 1, 0),
        new (-1, 1, 1),
        new (0, 1, -1),
        new (0, 1, 0),
        new (0, 1, 1),
        new (1, 1, -1),
        new (1, 1, 0),
        new (1, 1, 1),
        new (0, 2, 0)
    };

/// <summary>
/// If the chunk is above the surface, and the chunk has a tree at the current position, and the block
/// at the current position is grass, then replace the grass with dirt, and place a tree trunk and
/// leaves
/// </summary>
/// <param name="chunkData">The chunk data that is being used.</param>
/// <param name="x">The x position of the block in the chunk.</param>
/// <param name="y">The y coordinate of the block in the chunk.</param>
/// <param name="z">The z coordinate of the block in the chunk.</param>
/// <param name="surfaceHeightNoise">The height of the surface at the current position.</param>
/// <param name="mapSeedOffset">The offset for the map seed.</param>
/// <returns>
/// The return value is a boolean.
/// </returns>
    protected override bool TryHandling(ChunkData chunkData, int x, int y, int z, int surfaceHeightNoise, Vector2Int mapSeedOffset)
    {
        if (chunkData.worldPosition.y < 0)
            return false;
        if (!(surfaceHeightNoise < terrainHeightLimit)
            || !chunkData.treeData.treePositions.Contains(new Vector2Int(chunkData.worldPosition.x + x,
                chunkData.worldPosition.z + z))) return false;
        
        Vector3Int chunkCoordinates = new Vector3Int(x, surfaceHeightNoise, z);
        VoxelType type = Chunk.GetBlockFromChunkCoordinates(chunkData, chunkCoordinates);
        if (type != VoxelType.Grass) return false;
        
        Chunk.SetBlock(chunkData, chunkCoordinates, VoxelType.Dirt);
        for (int i = 1; i < 5; i++)
        {
            chunkCoordinates.y = surfaceHeightNoise + i;
            Chunk.SetBlock(chunkData, chunkCoordinates, VoxelType.TreeTrunk);
        }
        foreach (Vector3Int leafPosition in treeLeavesStaticLayout)
        {
            chunkData.treeData.treeLeaves.Add(new Vector3Int(x + leafPosition.x, surfaceHeightNoise + 5 + leafPosition.y, z + leafPosition.z));
        }
        return false;
    }
}