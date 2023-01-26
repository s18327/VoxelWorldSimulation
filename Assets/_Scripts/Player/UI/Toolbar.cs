using UnityEngine;
using UnityEngine.UI;

public class Toolbar : MonoBehaviour
{

    public PlayerInput playerInput;
    public RectTransform highlight;
    public ItemSlot[] itemSlots;
    
    public void Create()
    {

        var player = GameObject.FindWithTag("Player");
        playerInput = player.GetComponent<PlayerInput>();
        playerInput.VoxelTypeNumberSelected = 0;
    }

    private void Update()
    {

        if (GetVoxelTypeSelected().Equals(true))
        {
            highlight.position = itemSlots[playerInput.VoxelTypeNumberSelected].icon.transform.position;
        }

        var scroll = Input.GetAxis("Mouse ScrollWheel");
        switch (scroll)
        {
            case 0:
                return;
            case > 0:
                playerInput.VoxelTypeNumberSelected -= 1;
                break;
            default:
                playerInput.VoxelTypeNumberSelected += 1;
                break;
        }

        if (playerInput.VoxelTypeNumberSelected > itemSlots.Length - 1) playerInput.VoxelTypeNumberSelected = 0;

        if (playerInput.VoxelTypeNumberSelected < 0) playerInput.VoxelTypeNumberSelected = itemSlots.Length - 1;

        highlight.position = itemSlots[playerInput.VoxelTypeNumberSelected].icon.transform.position;
    }
    private bool GetVoxelTypeSelected()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) { playerInput.VoxelTypeNumberSelected = 0; return true; }
        if (Input.GetKeyDown(KeyCode.Alpha2)) { playerInput.VoxelTypeNumberSelected = 1; return true; }
        if (Input.GetKeyDown(KeyCode.Alpha3)) { playerInput.VoxelTypeNumberSelected = 2; return true; }
        if (Input.GetKeyDown(KeyCode.Alpha4)) { playerInput.VoxelTypeNumberSelected = 3; return true; }
        return false;
    }
}

[System.Serializable]
public class ItemSlot
{
    public Image icon;
}
