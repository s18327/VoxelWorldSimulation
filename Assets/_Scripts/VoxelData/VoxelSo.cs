using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

/* This class is a ScriptableObject that holds a list of TextureData objects. */
[CreateAssetMenu(fileName = "Block Data", menuName = "Data/Block Data")]
public class VoxelSo : ScriptableObject
{
    public float textureSizeX, textureSizeY;
    public List<VoxelData> voxelDataList;
}