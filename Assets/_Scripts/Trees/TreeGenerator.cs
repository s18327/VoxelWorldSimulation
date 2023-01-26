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
        var treeData = new TreeData();
        var noiseData = GenerateTreeNoise(chunk, treeNoiseSettings);
        treeData.treePositions = FindLocalTreePositions(noiseData, chunk.terrainPosition.x, chunk.terrainPosition.z);
        return treeData;
    }

    private float[,] GenerateTreeNoise(Chunk chunk, NoiseSettings treeNoiseSettings)
    {
        var noiseMax = new float[chunk.chunkSize, chunk.chunkSize];
        var xMax = chunk.terrainPosition.x + chunk.chunkSize;
        var xMin = chunk.terrainPosition.x;
        var zMax = chunk.terrainPosition.z + chunk.chunkSize;
        var zMin = chunk.terrainPosition.z;
        int xIndex = 0, zIndex = 0;
        for (var x = xMin; x < xMax; x++)
        {
            for (var z = zMin; z < zMax; z++)
            {
                noiseMax[xIndex, zIndex] = domainWrapping.GenerateDomainNoise(x, z, treeNoiseSettings);
                zIndex++;
            }

            xIndex++;
            zIndex = 0;
        }

        return noiseMax;
    }

    private static List<Vector2Int> FindLocalTreePositions(float[,] dataMatrix, int xCoord, int zCoord)
    {
        var positions = new List<Vector2Int>();
        for (var x = 0; x < dataMatrix.GetLength(0); x++)
        {
            for (var y = 0; y < dataMatrix.GetLength(1); y++)
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
        return (from direction in Directions.Directions2D
            let newPosition = new Vector2Int(x + direction.x, y + direction.y)
            where newPosition.x >= 0 && newPosition.x < dataMatrix.GetLength(0) && newPosition.y >= 0 &&
                  newPosition.y < dataMatrix.GetLength(1)
            select direction).All(direction => successCondition(dataMatrix[x + direction.x, y + direction.y]));
    }
}