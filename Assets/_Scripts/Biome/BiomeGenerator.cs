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

    /* It's a reference to a class that handles the first layer of voxels. */
    public LayerHandler startLayerHandler;

    public TreeGenerator treeGenerator;

    public List<LayerHandler> additionalLayerHandlers;

    /// <summary>
    /// If the treeGenerator is null, return an empty TreeData, otherwise return the treeGenerator's
    /// CreateTreeData function.
    /// </summary>
    /// <param name="data">This is the data that the chunk is using to generate itself.</param>
    /// <param name="mapSeedOffset">The position of the chunk in the terrain.</param>
    /// <returns>
    /// A TreeData object.
    /// </returns>
    internal TreeData GetTreeData(Chunk data, Vector2Int mapSeedOffset)
    {
        return treeGenerator is null ? new TreeData() : treeGenerator.CreateTreeData(data, mapSeedOffset);
    }


    /// <summary>
    /// It takes a chunk of data, and adds voxels to it
    /// </summary>
    /// <param name="data">This is the data that will be returned. It contains the chunk's position,
    /// the chunk's height, and the chunk's voxels.</param>
    /// <param name="x">The x position of the chunk column</param>
    /// <param name="z">The z position of the chunk column</param>
    /// <param name="seedOffset">The position of the chunk in the terrain.</param>
    /// <param name="noiseHeight">This is the height of the terrain at the given x,z position. If you
    /// don't have this, you can use the GetSurfaceHeight function to get it.</param>
    /// <returns>
    /// The Chunk object is being returned.
    /// </returns>
    public Chunk ProcessChunkColumn(Chunk data, int x, int z, Vector2Int seedOffset, int? noiseHeight)
    {
        biomeNoiseSettings.terrainOffset = seedOffset;

        var groundPosition = noiseHeight.HasValue == false
            ? GetSurfaceHeight(data.terrainPosition.x + x, data.terrainPosition.z + z, data.chunkHeight)
            : noiseHeight.Value;

        for (var y = data.terrainPosition.y; y < data.terrainPosition.y + data.chunkHeight; y++)
        {
            startLayerHandler.Handle(data, x, y, z, groundPosition, seedOffset);
        }

        foreach (var layer in additionalLayerHandlers)
        {
            layer.Handle(data, x, data.terrainPosition.y, z, groundPosition, seedOffset);
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
    public int GetSurfaceHeight(int x, int z, int chunkHeight)
    {
        var terrainHeight = domainWarpingEnabled == false
            ? Noise.GetNoise(x, z, biomeNoiseSettings)
            : domainWarping.GenerateDomainNoise(x, z, biomeNoiseSettings);

        terrainHeight = Noise.Redistribution(terrainHeight, biomeNoiseSettings);
        return Noise.RemapValueToInt(terrainHeight, 0, chunkHeight);
    }
    
    /// <summary>
    /// It takes the player's position, the draw range, and the map size, and returns a list of biome
    /// centers
    /// </summary>
    /// <param name="playerPosition">playerPosition</param>
    /// <param name="drawRange">The number of biomes to draw in each direction.</param>
    /// <param name="chunkSize">The size of the map in chunks.</param>
    /// <returns>
    /// A list of Vector3Ints
    /// </returns>
    public static List<Vector3Int> CalculateBiomeCenters(Vector3 playerPosition, int drawRange, int chunkSize)
    {
        int biomeLength = drawRange * chunkSize;

        Vector3Int originPoint = new Vector3Int(
            Mathf.RoundToInt(playerPosition.x / biomeLength) * biomeLength,
            0,
            Mathf.RoundToInt(playerPosition.z / biomeLength) * biomeLength);

        HashSet<Vector3Int> centersTemp = new HashSet<Vector3Int> { originPoint };

        foreach (var offsetXZ in Directions.Directions2D)
        {
            var newBiomePoint1 = new Vector3Int(originPoint.x + offsetXZ.x * biomeLength, 0,
                originPoint.z + offsetXZ.y * biomeLength);
            var newBiomePoint2 = new Vector3Int(originPoint.x + offsetXZ.x * biomeLength, 0,
                originPoint.z + offsetXZ.y * 2 * biomeLength);
            var newBiomePoint3 = new Vector3Int(originPoint.x + offsetXZ.x * 2 * biomeLength, 0,
                originPoint.z + offsetXZ.y * biomeLength);
            var newBiomePoint4 = new Vector3Int(originPoint.x + offsetXZ.x * 2 * biomeLength, 0,
                originPoint.z + offsetXZ.y * 2 * biomeLength);
            centersTemp.Add(newBiomePoint1);
            centersTemp.Add(newBiomePoint2);
            centersTemp.Add(newBiomePoint3);
            centersTemp.Add(newBiomePoint4);
        }

        return new List<Vector3Int>(centersTemp);
    }
}