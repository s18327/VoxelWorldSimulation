using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    public BiomeGenerator biomeGenerator;

    [SerializeField] List<Vector3Int> biomeCenters = new ();
    List<float> biomeNoise = new ();

    [SerializeField] private NoiseSettings biomeNoiseSettings;

    public DomainWarping biomeDomainWarping;

    [SerializeField] private List<BiomeData> biomeGeneratorsData = new();


    public ChunkData GenerateChunkData(ChunkData data, Vector2Int mapSeedOffset)
    {
        BiomeGeneratorSelection biomeSelection = SelectBiomeGenerator(data.worldPosition, data, false);
        data.treeData = biomeSelection.biomeGenerator.GetTreeData(data, mapSeedOffset);
        for (int x = 0; x < data.chunkSize; x++)
        {
            for (int z = 0; z < data.chunkSize; z++)
            {
                biomeSelection =
                    SelectBiomeGenerator(new Vector3Int(data.worldPosition.x + x, 0, data.worldPosition.z + z), data);
                data = biomeSelection.biomeGenerator.ProcessChunkColumn(data, x, z, mapSeedOffset,
                    biomeSelection.terrainSurfaceNoise);
            }
        }

        return data;
    }

    private BiomeGeneratorSelection SelectBiomeGenerator(Vector3Int worldPosition, ChunkData data,
        bool useDomainWarping = true)
    {
        if (useDomainWarping)
        {
            Vector2Int domainOffset =
                Vector2Int.RoundToInt(biomeDomainWarping.GenerateDomainOffset(worldPosition.x, worldPosition.z));
            worldPosition += new Vector3Int(domainOffset.x, 0, domainOffset.y);
        }

        List<BiomeSelectionHelper> biomeSelectionHelpers = GetBiomeGeneratorSelectionHelpers(worldPosition);
        BiomeGenerator generator1 = SelectBiome(biomeSelectionHelpers[0].Index);
        BiomeGenerator generator2 = SelectBiome(biomeSelectionHelpers[1].Index);

        float distance =
            Vector3.Distance(
                biomeCenters[biomeSelectionHelpers[0].Index],
                biomeCenters[biomeSelectionHelpers[1].Index]);
        float weight0 = biomeSelectionHelpers[0].Distance / distance;
        float weight1 = 1 - weight0;
        int terrainHeightNoise0 = generator1.GetSurfaceHeightNoise(worldPosition.x, worldPosition.z, data.chunkHeight);
        int terrainHeightNoise1 = generator2.GetSurfaceHeightNoise(worldPosition.x, worldPosition.z, data.chunkHeight);
        return new BiomeGeneratorSelection(generator1,
            Mathf.RoundToInt(terrainHeightNoise0 * weight0 + terrainHeightNoise1 * weight1));
    }

    private BiomeGenerator SelectBiome(int index)
    {
        float temp = biomeNoise[index];
        foreach (var data in biomeGeneratorsData)
        {
            if (temp >= data.temperatureStartThreshold && temp < data.temperatureEndThreshold)
                return data.biomeTerrainGenerator;
        }

        return biomeGeneratorsData[0].biomeTerrainGenerator;
    }

    private List<BiomeSelectionHelper> GetBiomeGeneratorSelectionHelpers(Vector3Int position)
    {
        position.y = 0;
        return GetClosestBiomeIndex(position);
    }

    private List<BiomeSelectionHelper> GetClosestBiomeIndex(Vector3Int position)
    {
        return biomeCenters.Select((center, index) =>
            new BiomeSelectionHelper
            {
                Index = index,
                Distance = Vector3.Distance(center, position)
            }).OrderBy(helper => helper.Distance).Take(4).ToList();
    }

    private struct BiomeSelectionHelper
    {
        public int Index;
        public float Distance;
    }

    public void GenerateBiomePoints(Vector3 playerPosition, int drawRange, int chunkSize, Vector2Int mapSeedOffset)
    {
        biomeCenters = new List<Vector3Int>();
        biomeCenters = BiomeCenterFinder.CalculateBiomeCenters(playerPosition, drawRange, chunkSize);

        for (int i = 0; i < biomeCenters.Count; i++)
        {
            Vector2Int domainWarpingOffset
                = biomeDomainWarping.GenerateDomainOffsetInt(biomeCenters[i].x, biomeCenters[i].y);
            biomeCenters[i] += new Vector3Int(domainWarpingOffset.x, 0, domainWarpingOffset.y);
        }

        biomeNoise = CalculateBiomeNoise(biomeCenters, mapSeedOffset);
    }

    private List<float> CalculateBiomeNoise(List<Vector3Int> biomeCenters, Vector2Int mapSeedOffset)
    {
        biomeNoiseSettings.worldOffset = mapSeedOffset;
        return biomeCenters.Select(center => MyOctavePerlin.OctavePerlin(center.x, center.y, biomeNoiseSettings)).ToList();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;

        foreach (var biomeCenterPoint in biomeCenters)
        {
            Gizmos.DrawLine(biomeCenterPoint, biomeCenterPoint + Vector3.up * 255);
        }
    }
}

[Serializable]
public struct BiomeData
{
    [Range(0f, 1f)] public float temperatureStartThreshold, temperatureEndThreshold;
    public BiomeGenerator biomeTerrainGenerator;
}

public class BiomeGeneratorSelection
{
    public BiomeGenerator biomeGenerator = null;
    public int? terrainSurfaceNoise = null;

    public BiomeGeneratorSelection(BiomeGenerator biomeGeneror, int? terrainSurfaceNoise = null)
    {
        this.biomeGenerator = biomeGeneror;
        this.terrainSurfaceNoise = terrainSurfaceNoise;
    }
}