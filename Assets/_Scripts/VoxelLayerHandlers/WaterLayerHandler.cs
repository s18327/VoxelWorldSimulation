using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterLayerHandler : LayerHandler
{
    public int waterLevel = 1;
    
    public VoxelType surfaceVoxelType;
    protected override bool TryHandling(Chunk chunk, int x, int y, int z, int surfaceHeightNoise, Vector2Int mapSeedOffset)
    {
        if (y <= surfaceHeightNoise || y > waterLevel) return false;
        
        Vector3Int pos = new Vector3Int(x, y, z);
        ChunkHelper.SetVoxel(chunk, pos, surfaceVoxelType);
        
        if (y != surfaceHeightNoise + 1) return true;
        
        pos.y = surfaceHeightNoise;
        ChunkHelper.SetVoxel(chunk, pos, VoxelType.Sand);
        
        return true;
    }
}
