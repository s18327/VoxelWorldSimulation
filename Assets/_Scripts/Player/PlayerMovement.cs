using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* It handles the movement of the player */
public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    private CharacterController controller;

    [SerializeField]
    private float playerSpeed = 5.0f, playerRunSpeed = 8;
    [SerializeField]
    private float jumpHeight = 1.0f;
    [SerializeField]
    private float gravityValue = -9.81f;
    [SerializeField]
    private float flySpeed = 2;

    private Vector3 playerVelocity;

    [Header("Grounded check parameters:")]
    [SerializeField]
    private LayerMask groundMask;
    [SerializeField]
    private float rayDistance = 1;
    [field: SerializeField]
    public bool IsGrounded { get; private set; }



    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }


/// <summary>
/// "Get the direction of movement based on the input from the player."
/// 
/// The function takes in a Vector3 called movementInput. This is the input from the player. It's a
/// Vector3 because it has an x, y, and z component. The x component is the horizontal input, the y
/// component is the vertical input, and the z component is the depth input
/// </summary>
/// <param name="movementInput">A 3D vector.</param>
/// <returns>
/// The direction of the movement.
/// </returns>
    private Vector3 GetMovementDirection(Vector3 movementInput)
{
    var transform1 = transform;
    return transform1.right * movementInput.x + transform1.forward * movementInput.z;
}

/// <summary>
/// If the player is pressing the ascend button, add the up vector to the movement direction, otherwise
/// if the player is pressing the descend button, subtract the up vector from the movement direction
/// </summary>
/// <param name="movementInput">movementInput</param>
/// <param name="ascendInput">A boolean that is true when the player is pressing the ascend
/// button.</param>
/// <param name="descendInput">A boolean that is true when the player is pressing the descend
/// button.</param>
    public void Fly(Vector3 movementInput, bool ascendInput, bool descendInput)
    {
        Vector3 movementDirection = GetMovementDirection(movementInput);

        if (ascendInput)
        {
            movementDirection += Vector3.up * flySpeed;
        }
        else if (descendInput)
        {
            movementDirection -= Vector3.up * flySpeed;
        }
        controller.Move(movementDirection * (playerRunSpeed * Time.deltaTime));
    }

/// <summary>
/// It takes in a Vector3 and a boolean, and returns a Vector3.
/// 
/// The Vector3 is the direction the player is moving in, and the boolean is whether or not the player
/// is running.
/// 
/// The function then returns a Vector3 that is the direction the player is moving in.
/// 
/// The function also moves the player in that direction.
/// 
/// The function also takes into account the speed of the player.
/// 
/// The function also takes into account the speed of the player when running.
/// 
/// The function also takes into account the time since the last frame.
/// 
/// The function also takes into account the time since the last frame when running.
/// 
/// The function also takes into account the time since the last frame when running and moving.
/// 
/// The function also takes into account the time since the last frame when running and moving and the
/// speed of the
/// </summary>
/// <param name="Vector3">movementInput</param>
/// <param name="runningInput">A boolean that is true if the player is running, false if they are
/// walking.</param>
    public void Walk(Vector3 movementInput, bool runningInput)
    {
        Vector3 movementDirection = GetMovementDirection(movementInput);
        float speed = runningInput ? playerRunSpeed : playerSpeed;
        controller.Move(movementDirection * (Time.deltaTime * speed));
    }

    public void HandleGravity(bool isJumping)
    {
        if (controller.isGrounded && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
        }
        if (isJumping && IsGrounded)
            AddJumpForce();
        ApplyGravityForce();
        controller.Move(playerVelocity * Time.deltaTime);
    }

    private void AddJumpForce()
    {
        playerVelocity.y = jumpHeight;
    }

    private void ApplyGravityForce()
    {
        playerVelocity.y += gravityValue * Time.deltaTime;
        playerVelocity.y = Mathf.Clamp(playerVelocity.y, gravityValue, 10);
    }

    private void Update()
    {
        IsGrounded = Physics.Raycast(transform.position, Vector3.down, rayDistance, groundMask);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawRay(transform.position, Vector3.down * rayDistance);
    }
}
