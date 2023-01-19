using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

public class World : MonoBehaviour
{
    public int mapSizeInChunks;
    public int chunkSize, chunkHeight;
    public int chunkDrawingRange;
    public Vector2Int mapSeedOffset;

    public bool newGame = false;
    private bool IsWorldCreated { get; set; }

    public GameObject chunkPrefab;
    public WorldRenderer worldRenderer;
    public GameData gameData;
    public TerrainGenerator terrainGenerator;

    CancellationTokenSource taskTokenSource = new CancellationTokenSource();
    public UnityEvent OnWorldCreated, OnNewChunksGenerated;

    private Dictionary<Vector3Int, int> durability = new Dictionary<Vector3Int, int>();


    //private void Awake()
    //{
    //    mapSizeInChunks = 0;
    //    chunkSize = 0;
    //    chunkHeight = 0;
    //    chunkDrawingRange = 0;
    //    mapSeedOffset = Vector2Int.zero;
    //}

    //private void Start() //TODO: implement
    //
    //{
    //    mapSizeInChunks = _Scripts.SetParameters.SettingsService.Instance.mapSizeInChunks;
    //}

    public void IsNewGame()
    {
        newGame = true;
    }

    public async void GenerateWorld()
    {
        var persistenceManager = GameObject.Find("SaveLoadSystem").GetComponent<DataPersistenceManager>();
        if (newGame)
        {
            gameData = new GameData
            {
                chunkHeight = chunkHeight,
                chunkSize = chunkSize,
                chunkDataDictionary = new Dictionary<Vector3Int, ChunkData>(),
                chunkDictionary = new Dictionary<Vector3Int, ChunkRenderer>()
            };

            await GenerateWorld(Vector3Int.zero);
        }
        else
        {
            var loadedChunkDataDictionary = persistenceManager.loadedChunkDataDictionary;
            gameData = new GameData
            {
                chunkHeight = chunkHeight,
                chunkSize = chunkSize,
                chunkDataDictionary = loadedChunkDataDictionary,
                chunkDictionary = new Dictionary<Vector3Int, ChunkRenderer>()
            };
            mapSizeInChunks = persistenceManager.worldData.mapSizeInChunk;
            chunkSize = persistenceManager.worldData.chunkSize;
            chunkHeight = persistenceManager.worldData.chunkHeight;
            chunkDrawingRange = persistenceManager.worldData.chunkDrawRange;
            mapSeedOffset = persistenceManager.worldData.mapSeedOffset;
            ;
            var persistedPlayerPosition = new Vector3Int(
                Mathf.FloorToInt(persistenceManager.playerData.playerPosition.x),
                Mathf.FloorToInt(persistenceManager.playerData.playerPosition.y),
                Mathf.FloorToInt(persistenceManager.playerData.playerPosition.z));
            await GenerateWorld(persistedPlayerPosition);
        }

        Cursor.lockState = CursorLockMode.Locked;
    }

