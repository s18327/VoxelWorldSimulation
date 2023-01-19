using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/* It's a class that sets the data of a block */
public class SetBlockNewData : MonoBehaviour
{
    public GameObject blockParameterParent;
    public List<GameObject> blockParameters;
    public VoxelSo voxel;
    public List<VoxelData> blockDataList;
/// <summary>
/// It's a function that adds the child objects of a parent object to a list, and then sets the values
/// of the objects in the list to a default value.
/// </summary>
    void Start()
    {
        for (int i = 0; i < blockParameterParent.transform.childCount; i++)
        {
            blockDataList.Add(voxel.voxelDataList[i]);
            blockParameters.Add(blockParameterParent.transform.GetChild(i).gameObject);
            
            blockDataList[i].durability = voxel.voxelDataList[i].durability;
            blockDataList[i].placable = voxel.voxelDataList[i].placable;
        }
        
        SetText();
        SetClicked();
    }


/// <summary>
/// It's a function that sets the text of an input field to the value of a variable.
/// </summary>
    public void SetText()
    {
        for (int i = 0; i < blockParameterParent.transform.childCount; i++)
        {
            if(blockParameters[i].name == blockDataList[i].voxelType.ToString())
                blockParameters[i].transform.GetChild(0).GetComponent<InputField>().text = blockDataList[i].durability.ToString();
        }
    }

/// <summary>
/// It's a function that is called when a button is clicked. It then checks if the button that was
/// clicked is the same as the name of the block type in the blockDataList. If it is, it then checks if
/// the block is placable or not. If it is, it disables the button that says "Place". If it isn't, it
/// disables the button that says "Destroy".
/// </summary>
    public void SetClicked()
    {
        for (int i = 1; i < blockParameterParent.transform.childCount; i++)
        {
            if (blockParameters[i].name == blockDataList[i].voxelType.ToString())
            {
                if(blockDataList[i].placable == true)
                {
                    blockParameters[i].transform.GetChild(2).GetComponent<Button>().interactable = false;
                }
                else
                {
                    blockParameters[i].transform.GetChild(3).GetComponent<Button>().interactable = false;
                }
            }
        }
    }

/// <summary>
/// It takes the text from a text field, converts it to an int, and then sets the durability of the
/// block to that int.
/// </summary>
    public void SetDurabilityData()
    {
        for (int i = 0; i < blockParameterParent.transform.childCount; i++)
        {
            GameObject currentButton = EventSystem.current.currentSelectedGameObject;
            if (currentButton.transform.parent.gameObject.name == blockDataList[i].voxelType.ToString())
            {
                Text newParameter = blockParameters[i].transform.GetChild(0).GetChild(2).GetComponent<Text>();
                blockDataList[i].durability = int.Parse(newParameter.text);
            }
        }
        SetText();
    }

/// <summary>
/// It's a function that sets the placeable value of a block to true or false depending on the button
/// that was clicked.
/// </summary>
    public void SetPlaceableData()
    {
        for (int i = 0; i < blockParameterParent.transform.childCount; i++)
        {
            GameObject currentButton = EventSystem.current.currentSelectedGameObject;
            if (currentButton.transform.parent.gameObject.name == blockParameters[i].name)
            {
                blockDataList[i].placable = currentButton.name == "Set On";
            }
        }
        SetClicked();
    }
}
