using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* It's a class that contains a bunch of static functions that are used to generate noise */
public static class Noise
{
    
/// <summary>
/// > Remaps the value from the original 0.0 - 1.0 range to the new range from 0 to chunk height.
/// </summary>
/// <param name="value">The value to remap.</param>
/// <param name="outputMax">The maximum value of the output range.</param>
/// <returns>
/// The new value
/// </returns>
    public static int RemapValue(float value, float outputMax)
    {
        return (int)(value * outputMax);
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

    public static float GetNoise(float x, float z, NoiseSettings settings)
    {
        x *= settings.noiseZoom;
        z *= settings.noiseZoom;
        x += settings.noiseZoom;
        z += settings.noiseZoom;

        var noise = Mathf.PerlinNoise(settings.offset.x + settings.terrainOffset.x + x, 
                                        settings.offset.y + settings.terrainOffset.y + z);
        
        return noise;
    }
}
