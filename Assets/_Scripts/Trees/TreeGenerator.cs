using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class TreeGenerator : MonoBehaviour
{
    public NoiseSettings treeNoiseSettings;
    public DomainWarping domainWrapping;


    public TreeData CreateTreeData(Chunk chunk, Vector2Int mapSeedOffset)
    {
        treeNoiseSettings.terrainOffset = mapSeedOffset;
        TreeData treeData = new TreeData();
        float[,] noiseData = GenerateTreeNoise(chunk, treeNoiseSettings);
        treeData.treePositions = FindLocalTreePositions(noiseData, chunk.terrainPosition.x, chunk.terrainPosition.z);
        return treeData;
    }

    private float[,] GenerateTreeNoise(Chunk chunk, NoiseSettings treeNoiseSettings)
    {
        float[,] noiseMax = new float[chunk.chunkSize, chunk.chunkSize];
        int xMax = chunk.terrainPosition.x + chunk.chunkSize;
        int xMin = chunk.terrainPosition.x;
        int zMax = chunk.terrainPosition.z + chunk.chunkSize;
        int zMin = chunk.terrainPosition.z;
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

    public static List<Vector2Int> FindLocalTreePositions(float[,] dataMatrix, int xCoord, int zCoord)
    {
        List<Vector2Int> positions = new List<Vector2Int>();
        for (int x = 0; x < dataMatrix.GetLength(0); x++)
        {
            for (int y = 0; y < dataMatrix.GetLength(1); y++)
            {
                var noiseVal = dataMatrix[x, y];
                if (CheckNeighbours(dataMatrix, x, y, (neighbourNoise) => neighbourNoise < noiseVal))
                {
                    positions.Add(new Vector2Int(xCoord + x, zCoord + y));
                }
            }
        }
        return positions;
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