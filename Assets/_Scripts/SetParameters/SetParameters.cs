using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;


public class SetParameters : MonoBehaviour
{
    public GameObject terrainParameters;
    public Terrain terrainManager;
    public List<GameObject> inputTerrainFields;


    private void Start()
    {
        InitializeParameterFields();
        SetText();
    }


    private void SetText()
    {
        inputTerrainFields[0].GetComponent<InputField>().text = terrainManager.chunkSize.ToString();
        inputTerrainFields[1].GetComponent<InputField>().text = terrainManager.chunkHeight.ToString();
        inputTerrainFields[2].GetComponent<InputField>().text = terrainManager.chunkDrawingRange.ToString();
        inputTerrainFields[3].GetComponent<InputField>().text = terrainManager.mapSeedOffset.x.ToString();
        inputTerrainFields[4].GetComponent<InputField>().text = terrainManager.mapSeedOffset.y.ToString();
    }

    private void InitializeParameterFields()
    {
        for (var i = 0; i < terrainParameters.transform.childCount; i++)
        {
            if (terrainParameters.transform.GetChild(i).childCount > 2)
            {
                inputTerrainFields.Add(terrainParameters.transform.GetChild(i).GetChild(0).gameObject);
                inputTerrainFields.Add(terrainParameters.transform.GetChild(i).GetChild(1).gameObject);
            }
            else if (terrainParameters.transform.GetChild(i).childCount == 2)
            {
                inputTerrainFields.Add(terrainParameters.transform.GetChild(i).GetChild(0).gameObject);
            }
        }
    }

    public void SetParameter()
    {

        if (EventSystem.current.currentSelectedGameObject.gameObject == inputTerrainFields[0].transform.parent.GetChild(1).gameObject)
        {
            var newParameter = inputTerrainFields[0].transform.GetChild(2).GetComponent<Text>();
            if (newParameter != null && newParameter.text.Length > 0)
                terrainManager.chunkSize = int.Parse(newParameter.text);
            return;
        }
        if (EventSystem.current.currentSelectedGameObject.gameObject == inputTerrainFields[1].transform.parent.GetChild(1).gameObject)
        {
            Text newParameter = inputTerrainFields[1].transform.GetChild(2).GetComponent<Text>();
            if (newParameter != null && newParameter.text.Length > 0)
                terrainManager.chunkHeight = int.Parse(newParameter.text);
            
            return;
        }
        if (EventSystem.current.currentSelectedGameObject.gameObject == inputTerrainFields[2].transform.parent.GetChild(1).gameObject)
        {
            var newParameter = inputTerrainFields[2].transform.GetChild(2).GetComponent<Text>();
            if (newParameter != null && newParameter.text.Length > 0)
                terrainManager.chunkDrawingRange = int.Parse(newParameter.text);
            return;
        }
        if (EventSystem.current.currentSelectedGameObject.gameObject == inputTerrainFields[3].transform.parent.GetChild(2).gameObject)
        {
            var xParameter = inputTerrainFields[3].transform.GetChild(2).GetComponent<Text>();
            var yParameter = inputTerrainFields[4].transform.GetChild(2).GetComponent<Text>();
            if (xParameter != null && yParameter != null && xParameter.text.Length > 0 && yParameter.text.Length > 0)
                terrainManager.mapSeedOffset = new Vector2Int(int.Parse(xParameter.text), int.Parse(yParameter.text));
            return;
        }
        SetText();
    }

}
