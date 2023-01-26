using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/* It's a class that enables or disables trees on a terrain */
public class TreeSwitch : MonoBehaviour
{
    public GameObject treeSetParameter;
    public NoiseSettings treeEnable;
    public NoiseSettings treeDisable;

    public GameObject terrainGenerator;
    public List<GameObject> biomeGenerators;


/// <summary>
/// It's a function that disables the tree generation for all the biomes in the scene on start.
/// </summary>
private void Start()
    {
        for (var i = 0; i < terrainGenerator.transform.childCount; i++)
        {
            biomeGenerators.Add(terrainGenerator.transform.GetChild(i).gameObject);
        }

        foreach (var biomeGenerator in biomeGenerators)
        {
            biomeGenerator.GetComponent<TreeGenerator>().treeNoiseSettings = treeDisable;
        }
        SetClicked();
    }

/// <summary>
/// If the treeNoiseSettings variable is equal to treeDisable, then the button at index 1 of the child
/// of the treeSetParameter object is set to false.
/// 
/// If the treeNoiseSettings variable is equal to treeEnable, then the button at index 0 of the child of
/// the treeSetParameter object is set to false.
/// </summary>
    private void SetClicked()
{
    foreach (var biome in biomeGenerators)
    {
        if (biome.GetComponent<TreeGenerator>().treeNoiseSettings == treeDisable)
        {
            treeSetParameter.transform.GetChild(1).GetComponent<Button>().interactable = false;
        }
        else if (biome.GetComponent<TreeGenerator>().treeNoiseSettings == treeEnable)
        {
            treeSetParameter.transform.GetChild(0).GetComponent<Button>().interactable = false;
        }
    }
}

/// <summary>
/// This function is called when the user clicks the "Enable Trees" button.
/// </summary>
    public void TreeEnable()
    {
    foreach (var biome in biomeGenerators)
    {
        biome.GetComponent<TreeGenerator>().treeNoiseSettings = treeEnable;
    }

    SetClicked();
    }

/// <summary>
/// It loops through all the biome generators and sets their tree noise settings to the treeDisable
/// noise settings.
/// </summary>
    public void TreeDisable()
{
    foreach (var biome in biomeGenerators)
    {
        biome.GetComponent<TreeGenerator>().treeNoiseSettings = treeDisable;
    }

    SetClicked();
}
}
