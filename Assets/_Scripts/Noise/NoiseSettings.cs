using UnityEngine;
using UnityEngine.Serialization;

/* It's a class that holds all the settings for the noise generation */
[CreateAssetMenu(fileName = "noiseSettings", menuName = "Data/NoiseSettings")]
public class NoiseSettings : ScriptableObject
{
    public float noiseZoom;
    public int octaves; 
    public Vector2Int offset;
    public Vector2Int terrainOffset;
    public float persistence;
    public float redistributionModifier;
    public float exponent;
}
