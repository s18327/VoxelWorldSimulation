using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/* It's a class that handles the player's movement and interaction with the world. */
public class Character : MonoBehaviour
{
    [SerializeField]
    private Camera mainCamera;
    [SerializeField]
    private PlayerInput playerInput;
    [SerializeField]
    private PlayerMovement playerMovement;

    public float interactionRayLength = 5;

    public LayerMask groundMask;

    public bool fly;

    bool isWaiting;

    public World world;
    public string currentBlock;


/// <summary>
/// If the main camera is null, set it to the main camera. Then, get the player input and player
/// movement components, and find the world object
/// </summary>
    private void Awake()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;
        playerInput = GetComponent<PlayerInput>();
        playerMovement = GetComponent<PlayerMovement>();
        world = FindObjectOfType<World>();
    }
/// <summary>
/// "When the player clicks the left mouse button, call the HandleLeftMouseClick function."
/// 
/// The same thing happens for the right mouse button and the fly button.
/// </summary>
    private void Start()
    {
        playerInput.OnLeftMouseClick += HandleLeftMouseClick;
        playerInput.OnRightMouseClick += HandleRightMouseClick;
        playerInput.OnFly += HandleFlyClick;
    }

/// <summary>
/// If the player is flying, stop flying. If the player is not flying, start flying
/// </summary>
    private void HandleFlyClick()
    {
        fly = !fly;
    }

    void Update()
    {
       /* It's getting the current block that the player has selected. */
        currentBlock = VoxelDataManager.selectablevoxelList.ElementAt(playerInput.BlockTypeNumberSelected).Key.ToString();
        if (fly)
        {
            playerMovement.Fly(playerInput.MovementInput, playerInput.IsJumping, playerInput.RunningPressed);
        }
        else
        {

            if (playerMovement.IsGrounded && playerInput.IsJumping && isWaiting == false)
            {

                isWaiting = true;
                StopAllCoroutines();
                StartCoroutine(ResetWaiting());
            }

            playerMovement.HandleGravity(playerInput.IsJumping);
            playerMovement.Walk(playerInput.MovementInput, playerInput.RunningPressed);
        }
    }
    IEnumerator ResetWaiting()
    {
        yield return new WaitForSeconds(0.1f);
        isWaiting = false;
    }

/// <summary>
/// If the player clicks on the ground, modify the terrain at the point of impact.
/// 
/// The first thing we do is create a ray that starts at the camera's position and points in the
/// direction the camera is facing. We then use the `Physics.Raycast` function to check if the ray hits
/// anything. If it does, we call the `ModifyTerrain` function and pass it the `RaycastHit` object.
/// </summary>
    private void HandleLeftMouseClick()
    {
        Ray playerRay = new Ray(mainCamera.transform.position, mainCamera.transform.forward);
        RaycastHit hit;
        if (Physics.Raycast(playerRay, out hit, interactionRayLength, groundMask))
        {
            ModifyTerrain(hit);
        }
    }
/// <summary>
/// If the player right clicks, and the raycast hits the ground, place a block
/// </summary>
    private void HandleRightMouseClick()
    {
        Ray playerRay = new Ray(mainCamera.transform.position, mainCamera.transform.forward);
        if (!Physics.Raycast(playerRay, out var hit, interactionRayLength, groundMask)) return;
        PlaceBlock(hit);
    }

    private void ModifyTerrain(RaycastHit hit)
    {
        world.SetBlock(hit, VoxelType.Air);
    }

/// <summary>
/// > This function takes the block type number selected by the player and uses it to place a block of
/// the corresponding type at the location of the raycast hit
/// </summary>
/// <param name="hit">The raycast hit that was returned from the raycast.</param>
    private void PlaceBlock(RaycastHit hit)
    {
        world.PlaceBlock(hit, VoxelDataManager.selectablevoxelList.ElementAt(playerInput.BlockTypeNumberSelected).Key);
    }
}
