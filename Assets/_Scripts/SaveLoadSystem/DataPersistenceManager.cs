using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Newtonsoft.Json;

/* It's a class that saves and loads data from a file */
public class DataPersistenceManager : MonoBehaviour
{
    [Header("File Storage Config")]
    [SerializeField] private string fileNamePlayerData;
    [SerializeField] private string fileNameWorldData;
    [SerializeField] private string fileNameGameData;
    [SerializeField] private bool useEncryption;

    public PlayerData playerData;
    public WorldData worldData;
    public GameData gameData;

    public Dictionary<Vector3Int, ChunkData> loadedChunkDataDictionary = new Dictionary<Vector3Int, ChunkData>();
    private List<IDataPersistence> dataPersistenceObjects;
    private FileDataHandler dataHandler;

    public static DataPersistenceManager instance { get; private set; }

/// <summary>
/// If there is more than one Data Persistence Manager in the scene, throw an error
/// </summary>
    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Found more than one Data Persistence Manager in the scene.");
        }
        instance = this;
    }

/// <summary>
/// It finds all the objects in the scene that have a DataPersistenceObject component attached to them
/// </summary>
    private void Start()
    {
        this.dataHandler = new FileDataHandler(Application.persistentDataPath, fileNamePlayerData, fileNameWorldData, fileNameGameData, useEncryption);
        this.dataPersistenceObjects = FindAllDataPersistenceObjects();
    }


    public void NewGame()
    {
        print("new game");
        PlayerData player = new PlayerData();
        WorldData block = new WorldData();
        GameData game = new GameData();


        this.playerData = new PlayerData();
        this.worldData = new WorldData();
        this.gameData = new GameData();
    }

/// <summary>
/// It loads the game data from a JSON file, and then uses that data to generate the world
/// </summary>
    public void LoadGame()
    {
        print("load game");
        this.playerData = dataHandler.LoadPlayerData();
        this.worldData = dataHandler.LoadWorldData();
        this.gameData = dataHandler.LoadGameData();

        /* It's checking if the data is null, if it is, it creates new data. */
        if (this.playerData == null || this.worldData == null || this.gameData == null)
        {
            Debug.Log("No data was found. Initializing data to defaults.");
            NewGame();
        }

        /* Calling the LoadPlayerData, LoadWorldData, and LoadGameData methods from the IDataPersistence
            interface. */
        foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects)
        {
            dataPersistenceObj.LoadPlayerData(playerData);
            dataPersistenceObj.LoadWorldData(worldData);
            dataPersistenceObj.LoadGameData(gameData);
        }
        //TODO: Debug This
        Dictionary<string, ChunkData> newLoadedChunkDataDictionary = JsonConvert.DeserializeObject<Dictionary<string, ChunkData>>(this.gameData.jsonChunkDataDictionary);
        string[] stringKeys = newLoadedChunkDataDictionary.Keys.ToArray();
        Vector3Int[] keys = new Vector3Int[stringKeys.Length];
        ChunkData[] values = newLoadedChunkDataDictionary.Values.ToArray();

        for (int i = 0; i < stringKeys.Length; i++)
        {
            keys[i] = StringToVector3(stringKeys[i]);
            loadedChunkDataDictionary.Add(keys[i], values[i]);
            loadedChunkDataDictionary[keys[i]].worldReference = GameObject.Find("World").GetComponent<World>();
        }


        GameObject.Find("World").GetComponent<World>().GenerateWorld();
    }

/// <summary>
/// It saves the game data to a file.
/// 
/// The function is called when the player presses the exit button.
/// The function first gets the world object and then gets the game data from it.
/// 
/// The game data is a class that contains all the data that needs to be saved.
/// The game data contains a dictionary that contains all the chunk data.
/// 
/// The chunk data is a class that contains all the data that needs to be saved for each chunk.
/// The chunk data contains a dictionary that contains all the block data.
/// 
/// The block data is a class that contains all the data that needs to be saved for each block.
/// </summary>
    public void SaveGame()
    {
        World world = GameObject.Find("World").GetComponent<World>();
        this.gameData = world.gameData;
        this.gameData.jsonChunkDataDictionary = JsonConvert.SerializeObject(gameData.chunkDataDictionary, Formatting.None);

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        Vector3 playerPos = player.transform.position;
        Quaternion playerRot = player.transform.rotation;
        this.playerData.spawnPos = GameObject.Find("GameManager").GetComponent<GameManager>().spawnPos;
        this.playerData.playerPosition = playerPos;
        this.playerData.playerRotation = playerRot;

        this.worldData.mapSizeInChunk = world.mapSizeInChunks;
        this.worldData.chunkSize = world.chunkSize;
        this.worldData.chunkHeight = world.chunkHeight;
        this.worldData.chunkDrawRange = world.chunkDrawingRange;
        this.worldData.mapSeedOffset =  world.mapSeedOffset;


        foreach (var dataPersistenceObj in dataPersistenceObjects)
        {
            dataPersistenceObj.SavePlayerData(playerData);
            dataPersistenceObj.SaveWorldData(worldData);
            dataPersistenceObj.SaveGameData(gameData);
        }


        dataHandler.SavePlayerData(playerData);
        dataHandler.SaveWorldData(worldData);
        dataHandler.SaveGameData(gameData);
    }

/// <summary>
/// When the application quits, save the game
/// </summary>
    private void OnApplicationQuit()
    {
        SaveGame();
        print("Saved game");
    }

/// <summary>
/// Find all objects in the scene that implement the `IDataPersistence` interface and return them as a list
/// </summary>
/// <returns>
/// A list of all the objects that implement the IDataPersistence interface.
/// </returns>
    private List<IDataPersistence> FindAllDataPersistenceObjects()
    {
        IEnumerable<IDataPersistence> dataPersistenceObjects = FindObjectsOfType<MonoBehaviour>()
            .OfType<IDataPersistence>();

        return new List<IDataPersistence>(dataPersistenceObjects);
    }


/// <summary>
/// It takes a string in the format "(x,y,z)" and returns a Vector3Int with the values of x, y, and z
/// </summary>
/// <param name="sVector">The string to convert to a Vector3Int.</param>
/// <returns>
/// A Vector3Int
/// </returns>
private static Vector3Int StringToVector3(string sVector)
    {
        if (sVector.StartsWith("(") && sVector.EndsWith(")"))
        {
            sVector = sVector.Substring(1, sVector.Length - 2);
        }

        var sArray = sVector.Split(',');

        var result = new Vector3Int(
            int.Parse(sArray[0]),
            int.Parse(sArray[1]),
            int.Parse(sArray[2]));

        return result;
    }
}