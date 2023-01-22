using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine.Serialization;

/* It's a class that saves and loads data from a file */
public class DataPersistenceManager : MonoBehaviour
{
    [Header("File Storage Config")]
    [SerializeField] private string fileNamePlayerData;
    [SerializeField] private string fileNameTerrainParameters;
    [SerializeField] private string fileNameTerrainData;
    [SerializeField] private bool useEncryption;

    public PlayerData playerData;
    public TerrainParameters terrainParameters;
    public TerrainData terrainData;

    public Dictionary<Vector3Int, Chunk> loadedChunkDataDictionary = new ();
    private List<IDataPersistence> dataPersistenceObjects;
    private FileDataHandler dataHandler;

    private static DataPersistenceManager Instance { get; set; }

/// <summary>
/// If there is more than one Data Persistence Manager in the scene, throw an error
/// </summary>
    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("Found more than one Data Persistence Manager in the scene.");
        }
        Instance = this;
    }

/// <summary>
/// It finds all the objects in the scene that have a DataPersistenceObject component attached to them
/// </summary>
    private void Start()
    {
        dataHandler = new FileDataHandler(Application.persistentDataPath, fileNamePlayerData, fileNameTerrainParameters, fileNameTerrainData, useEncryption);
        dataPersistenceObjects = FindAllDataPersistenceObjects();
    }


    public void NewGame()
    {
        print("new game");

        playerData = new PlayerData();
        terrainParameters = new TerrainParameters();
        terrainData = new TerrainData();
    }

/// <summary>
/// It loads the game data from a JSON file, and then uses that data to generate the terrain
/// </summary>
    public void LoadGame()
    {
        print("load game");
        playerData = dataHandler.LoadPlayerData();
        terrainParameters = dataHandler.LoadTerrainParameters();
        terrainData = dataHandler.LoadTerrainData();

        /* It's checking if the data is null, if it is, it creates new data. */
        if (playerData == null || terrainParameters == null || terrainData == null)
        {
            Debug.Log("No data was found. Initializing data to defaults.");
            NewGame();
        }

        /* Calling the LoadPlayerData, LoadTerrainParameters, and LoadTerrainData methods from the IDataPersistence
            interface. */
        foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects)
        {
            dataPersistenceObj.LoadPlayerData(playerData);
            dataPersistenceObj.LoadTerrainParameters(terrainParameters);
            dataPersistenceObj.LoadTerrainData(terrainData);
        }
        Dictionary<string, Chunk> newLoadedChunkDataDictionary = JsonConvert.DeserializeObject<Dictionary<string, Chunk>>(terrainData.jsonChunkDataDictionary);
        string[] stringKeys = newLoadedChunkDataDictionary.Keys.ToArray();
        Vector3Int[] keys = new Vector3Int[stringKeys.Length];
        Chunk[] values = newLoadedChunkDataDictionary.Values.ToArray();

        for (int i = 0; i < stringKeys.Length; i++)
        {
            keys[i] = StringToVector3(stringKeys[i]);
            loadedChunkDataDictionary.Add(keys[i], values[i]);
            loadedChunkDataDictionary[keys[i]].terrainReference = GameObject.Find("Terrain").GetComponent<Terrain>();
        }


        GameObject.Find("Terrain").GetComponent<Terrain>().GenerateTerrain();
    }

/// <summary>
/// It saves the game data to a file.
/// 
/// The function is called when the player presses the exit button.
/// The function first gets the terrain object and then gets the game data from it.
/// 
/// The game data is a class that contains all the data that needs to be saved.
/// The game data contains a dictionary that contains all the chunk data.
/// 
/// The chunk data is a class that contains all the data that needs to be saved for each chunk.
/// The chunk data contains a dictionary that contains all the voxel data.
/// 
/// The voxel data is a class that contains all the data that needs to be saved for each voxel.
/// </summary>
    public void SaveGame()
    {
        Terrain terrain = GameObject.Find("Terrain").GetComponent<Terrain>();
        terrainData = terrain.terrainData;
        terrainData.jsonChunkDataDictionary = JsonConvert.SerializeObject(terrainData.chunkDataDictionary, Formatting.None);

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        Vector3 playerPos = player.transform.position;
        Quaternion playerRot = player.transform.rotation;
        playerData.spawnPos = GameObject.Find("Game Manager").GetComponent<GameManager>().spawnPos;
        playerData.playerPosition = playerPos;
        playerData.playerRotation = playerRot;

        
        terrainParameters.chunkSize = terrain.chunkSize;
        terrainParameters.chunkHeight = terrain.chunkHeight;
        terrainParameters.chunkDrawRange = terrain.chunkDrawingRange;
        terrainParameters.mapSeedOffset =  terrain.mapSeedOffset;


        foreach (var dataPersistenceObj in dataPersistenceObjects)
        {
            dataPersistenceObj.SavePlayerData(playerData);
            dataPersistenceObj.SaveTerrainParameters(terrainParameters);
            dataPersistenceObj.SaveTerrainData(terrainData);
        }
        
        dataHandler.SavePlayerData(playerData);
        dataHandler.SaveTerrainParameters(terrainParameters);
        dataHandler.SaveTerrainData(terrainData);
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