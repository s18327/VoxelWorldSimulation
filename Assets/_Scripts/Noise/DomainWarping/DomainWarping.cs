using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* It generates a 2D offset based on the input coordinates, and the noise settings */
public class DomainWarping : MonoBehaviour
{
    public NoiseSettings noiseDomainX, noiseDomainY;
    public int amplitudeX = 20, amplitudeY = 20;

/// <summary>
/// It takes in an x and z coordinate, and returns a float value between 0 and 1
/// </summary>
/// <param name="x">The x coordinate of the point in the domain.</param>
/// <param name="z">The z coordinate of the point in the noise map.</param>
/// <param name="defaultNoiseSettings">This is a class that contains all the parameters for the noise
/// generation.</param>
/// <returns>
/// The domain offset is being returned.
/// </returns>
    public float GenerateDomainNoise(int x, int z, NoiseSettings defaultNoiseSettings)
    {
        var domainOffset = GenerateDomainOffset(x, z);
        return Noise.GetNoise(x + domainOffset.x, z + domainOffset.y, defaultNoiseSettings);
    }

/// <summary>
/// > GenerateDomainOffset() returns a Vector2 that represents the offset of the domain texture for a
/// given x and z coordinate
/// </summary>
/// <param name="x">The x coordinate of the vertex.</param>
/// <param name="z">The z coordinate of the chunk.</param>
/// <returns>
/// A Vector2 with the x and y values of the noise.
/// </returns>
    public Vector2 GenerateDomainOffset(int x, int z)
    {
        var noiseX = Noise.GetNoise(x, z, noiseDomainX) * amplitudeX;
        var noiseY = Noise.GetNoise(x, z, noiseDomainY) * amplitudeY;
        return new Vector2(noiseX, noiseY);
    }
/// <summary>
/// It takes a point in the domain space and returns a point in the range space
/// </summary>
/// <param name="x">The x coordinate of the point to generate the offset for.</param>
/// <param name="z">The z coordinate of the point to generate the offset for.</param>
/// <returns>
/// A Vector2Int.
/// </returns>

    public Vector2Int GenerateDomainOffsetInt(int x, int z)
    {
        return Vector2Int.RoundToInt(GenerateDomainOffset(x, z));
    }
}
