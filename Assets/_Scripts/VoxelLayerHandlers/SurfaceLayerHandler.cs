using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class SurfaceLayerHandler : LayerHandler
{
    public VoxelType surfaceVoxelType;
    protected override bool TryHandling(ChunkData chunkData, int x, int y, int z, int surfaceHeightNoise, Vector2Int mapSeedOffset)
    {
        if (y != surfaceHeightNoise) return false; //TODO: Make sure it is not lower than water 
        Vector3Int pos = new Vector3Int(x, y, z);
        Chunk.SetBlock(chunkData, pos, surfaceVoxelType);
        return true;
    }
}
