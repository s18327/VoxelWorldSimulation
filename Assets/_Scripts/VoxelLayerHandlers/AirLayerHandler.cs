using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirLayerHandler : LayerHandler
{
    protected override bool TryHandling(ChunkData chunkData, int x, int y, int z, int surfaceHeightNoise, Vector2Int mapSeedOffset)
    {
        if (y <= surfaceHeightNoise) return false;
        
        Vector3Int pos = new Vector3Int(x, y, z);
        Chunk.SetBlock(chunkData, pos, VoxelType.Air);
        return true;
    }
}
