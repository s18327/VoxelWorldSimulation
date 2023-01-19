using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* The PlayerCamera class is a script that is attached to the camera game object. It allows the player
to look around the scene by moving the mouse */
public class PlayerCamera : MonoBehaviour
{
    [SerializeField]
    private float sensitivity = 300f;
    [SerializeField]
    private Transform playerBody;
    [SerializeField]
    private PlayerInput playerInput;

    float verticalRotation = 0f;


/// <summary>
/// This function is called when the script is first loaded. It gets the PlayerInput component from the
/// parent object
/// </summary>
    private void Awake()
    {
        playerInput = GetComponentInParent<PlayerInput>();
    }

/// <summary>
/// The first thing we do is check if the player is currently using the key. If they are, then we don't
/// want to rotate the player's body or camera.
/// 
/// Next, we get the mouse's position and multiply it by the sensitivity and Time.deltaTime. This will
/// give us the amount of rotation we want to apply to the player's body and camera.
/// 
/// We then rotate the player's body by the amount of mouseX.
/// 
/// Finally, we rotate the camera's vertical rotation by the amount of mouseY. We also clamp the
/// vertical rotation so that the player can't look too far up or down.
/// 
/// If you run the game, you should be able to look around with the mouse.
/// </summary>
    void Update()
{
    if (GameObject.Find("GameManager").GetComponent<GameManager>().keyClicked) return;
    
    float mouseX = playerInput.MousePosition.x * sensitivity * Time.deltaTime;
    float mouseY = playerInput.MousePosition.y * sensitivity * Time.deltaTime;

    verticalRotation -= mouseY;
    verticalRotation = Mathf.Clamp(verticalRotation, -90, 90);

    transform.localRotation = Quaternion.Euler(verticalRotation, 0, 0);
    playerBody.Rotate(Vector3.up * mouseX);
}
}
