using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class TreeGenerator : MonoBehaviour
{
    public NoiseSettings treeNoiseSettings;
    public DomainWarping domainWrapping;


    public TreeData GenerateTreeData(ChunkData chunkData, Vector2Int mapSeedOffset)
    {
        treeNoiseSettings.worldOffset = mapSeedOffset;
        TreeData treeData = new TreeData();
        float[,] noiseData = GenerateTreeNoise(chunkData, treeNoiseSettings);
        treeData.treePositions = FindLocalMaxima(noiseData, chunkData.worldPosition.x, chunkData.worldPosition.z);
        return treeData;
    }

    private float[,] GenerateTreeNoise(ChunkData chunkData, NoiseSettings treeNoiseSettings)
    {
        float[,] noiseMax = new float[chunkData.chunkSize, chunkData.chunkSize];
        int xMax = chunkData.worldPosition.x + chunkData.chunkSize;
        int xMin = chunkData.worldPosition.x;
        int zMax = chunkData.worldPosition.z + chunkData.chunkSize;
        int zMin = chunkData.worldPosition.z;
        int xIndex = 0, zIndex = 0;
        for (int x = xMin; x < xMax; x++)
        {
            for (int z = zMin; z < zMax; z++)
            {
                noiseMax[xIndex, zIndex] = domainWrapping.GenerateDomainNoise(x, z, treeNoiseSettings);
                zIndex++;
            }
            xIndex++;
            zIndex = 0;
        }
        return noiseMax;
    }

    public static List<Vector2Int> FindLocalMaxima(float[,] dataMatrix, int xCoord, int zCoord)
    {
        List<Vector2Int> maximas = new List<Vector2Int>();
        for (int x = 0; x < dataMatrix.GetLength(0); x++)
        {
            for (int y = 0; y < dataMatrix.GetLength(1); y++)
            {
                float noiseVal = dataMatrix[x, y];
                if (CheckNeighbours(dataMatrix, x, y, (neighbourNoise) => neighbourNoise < noiseVal))
                {
                    maximas.Add(new Vector2Int(xCoord + x, zCoord + y));
                }

            }
        }
        return maximas;
    }

    private static bool CheckNeighbours(float[,] dataMatrix, int x, int y, Func<float, bool> successCondition)
    {
        return (Directions.Directions2D
            .Select(direction => new { direction, newPosition = new Vector2Int(x + direction.x, y + direction.y) })
            .Where(t => t.newPosition.x >= 0 && t.newPosition.x < dataMatrix.GetLength(0) && t.newPosition.y >= 0 &&
                        t.newPosition.y < dataMatrix.GetLength(1))
            .Select(t => t.direction)).All(direction => successCondition(dataMatrix[x + direction.x, y + direction.y]));
    }
}