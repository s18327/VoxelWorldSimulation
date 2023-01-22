using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoneLayerHandler : LayerHandler
{
    [Range(0, 1)]
    public float stoneThreshold = 0.5f;

    [SerializeField]
    private NoiseSettings stoneNoiseSettings;

    public DomainWarping domainWarping;

    protected override bool TryHandling(Chunk chunk, int x, int y, int z, int surfaceHeightNoise, Vector2Int mapSeedOffset)
    {
        if (chunk.terrainPosition.y > surfaceHeightNoise)
            return false;

        stoneNoiseSettings.terrainOffset = mapSeedOffset;
        float stoneNoise = domainWarping.GenerateDomainNoise(chunk.terrainPosition.x + x, chunk.terrainPosition.z + z, stoneNoiseSettings);
        int endPosition = surfaceHeightNoise;
        if (chunk.terrainPosition.y < 0)
        {
            endPosition = chunk.terrainPosition.y + chunk.chunkHeight;
        }

        if (!(stoneNoise > stoneThreshold)) return false;
        
        for (int i = chunk.terrainPosition.y; i <= endPosition; i++)
        {
            Vector3Int pos = new Vector3Int(x, i, z);
            ChunkHelper.SetVoxel(chunk, pos, VoxelType.Stone);
        }
        return true;
    }
}
