using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirLayerHandler : LayerHandler
{
    protected override bool TryHandling(Chunk chunk, int x, int y, int z, int surfaceHeightNoise, Vector2Int mapSeedOffset)
    {
        if (y <= surfaceHeightNoise) return false;
        
        Vector3Int pos = new Vector3Int(x, y, z);
        ChunkHelper.SetVoxel(chunk, pos, VoxelType.Air);
        return true;
    }
}