    private async Task GenerateWorld(Vector3Int position)
    {
        terrainGenerator.GenerateBiomePoints(position, chunkDrawingRange, chunkSize, mapSeedOffset);

        var worldGenerationData =
            await Task.Run(() => GetVisiblePositionsForThePlayer(position), taskTokenSource.Token);

        foreach (var chunkPositionToRemove in worldGenerationData.chunkPositionsToRemove)
        {
            WorldDataHelper.RemoveChunk(this, chunkPositionToRemove);
        }

        foreach (var chunkDataToRemove in worldGenerationData.chunkDataToRemove)
        {
            WorldDataHelper.RemoveChunkData(this, chunkDataToRemove);
        }


        ConcurrentDictionary<Vector3Int, ChunkData> dataDictionary = null;

        try
        {
            dataDictionary = await CalculateWorldChunkData(worldGenerationData.chunkDataPositionsToCreate);
        }
        catch (Exception)
        {
            Debug.Log("Task canceled");
            return;
        }


        foreach (var calculatedData in dataDictionary)
        {
            this.gameData.chunkDataDictionary.Add(calculatedData.Key, calculatedData.Value);
        }

        foreach (var chunkData in this.gameData.chunkDataDictionary.Values)
        {
            AddTreeLeafs(chunkData);
        }

        ConcurrentDictionary<Vector3Int, MeshData> meshDataDictionary;

        List<ChunkData> dataToRender = this.gameData.chunkDataDictionary
            .Where(keyvaluepair => worldGenerationData.chunkPositionsToCreate.Contains(keyvaluepair.Key))
            .Select(keyvalpair => keyvalpair.Value)
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

    private void AddTreeLeafs(ChunkData chunkData)
    {
        foreach (var treeLeaves in chunkData.treeData.treeLeaves)
        {
            Chunk.SetBlock(chunkData, treeLeaves, VoxelType.TreeLeaves);
        }
    }

    private Task<ConcurrentDictionary<Vector3Int, MeshData>> CreateMeshDataAsync(List<ChunkData> dataToRender)
    {
        ConcurrentDictionary<Vector3Int, MeshData> dictionary = new ConcurrentDictionary<Vector3Int, MeshData>();
        return Task.Run(() =>
            {
                foreach (ChunkData data in dataToRender)
                {
                    if (taskTokenSource.Token.IsCancellationRequested)
                    {
                        taskTokenSource.Token.ThrowIfCancellationRequested();
                    }

                    MeshData meshData = Chunk.GetChunkMeshData(data);
                    dictionary.TryAdd(data.worldPosition, meshData);
                }

                return dictionary;
            }, taskTokenSource.Token
        );
    }

    private Task<ConcurrentDictionary<Vector3Int, ChunkData>> CalculateWorldChunkData(
        List<Vector3Int> chunkDataPositionsToCreate)
    {
        ConcurrentDictionary<Vector3Int, ChunkData> dictionary = new ConcurrentDictionary<Vector3Int, ChunkData>();
        return Task.Run(() =>
            {
                foreach (Vector3Int pos in chunkDataPositionsToCreate)
                {
                    if (taskTokenSource.Token.IsCancellationRequested)
                    {
                        taskTokenSource.Token.ThrowIfCancellationRequested();
                    }

                    ChunkData data = new ChunkData(chunkSize, chunkHeight, this, pos);
                    ChunkData newData = terrainGenerator.GenerateChunkData(data, mapSeedOffset);

                    dictionary.TryAdd(pos, newData);
                }

                return dictionary;
            },
            taskTokenSource.Token
        );
    }

    private IEnumerator ChunkCreationCoroutine(ConcurrentDictionary<Vector3Int, MeshData> meshDataDictionary)
    {
        foreach (var item in meshDataDictionary)
        {
            CreateChunk(this.gameData, item.Key, item.Value);
            yield return new WaitForEndOfFrame();
        }

        if (IsWorldCreated) yield break;

        IsWorldCreated = true;
        OnWorldCreated?.Invoke();
    }

    private void CreateChunk(GameData gameData, Vector3Int position, MeshData meshData)
    {
        var chunkRenderer = worldRenderer.RenderChunk(gameData, position, meshData);
        gameData.chunkDictionary.Add(position, chunkRenderer);
    }


    internal bool SetBlock(RaycastHit hit, VoxelType voxelType) //TODO:Check this and the Place Block Below 
    {
        ChunkRenderer chunk = hit.collider.GetComponent<ChunkRenderer>();
        if (chunk == null)
            return false;

        Vector3Int pos = GetBlockPositionFromRaycast(hit);

        WorldDataHelper.SetNewBlock(chunk.ChunkData.worldReference, pos, voxelType, durability, false);

        chunk.ModifiedByThePlayer = true;

        if (Chunk.IsOnEdge(chunk.ChunkData, pos))
        {
            List<ChunkData> neighbourDataList = Chunk.GetEdgeNeighbourChunk(chunk.ChunkData, pos);
            foreach (ChunkData neighbourData in neighbourDataList)
            {
                ChunkRenderer chunkToUpdate =
                    WorldDataHelper.GetChunk(neighbourData.worldReference, neighbourData.worldPosition);
                if (chunkToUpdate != null)
                    chunkToUpdate.UpdateChunk();
            }
        }

        chunk.UpdateChunk();
        return true;
    }

    internal bool PlaceBlock(RaycastHit hit, VoxelType voxelType)
    {
        ChunkRenderer chunkRenderer = hit.collider.GetComponent<ChunkRenderer>();
        if (chunkRenderer == null)
        {
            Debug.Log(chunkRenderer);
            return false;
        }

        Vector3Int position = GetBlockPositionFromRaycast(hit);
        position = PlaceBlockInPosition(position, hit);

        WorldDataHelper.SetNewBlock(chunkRenderer.ChunkData.worldReference, position, voxelType, durability, true);

        chunkRenderer.ModifiedByThePlayer = true;

        chunkRenderer.UpdateChunk();
        return true;
    }

    private Vector3Int GetBlockPositionFromRaycast(RaycastHit hit)
    {
        Vector3 position = new Vector3(
            CalculateNormalisedBlockPosition(hit.point.x, hit.normal.x),
            CalculateNormalisedBlockPosition(hit.point.y, hit.normal.y),
            CalculateNormalisedBlockPosition(hit.point.z, hit.normal.z)
        );

        return Vector3Int.RoundToInt(position);
    }

    private Vector3Int
        PlaceBlockInPosition(Vector3Int hitPosition, RaycastHit hit) //TODO: check if position is in chunk.
    {
        Vector3 position = new Vector3(
            hitPosition.x + hit.normal.x,
            hitPosition.y + hit.normal.y,
            hitPosition.z + hit.normal.z
        );
        return Vector3Int.RoundToInt(position);
    }

    private float CalculateNormalisedBlockPosition(float pos, float normal)
    {
        const double tolerance = 0.0000000001;
        if (Math.Abs(Mathf.Abs(pos % 1) - 0.5f) < tolerance)
        {
            pos -= (normal / 2);
        }

        return pos;
    }

    private WorldGenerationData GetVisiblePositionsForThePlayer(Vector3Int playerPosition)
    {
        List<Vector3Int> requiredChunkPositions = WorldDataHelper.GetChunkPositionsAroundPlayer(this, playerPosition);

        List<Vector3Int> requiredChunkDataPositions =
            WorldDataHelper.GetDataPositionsAroundPlayer(this, playerPosition);

        List<Vector3Int> chunkPositionsToCreate =
            WorldDataHelper.SelectPositionsToCreate(this.gameData, requiredChunkPositions, playerPosition);
        List<Vector3Int> chunkDataPositionsToCreate =
            WorldDataHelper.SelectDataPositionsToCreate(this.gameData, requiredChunkDataPositions, playerPosition);

        List<Vector3Int> chunkPositionsToRemove =
            WorldDataHelper.GetUnnecessaryChunks(this.gameData, requiredChunkPositions);
        List<Vector3Int> chunkDataToRemove =
            WorldDataHelper.GetUnnecessaryData(this.gameData, requiredChunkDataPositions);

        var data = new WorldGenerationData
        {
            chunkPositionsToCreate = chunkPositionsToCreate,
            chunkDataPositionsToCreate = chunkDataPositionsToCreate,
            chunkPositionsToRemove = chunkPositionsToRemove,
            chunkDataToRemove = chunkDataToRemove,
            chunkPositionsToUpdate = new List<Vector3Int>()
        };
        return data;
    }

    internal async void RequestAdditionalChunkLoad(GameObject player)
    {
        Debug.Log("Load more chunks");
        await GenerateWorld(Vector3Int.RoundToInt(player.transform.position));
        OnNewChunksGenerated?.Invoke();
    }

    internal VoxelType GetBlockFromChunkCoordinates(ChunkData chunkData, int x, int y, int z)
    {
        Vector3Int pos = Chunk.ChunkPositionFromBlockCoords(this, x, y, z);

        gameData.chunkDataDictionary.TryGetValue(pos, out var containerChunk);

        if (containerChunk == null)
            return VoxelType.Nothing;
        Vector3Int blockFromChunkCoordinates =
            Chunk.GetVoxelInChunkCoordinates(containerChunk, new Vector3Int(x, y, z));
        return Chunk.GetBlockFromChunkCoordinates(containerChunk, blockFromChunkCoordinates);
    }

    public void OnDisable()
    {
        taskTokenSource.Cancel();
    }
}