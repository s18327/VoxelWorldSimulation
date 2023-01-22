using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;
using static UnityEngine.Vector3Int;

/* It's a class that manages the game */
public class GameManager : MonoBehaviour
{
    public GameObject gameManager;
    public GameObject playerPrefab;
    private GameObject player;
    public Vector3Int currentChunkPosition;
    private Vector3Int currentChunkCenter = zero;

    public Terrain terrain;

    public float detectionTime = 1;

    public VoxelSo voxel;

    PlayerData playerData;

    public Vector3 spawnPos;
    public Vector3 playerPositions;

    public bool newGame;
    public bool loadGame;

    public bool keyClicked;


/// <summary>
/// The function is called when the game starts. It unlocks the cursor and sets the keyClicked variable
/// to false.
/// </summary>
    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        keyClicked = false;

    }

void Update()
    {
        IsCursorFree();
        Following();
    }


    public void Following()
    {
        PlayerData playerData = GameObject.Find("SaveLoadSystem").GetComponent<DataPersistenceManager>().playerData;
        if (loadGame)
        {
            gameManager.transform.position = playerData.playerPosition;
            loadGame = false;
        }
        else if (newGame)
        {
            this.gameObject.transform.position = new Vector3Int(terrain.chunkSize / 2, 100, terrain.chunkSize / 2);
           // newGame = false;
        }
        else if (player != null)
        {
            gameManager.transform.position = player.transform.position;
        }
    }

    private void IsCursorFree()
    {
        if (!Input.GetKeyDown(KeyCode.Escape)) return;
        if (keyClicked == false)
        {
            Cursor.lockState = CursorLockMode.None;
            keyClicked = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            keyClicked = false;
        }
    }
    
    private void SpawnPlayer()
    {
        Cursor.lockState = CursorLockMode.Locked;
        if (player != null) return;
        
        Vector3Int raycastStartPosition = new Vector3Int(Mathf.FloorToInt(gameManager.transform.position.x), 200, Mathf.FloorToInt(gameManager.transform.position.z));
        if (!Physics.Raycast(raycastStartPosition, Vector3.down, out var hit, 200)) return;
        
        playerData = GameObject.Find("SaveLoadSystem").GetComponent<DataPersistenceManager>().playerData;
        spawnPos = hit.point + up*10;

        if (playerData is null || newGame)
        {
            playerData = new PlayerData
            {
                spawnPos = spawnPos,
                playerPosition = spawnPos
            };
            newGame = false;  //TODO: check if this doesn't break save system and try to clean up
        }

        if (playerData.spawnPos != playerData.playerPosition)
        {
            if (playerData != null)
                player = Instantiate(playerPrefab, playerData.playerPosition, playerData.playerRotation);
        }
        else
        {
            player = Instantiate(playerPrefab, playerData.spawnPos, Quaternion.identity);
        }


        StartCheckingTheMap();

        Toolbar toolbar = GameObject.FindWithTag("Toolbar").GetComponent<Toolbar>();
        toolbar.Create();
    }
    public void IsNewGame()
    {
        newGame = true;
    }

    public void IsLoadGame()
    {
        loadGame = true;
    }

/// <summary>
/// If we're in the Unity Editor, stop playing the game, otherwise quit the application
/// </summary>
    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif

        Application.Quit();
    }
/// <summary>
/// Public void StartCheckingTheMap()
/// 
/// This function is called when the player enters the game. It sets the current chunk coordinates and
/// starts a coroutine that checks if the player has moved to a new chunk.
/// </summary>
    public void StartCheckingTheMap()
    {
        SetCurrentChunkCoordinates();
        StopAllCoroutines();
        StartCoroutine(CheckIfShouldLoadNextPosition());
    }

/// <summary>
/// If the player is outside of the current chunk, load the next chunk
/// </summary>
    private IEnumerator CheckIfShouldLoadNextPosition()
    {
        yield return new WaitForSeconds(detectionTime);
        if (
            Mathf.Abs(currentChunkCenter.x - player.transform.position.x) > terrain.chunkSize ||
            Mathf.Abs(currentChunkCenter.z - player.transform.position.z) > terrain.chunkSize ||
            (Mathf.Abs(currentChunkPosition.y - player.transform.position.y) > terrain.chunkHeight)
            )
        {
            terrain.RequestAdditionalChunkLoad(player);

        }
        else
        {
            StartCoroutine(CheckIfShouldLoadNextPosition());
        }
    }


/// <summary>
/// "Set the current chunk coordinates to the chunk that the player is currently in."
/// 
/// The first line of the function is a call to a function in the TerrainHelper class. This function
/// takes the terrain and the player's position as parameters and returns the chunk coordinates of the
/// chunk that the player is currently in.
/// 
/// The next two lines set the x and z coordinates of the currentChunkCenter variable to the center of
/// the chunk that the player is currently in.
/// </summary>
    private void SetCurrentChunkCoordinates()
    {
        currentChunkPosition = TerrainHelper.GetChunkPositionFromVoxelCoordinates(terrain, RoundToInt(player.transform.position));
        currentChunkCenter.x = currentChunkPosition.x + terrain.chunkSize / 2;
        currentChunkCenter.z = currentChunkPosition.z + terrain.chunkSize / 2;
    }

}
