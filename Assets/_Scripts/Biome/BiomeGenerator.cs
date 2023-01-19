using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

/* It's a class that generates biomes. */
public class BiomeGenerator : MonoBehaviour
{
    /* It's a reference to a class that handles the noise settings. */
    public NoiseSettings biomeNoiseSettings;

    /* It's a reference to a class that handles the domain warping. */
    public DomainWarping domainWarping;

    public bool domainWarpingEnabled = true;

    /* It's a reference to a class that handles the first layer of blocks. */
    public LayerHandler startLayerHandler;

    public TreeGenerator treeGenerator;

    public List<LayerHandler> additionalLayerHandlers;

    /// <summary>
    /// If the treeGenerator is null, return an empty TreeData, otherwise return the treeGenerator's
    /// GenerateTreeData function.
    /// </summary>
    /// <param name="data">This is the data that the chunk is using to generate itself.</param>
    /// <param name="mapSeedOffset">The position of the chunk in the world.</param>
    /// <returns>
    /// A TreeData object.
    /// </returns>
    internal TreeData GetTreeData(ChunkData data, Vector2Int mapSeedOffset)
    {
        return treeGenerator is null ? new TreeData() : treeGenerator.GenerateTreeData(data, mapSeedOffset);
    }


    /// <summary>
    /// It takes a chunk of data, and adds blocks to it
    /// </summary>
    /// <param name="data">This is the data that will be returned. It contains the chunk's position,
    /// the chunk's height, and the chunk's blocks.</param>
    /// <param name="x">The x position of the chunk column</param>
    /// <param name="z">The z position of the chunk column</param>
    /// <param name="seedOffset">The position of the chunk in the world.</param>
    /// <param name="noiseHeight">This is the height of the terrain at the given x,z position. If you
    /// don't have this, you can use the GetSurfaceHeightNoise function to get it.</param>
    /// <returns>
    /// The ChunkData object is being returned.
    /// </returns>
    public ChunkData ProcessChunkColumn(ChunkData data, int x, int z, Vector2Int seedOffset, int? noiseHeight)
    {
        biomeNoiseSettings.worldOffset = seedOffset;

        var groundPosition = noiseHeight.HasValue == false
            ? GetSurfaceHeightNoise(data.worldPosition.x + x, data.worldPosition.z + z, data.chunkHeight)
            : noiseHeight.Value;

        for (var y = data.worldPosition.y; y < data.worldPosition.y + data.chunkHeight; y++)
        {
            startLayerHandler.Handle(data, x, y, z, groundPosition, seedOffset);
        }

        foreach (var layer in additionalLayerHandlers)
        {
            layer.Handle(data, x, data.worldPosition.y, z, groundPosition, seedOffset);
        }

        return data;
    }

    /// <summary>
    /// It takes in a chunk position and a chunk height, and returns a height value for the surface of the
    /// terrain at that chunk position
    /// </summary>
    /// <param name="x">The x coordinate of the chunk.</param>
    /// <param name="z">The z coordinate of the chunk.</param>
    /// <param name="chunkHeight">The height of the chunk.</param>
    /// <returns>
    /// The surface height of the terrain.
    /// </returns>
    public int GetSurfaceHeightNoise(int x, int z, int chunkHeight)
    {
        var terrainHeight = domainWarpingEnabled == false
            ? MyOctavePerlin.OctavePerlin(x, z, biomeNoiseSettings)
            : domainWarping.GenerateDomainNoise(x, z, biomeNoiseSettings);

        terrainHeight = MyOctavePerlin.Redistribution(terrainHeight, biomeNoiseSettings);
        return MyOctavePerlin.RemapValueToInt(terrainHeight, 0, chunkHeight);
    }
}