using System.Collections.Generic;
using UnityEngine;

public static class BiomeCenterFinder
{
    /// <summary>
    /// It takes the player's position, the draw range, and the map size, and returns a list of biome
    /// centers
    /// </summary>
    /// <param name="playerPosition">playerPosition</param>
    /// <param name="drawRange">The number of biomes to draw in each direction.</param>
    /// <param name="mapSize">The size of the map in chunks.</param>
    /// <returns>
    /// A list of Vector3Ints
    /// </returns>
    public static List<Vector3Int> CalculateBiomeCenters(Vector3 playerPosition, int drawRange, int mapSize)
    {
        int biomeLength = drawRange * mapSize;

        Vector3Int origin = new Vector3Int(
            Mathf.RoundToInt(playerPosition.x / biomeLength) * biomeLength,
            0,
            Mathf.RoundToInt(playerPosition.z / biomeLength) * biomeLength);

        HashSet<Vector3Int> biomeCentersTemp = new HashSet<Vector3Int>();

        biomeCentersTemp.Add(origin);

        foreach (Vector2Int offsetXZ in Directions.Directions2D)
        {
            Vector3Int newBiomePoint1 = new Vector3Int(origin.x + offsetXZ.x * biomeLength, 0,
                origin.z + offsetXZ.y * biomeLength);
            Vector3Int newBiomePoint2 = new Vector3Int(origin.x + offsetXZ.x * biomeLength, 0,
                origin.z + offsetXZ.y * 2 * biomeLength);
            Vector3Int newBiomePoint3 = new Vector3Int(origin.x + offsetXZ.x * 2 * biomeLength, 0,
                origin.z + offsetXZ.y * biomeLength);
            Vector3Int newBiomePoint4 = new Vector3Int(origin.x + offsetXZ.x * 2 * biomeLength, 0,
                origin.z + offsetXZ.y * 2 * biomeLength);
            biomeCentersTemp.Add(newBiomePoint1);
            biomeCentersTemp.Add(newBiomePoint2);
            biomeCentersTemp.Add(newBiomePoint3);
            biomeCentersTemp.Add(newBiomePoint4);
        }

        return new List<Vector3Int>(biomeCentersTemp);
    }
}