using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class UndergroundLayerHandler : LayerHandler
{
    public VoxelType undergroundVoxelType;
    protected override bool TryHandling(Chunk chunk, int x, int y, int z, int surfaceHeightNoise, Vector2Int mapSeedOffset)
    {
        if (y >= surfaceHeightNoise) return false;
        Vector3Int pos = new Vector3Int(x, y - chunk.terrainPosition.y, z);
        ChunkHelper.SetVoxel(chunk, pos, undergroundVoxelType);
        return true;
    }
}