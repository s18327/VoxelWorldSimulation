using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

/* It's a class that sets the data of a voxel */
public class SetDurabilityNewData : MonoBehaviour
{
    [FormerlySerializedAs("blockParameterParent")] public GameObject voxelParameterParent;
    [FormerlySerializedAs("blockParameters")] public List<GameObject> voxelParameters;
    public VoxelSo voxel;
    [FormerlySerializedAs("blockDataList")] public List<VoxelData> voxelDataList;
/// <summary>
/// It's a function that adds the child objects of a parent object to a list, and then sets the values
/// of the objects in the list to a default value.
/// </summary>
    void Start()
    {
        //for (int i = 0; i < voxelParameterParent.transform.childCount; i++)
        //{
        //    voxelDataList.Add(voxel.voxelDataList[i]);
        //    voxelParameters.Add(voxelParameterParent.transform.GetChild(i).gameObject);
        //    
        //    voxelDataList[i].durability = voxel.voxelDataList[i].durability;
        //    voxelDataList[i].isPlaceable = voxel.voxelDataList[i].isPlaceable;
        //}
        //
        //SetText();
        //SetClicked();
        for (int i = 0; i < voxel.voxelDataList.Count; i++)
        {
            for (var j= 0; j < voxelParameterParent.transform.childCount;j++)
            {
                if (!voxel.voxelDataList[i].voxelType.ToString()
                        .Equals(voxelParameterParent.transform.GetChild(j).gameObject.name)) continue;
                
                voxelDataList.Add(voxel.voxelDataList[i]);
                voxelParameters.Add(voxelParameterParent.transform.GetChild(j).gameObject);
            }
        }
    }


/// <summary>
/// It's a function that sets the text of an input field to the value of a variable.
/// </summary>
    public void SetText()
    {
        for (int i = 0; i < voxelParameterParent.transform.childCount; i++)
        {
            if(voxelParameters[i].name == voxelDataList[i].voxelType.ToString())
                voxelParameters[i].transform.GetChild(0).GetComponent<InputField>().text = voxelDataList[i].durability.ToString();
        }
    }

/// <summary>
/// It's a function that is called when a button is clicked. It then checks if the button that was
/// clicked is the same as the name of the voxel type in the voxelDataList. If it is, it then checks if
/// the voxel is isPlaceable or not. If it is, it disables the button that says "Place". If it isn't, it
/// disables the button that says "Destroy".
/// </summary>
    public void SetClicked()
    {
        for (int i = 1; i < voxelParameterParent.transform.childCount; i++)
        {
            if (voxelParameters[i].name == voxelDataList[i].voxelType.ToString())
            {
                if(voxelDataList[i].isPlaceable == true)
                {
                    voxelParameters[i].transform.GetChild(2).GetComponent<Button>().interactable = false;
                }
                else
                {
                    voxelParameters[i].transform.GetChild(3).GetComponent<Button>().interactable = false;
                }
            }
        }
    }

/// <summary>
/// It takes the text from a text field, converts it to an int, and then sets the durability of the
/// voxel to that int.
/// </summary>
    public void SetDurabilityData()
    {
        GameObject currentButton = EventSystem.current.currentSelectedGameObject;
        for (int i = 0; i < voxelParameterParent.transform.childCount; i++)
        {
            
            if (currentButton.transform.parent.gameObject.name == voxelDataList[i].voxelType.ToString())
            {
                voxelDataList[i].durability = (int)currentButton.transform.GetComponent<Slider>().value;
//                Text newParameter = voxelParameters[i].transform.GetChild(0).GetChild(2).GetComponent<Text>();
//                voxelDataList[i].durability = int.Parse(newParameter.text);
            }
        }
        //SetText();
    }

/// <summary>
/// It's a function that sets the isPlaceable value of a voxel to true or false depending on the button
/// that was clicked.
/// </summary>
    public void SetPlacableData()
    {
        for (int i = 0; i < voxelParameterParent.transform.childCount; i++)
        {
            GameObject currentButton = EventSystem.current.currentSelectedGameObject;
            if (currentButton.transform.parent.gameObject.name == voxelParameters[i].name)
            {
                if(currentButton.name == "Set On")
                {
                    voxelDataList[i].isPlaceable = true;
                }
                else
                {
                    voxelDataList[i].isPlaceable = false;
                }
            }
        }
        SetClicked();
    }
}
