using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

/* It's a class that holds a dictionary of all the voxel types and their corresponding texture
data */
public class VoxelDataManager : MonoBehaviour
{
    public static float tileOffset = 0.001f;
    public static float tileDimX, tileDimY;
    public static Dictionary<VoxelType, VoxelData> voxelDataDictionary = new();
    public static Dictionary<VoxelType, VoxelData> selectableVoxelList = new();

    public VoxelSo voxelData;

    /// <summary>
    /// It takes the data from the textureData scriptable object and puts it into a dictionary.
    /// </summary>
    private void Awake()
    {
        foreach (var voxel in voxelData.voxelDataList)
        {
            if (voxelDataDictionary.ContainsKey(voxel.voxelType) == false)
                voxelDataDictionary.Add(voxel.voxelType, voxel);

            if (selectableVoxelList.ContainsKey(voxel.voxelType) == false && voxel.isPlaceable.Equals(true))
                selectableVoxelList.Add(voxel.voxelType, voxel);
        }

        tileDimX = voxelData.textureSizeX;
        tileDimY = voxelData.textureSizeY;
    }
}