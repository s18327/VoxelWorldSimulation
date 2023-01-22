using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDataPersistence
{
    void LoadPlayerData(PlayerData data);
    void SavePlayerData(PlayerData data);

    void LoadTerrainParameters(TerrainParameters data);
    void SaveTerrainParameters(TerrainParameters data);

    void LoadTerrainData(TerrainData data);
    void SaveTerrainData(TerrainData data);
}