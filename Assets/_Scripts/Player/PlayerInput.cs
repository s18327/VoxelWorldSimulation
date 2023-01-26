using System;
using UnityEngine;

/* It's a class that gets input from the player and then sends it to the other classes that need it */
public class PlayerInput : MonoBehaviour
{
    public event Action OnLeftMouseClick, OnRightMouseClick, OnFly;
    public bool RunningPressed { get; private set; }
    public Vector3 MovementInput { get; private set; }
    public Vector2 MousePosition { get; private set; }
    public bool IsJumping { get; private set; }

    public int VoxelTypeNumberSelected { get; set; }

/// <summary>
/// This function is called every frame and updates the values of the input variables
/// </summary>
private void Update()
    {
        GetLeftMouseClick();
        GetRightMouseClick();
        GetMousePosition();
        GetMovementInput();
        GetJumpInput();
        GetRunInput();
        GetFlyInput();
    }

/// <summary>
/// If the V key is pressed, then invoke the OnFly event
/// </summary>
    private void GetFlyInput()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            OnFly?.Invoke();
        }
    }

    private void GetRunInput()
    {
        RunningPressed = Input.GetKey(KeyCode.LeftShift);
    }

    private void GetJumpInput()
    {
        IsJumping = Input.GetButton("Jump");
    }

    private void GetMovementInput()
    {
        MovementInput = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
    }

    private void GetMousePosition()
    {
        MousePosition = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
    }

    private void GetLeftMouseClick()
    {
        if (Input.GetMouseButtonDown(0)) OnLeftMouseClick?.Invoke();

    }
    private void GetRightMouseClick()
    {
        if (Input.GetMouseButtonDown(1)) OnRightMouseClick?.Invoke();

    }
}