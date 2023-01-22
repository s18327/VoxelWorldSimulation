using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public class Terrain : MonoBehaviour
{
    public int chunkSize, chunkHeight;
    public int chunkDrawingRange;
    public Vector2Int mapSeedOffset;

    public bool newGame;
    private bool IsTerrainCreated { get; set; }
    
    public TerrainRenderer terrainRenderer;
    public TerrainData terrainData;
    public TerrainGenerator terrainGenerator;

    private CancellationTokenSource taskSource = new ();
    public UnityEvent OnTerrainCreated;
    public UnityEvent OnNewChunksGenerated;

    private Dictionary<Vector3Int, int> durability = new ();
    
    public void IsNewGame()
    {
        newGame = true;
    }

    public async void GenerateTerrain()
    {
        var persistenceManager = GameObject.Find("SaveLoadSystem").GetComponent<DataPersistenceManager>();
        if (newGame)
        {
            terrainData = new TerrainData
            {
                chunkDataDictionary = new Dictionary<Vector3Int, Chunk>(),
                chunkDictionary = new Dictionary<Vector3Int, ChunkRenderer>()
            };

            await GenerateTerrain(Vector3Int.zero);
        }
        else
        {
            var loadedChunkDataDictionary = persistenceManager.loadedChunkDataDictionary;
            terrainData = new TerrainData
            {
                chunkDataDictionary = loadedChunkDataDictionary,
                chunkDictionary = new Dictionary<Vector3Int, ChunkRenderer>()
            };
            
            chunkSize = persistenceManager.terrainParameters.chunkSize;
            chunkHeight = persistenceManager.terrainParameters.chunkHeight;
            chunkDrawingRange = persistenceManager.terrainParameters.chunkDrawRange;
            mapSeedOffset = persistenceManager.terrainParameters.mapSeedOffset;
            ;
            var persistedPlayerPosition = new Vector3Int(
                Mathf.FloorToInt(persistenceManager.playerData.playerPosition.x),
                Mathf.FloorToInt(persistenceManager.playerData.playerPosition.y),
                Mathf.FloorToInt(persistenceManager.playerData.playerPosition.z));
            await GenerateTerrain(persistedPlayerPosition);
        }

        Cursor.lockState = CursorLockMode.Locked;
    }

    private async Task GenerateTerrain(Vector3Int position)
    {
        terrainGenerator.GenerateBiomePoints(position, chunkDrawingRange, chunkSize, mapSeedOffset);

        TerrainGenerationData Function() => GetVisiblePositionsForThePlayer(position);

        var terrainGenerationData =
            await Task.Run(Function, taskSource.Token);

        foreach (var chunkPositionToRemove in terrainGenerationData.chunkPositionsToRemove)
        {
            TerrainHelper.RemoveChunk(this, chunkPositionToRemove);
        }

        foreach (var chunkDataToRemove in terrainGenerationData.chunkDataToRemove)
        {
            TerrainHelper.RemoveChunkData(this, chunkDataToRemove);
        }


        ConcurrentDictionary<Vector3Int, Chunk> dataDictionary;

        try
        {
            dataDictionary = await CalculateTerrainChunkData(terrainGenerationData.chunkDataPositionsToCreate);
        }
        catch (Exception)
        {
            Debug.Log("Task canceled");
            return;
        }


        foreach (var calculatedData in dataDictionary)
        {
            terrainData.chunkDataDictionary.Add(calculatedData.Key, calculatedData.Value);
        }

        foreach (var chunkData in terrainData.chunkDataDictionary.Values)
        {
            AddTreeLeaves(chunkData);
        }

        ConcurrentDictionary<Vector3Int, Mesh> meshDataDictionary;

        bool Predicate(KeyValuePair<Vector3Int, Chunk> keyValuePair) => terrainGenerationData.chunkPositionsToCreate.Contains(keyValuePair.Key);

        List<Chunk> dataToRender = terrainData.chunkDataDictionary
            .Where(Predicate)
            .Select(keyValuePair => keyValuePair.Value)
            .ToList();

        try
        {
            meshDataDictionary = await CreateMeshDataAsync(dataToRender);
        }
        catch (Exception)
        {
            Debug.Log("Task canceled");
            return;
        }

        StartCoroutine(ChunkCreationCoroutine(meshDataDictionary));
    }

    private static void AddTreeLeaves(Chunk chunk)
    {
        foreach (var treeLeaves in chunk.treeData.treeLeaves)
        {
            ChunkHelper.SetVoxel(chunk, treeLeaves, VoxelType.TreeLeaves);
        }
    }

    private Task<ConcurrentDictionary<Vector3Int, Mesh>> CreateMeshDataAsync(List<Chunk> dataToRender)
    {
        var dictionary = new ConcurrentDictionary<Vector3Int, Mesh>();

        ConcurrentDictionary<Vector3Int, Mesh> Function()
        {
            foreach (var data in dataToRender)
            {
                if (taskSource.Token.IsCancellationRequested)
                {
                    taskSource.Token.ThrowIfCancellationRequested();
                }

                var mesh = ChunkHelper.GetChunkMeshData(data);
                dictionary.TryAdd(data.terrainPosition, mesh);
            }

            return dictionary;
        }

        return Task.Run(Function, taskSource.Token);
    }

    private Task<ConcurrentDictionary<Vector3Int, Chunk>> CalculateTerrainChunkData(
        List<Vector3Int> chunkDataPositionsToCreate)
    {
        var concurrentDictionary = new ConcurrentDictionary<Vector3Int, Chunk>();

        ConcurrentDictionary<Vector3Int, Chunk> Function()
        {
            foreach (var pos in chunkDataPositionsToCreate)
            {
                if (taskSource.Token.IsCancellationRequested)
                {
                    taskSource.Token.ThrowIfCancellationRequested();
                }

                var data = new Chunk(chunkSize, chunkHeight, this, pos);
                var generatedChunk = terrainGenerator.GenerateChunkData(data, mapSeedOffset);

                concurrentDictionary.TryAdd(pos, generatedChunk);
            }

            return concurrentDictionary;
        }

        return Task.Run(Function, taskSource.Token);
    }

    private IEnumerator ChunkCreationCoroutine(ConcurrentDictionary<Vector3Int, Mesh> meshDataDictionary)
    {
        foreach (var item in meshDataDictionary)
        {
            CreateChunk(terrainData, item.Key, item.Value);
            yield return new WaitForEndOfFrame();
        }

        if (IsTerrainCreated) yield break;

        IsTerrainCreated = true;
        OnTerrainCreated?.Invoke();
    }

    private void CreateChunk(TerrainData terrainData, Vector3Int position, Mesh mesh)
    {
        var chunkRenderer = terrainRenderer.RenderChunk(terrainData, position, mesh);
        terrainData.chunkDictionary.Add(position, chunkRenderer);
    }


    internal void SetVoxel(RaycastHit hit, VoxelType voxelType) //TODO:Check this and the Place Voxel Below 
    {
        var chunkRenderer = hit.collider.GetComponent<ChunkRenderer>();
        
        if (chunkRenderer == null) return;

        Vector3Int pos = GetVoxelPositionFromRaycast(hit);

        TerrainHelper.SetNewVoxel(chunkRenderer.chunk.terrainReference, pos, voxelType, durability, false);

        chunkRenderer.IsPlayerModified = true;

        if (ChunkHelper.IsOnEdge(chunkRenderer.chunk, pos))
        {
            var neighbourDataList = ChunkHelper.GetEdgeNeighbourChunk(chunkRenderer.chunk, pos);
            foreach (var chunkToUpdate in neighbourDataList.
                         Select(neighbourData => TerrainHelper.
                         GetChunk(neighbourData.terrainReference, neighbourData.terrainPosition)).
                         Where(chunkToUpdate => chunkToUpdate != null))
            {
                chunkToUpdate.UpdateChunk();
            }
        }

        chunkRenderer.UpdateChunk();
       
    }

    internal void PlaceVoxel(RaycastHit hit, VoxelType voxelType)
    {
        var chunkRenderer = hit.collider.GetComponent<ChunkRenderer>();
        if (chunkRenderer == null)
        {
            Debug.Log(chunkRenderer);
            return;
        }

        var position = GetVoxelPositionFromRaycast(hit);
        position = PlaceVoxelInPosition(position, hit);

        TerrainHelper.SetNewVoxel(chunkRenderer.chunk.terrainReference, position, voxelType, durability, true);

        chunkRenderer.IsPlayerModified = true;

        chunkRenderer.UpdateChunk();
    }
    
    private static Vector3Int PlaceVoxelInPosition(Vector3Int hitPosition, RaycastHit hit) //TODO: check if position is in chunk.
    {
        var position = new Vector3(
            hitPosition.x + hit.normal.x,
            hitPosition.y + hit.normal.y,
            hitPosition.z + hit.normal.z
        );
        return Vector3Int.RoundToInt(position);
    }
    
    private static Vector3Int GetVoxelPositionFromRaycast(RaycastHit hit)
    {
        Vector3 position = new Vector3(
            CalculateNormalisedVoxelPosition(hit.point.x, hit.normal.x),
            CalculateNormalisedVoxelPosition(hit.point.y, hit.normal.y),
            CalculateNormalisedVoxelPosition(hit.point.z, hit.normal.z)
        );

        return Vector3Int.RoundToInt(position);
    }
    
    private static float CalculateNormalisedVoxelPosition(float position, float normal)
    {
        const double tolerance = 0.0000000001;
        if (Math.Abs(Mathf.Abs(position % 1) - 0.5f) < tolerance)
        {
            position -= (normal / 2);
        }

        return position;
    }

    private TerrainGenerationData GetVisiblePositionsForThePlayer(Vector3Int playerPosition)
    {
        var requiredChunkPositions = TerrainHelper.GetChunkPositionsAroundPlayer(this, playerPosition);

        var requiredChunkDataPositions =
            TerrainHelper.GetDataPositionsAroundPlayer(this, playerPosition);
        
        return new TerrainGenerationData
        {
            chunkPositionsToCreate = TerrainHelper.SelectPositionsToCreate(terrainData, requiredChunkPositions, playerPosition),
            chunkDataPositionsToCreate =  TerrainHelper.SelectDataPositionsToCreate(terrainData, requiredChunkDataPositions, playerPosition),
            chunkPositionsToRemove = TerrainHelper.GetUnnecessaryChunks(terrainData, requiredChunkPositions),
            chunkDataToRemove = TerrainHelper.GetUnnecessaryData(terrainData, requiredChunkDataPositions),
        };
    }



    internal VoxelType GetVoxelFromCoordinates(int x, int y, int z)
    {
        var chunkPositionFromVoxelCoordinates = ChunkHelper.ChunkPositionFromVoxelCoordinates(this, x, y, z);

        terrainData.chunkDataDictionary.TryGetValue(chunkPositionFromVoxelCoordinates, out var containerChunk);

        if (containerChunk == null) return VoxelType.Nothing;
        
        var voxelPosInChunkCoordinates =
                ChunkHelper.GetVoxelPosInChunkCoordinates(containerChunk, new Vector3Int(x, y, z));
        
        return ChunkHelper.GetVoxelFromChunkCoordinates(containerChunk, voxelPosInChunkCoordinates);
    }
    
    internal async void RequestAdditionalChunkLoad(GameObject player)
    {
        Debug.Log("Load more chunks");
        await GenerateTerrain(Vector3Int.RoundToInt(player.transform.position));
        OnNewChunksGenerated?.Invoke();
    }
    
    public void OnDisable()
    {
        taskSource.Cancel();
    }
}