using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterLayerHandler : LayerHandler
{
    public int waterLevel = 1;
    
    public VoxelType surfaceVoxelType;
    protected override bool TryHandling(ChunkData chunkData, int x, int y, int z, int surfaceHeightNoise, Vector2Int mapSeedOffset)
    {
        if (y <= surfaceHeightNoise || y > waterLevel) return false;
        
        Vector3Int pos = new Vector3Int(x, y, z);
        Chunk.SetBlock(chunkData, pos, surfaceVoxelType);
        
        if (y != surfaceHeightNoise + 1) return true;
        
        pos.y = surfaceHeightNoise;
        Chunk.SetBlock(chunkData, pos, VoxelType.Sand);
        
        return true;
    }
}
