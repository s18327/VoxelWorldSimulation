using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class UndergroundLayerHandler : LayerHandler
{
    [FormerlySerializedAs("undergroundBlockType")] public VoxelType undergroundVoxelType;
    protected override bool TryHandling(ChunkData chunkData, int x, int y, int z, int surfaceHeightNoise, Vector2Int mapSeedOffset)
    {
        if (y >= surfaceHeightNoise) return false;
        Vector3Int pos = new Vector3Int(x, y - chunkData.worldPosition.y, z);
        Chunk.SetBlock(chunkData, pos, undergroundVoxelType);
        return true;
    }
}