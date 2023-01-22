using System.Collections.Generic;
using UnityEngine;

/* This class is a ScriptableObject that holds a list of TextureData objects. */
[CreateAssetMenu(fileName = "Voxel Data", menuName = "Data/Voxel Data")]
public class VoxelSo : ScriptableObject
{
    public float textureSizeX, textureSizeY;
    public List<VoxelData> voxelDataList;
}