using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* It's a class that contains a bunch of static functions that are used to generate noise */
public static class MyOctavePerlin
{
    
/// <summary>
/// > RemapValue01(value, outputMin, outputMax) = outputMin + (value - 0) * (outputMax - outputMin) / (1
/// - 0)
/// </summary>
/// <param name="value">The value to remap.</param>
/// <param name="outputMin">The minimum value of the output range.</param>
/// <param name="outputMax">The maximum value of the output range.</param>
/// <returns>
/// The value of the outputMin + (value - 0) * (outputMax - outputMin) / (1 - 0)
/// </returns>
    public static float RemapValue(float value, float outputMin, float outputMax)
    {
       // return outputMin + (value - 0) * (outputMax - outputMin) / (1 - 0);
       return outputMin + value  * (outputMax - outputMin);
    }

    public static int RemapValueToInt(float value, float outputMin, float outputMax)
    {
        return (int)RemapValue(value, outputMin, outputMax);
    }

/// <summary>
/// > This function takes a noise value and returns a new value based on the exponent and redistribution
/// modifier values in the settings
/// </summary>
/// <param name="noise">The noise value to be redistributed.</param>
/// <param name="settings">This is a class that contains all the parameters for the noise.</param>
/// <returns>
/// The noise value is being returned.
/// </returns>
    public static float Redistribution(float noise, NoiseSettings settings)
    {
        return Mathf.Pow(noise * settings.redistributionModifier, settings.exponent);
    }

    public static float OctavePerlin(float x, float z, NoiseSettings settings)
    {
        x *= settings.noiseZoom;
        z *= settings.noiseZoom;
        x += settings.noiseZoom;
        z += settings.noiseZoom;

        float noise = 0;
        float frequency = 1;
        float amplitude = 1;
        float amplitudeSum = 0;
        for (var i = 0; i < settings.octaves; i++)
        {
            noise += Mathf.PerlinNoise((settings.offset.x + settings.worldOffset.x + x) * frequency, 
                                        (settings.offset.y + settings.worldOffset.y + z) * frequency) * amplitude;

            amplitudeSum += amplitude;

            amplitude *= settings.persistence;
            frequency *= 2;
        }

        return noise / amplitudeSum;
    }
}
