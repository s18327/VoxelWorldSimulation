using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

/* It's a class that handles the saving and loading of data to and from files */
public class FileDataHandler
{
    private string playerDataDirPath = "";
    private string terrainParametersDirPath = "";
    private string terrainDataDirPath = "";

    private string playerDataFileName = "";
    private string terrainParametersFileName = "";
    private string terrainDataFileName = "";



    private bool useEncryption;
    private readonly string encryptionCodeWord = "word";

    public FileDataHandler(string dataDirPath, string playerDataFileName, string terrainParametersFileName,string terrainDataFileName, bool useEncryption)
    {
        playerDataDirPath = dataDirPath;
        terrainParametersDirPath = dataDirPath;
        terrainDataDirPath = dataDirPath;

        this.playerDataFileName = playerDataFileName;
        this.terrainParametersFileName = terrainParametersFileName;
        this.terrainDataFileName = terrainDataFileName;

        this.useEncryption = useEncryption;
    }

    /// <summary>
/// It checks if the file exists, if it does, it reads the file, decrypts it if necessary, and then
/// converts the string into a PlayerData object
    /// </summary>
    /// <returns>
    /// The PlayerData is being returned.
    /// </returns>
    public PlayerData LoadPlayerData()
    {
        var fullPath = Path.Combine(playerDataDirPath, playerDataFileName);
        PlayerData loadedData = null;
        if (!File.Exists(fullPath)) return null;
        try
        {
            var dataToLoad = "";
            using (var stream = new FileStream(fullPath, FileMode.Open))
            {
                using (var reader = new StreamReader(stream))
                {
                    dataToLoad = reader.ReadToEnd();
                }
            }

            if (useEncryption)
            {
                dataToLoad = EncryptDecrypt(dataToLoad);
            }

            loadedData = JsonUtility.FromJson<PlayerData>(dataToLoad);
        }
        catch (Exception e)
        {
            Debug.LogError("Error occured when trying to load data from file: " + fullPath + "\n" + e);
        }
        return loadedData;
    }
/// <summary>
/// It checks if the file exists, if it does, it reads the file, decrypts it if necessary, and then
/// converts the string into a TerrainParameters object
/// </summary>
/// <returns>
/// The TerrainParameters object is being returned.
/// </returns>
    public TerrainParameters LoadTerrainParameters()
    {
        var fullPath = Path.Combine(terrainParametersDirPath, terrainParametersFileName);
        TerrainParameters loadedData = null;
        if (!File.Exists(fullPath)) return null;
        try
        {
            string dataToLoad;
            using (var stream = new FileStream(fullPath, FileMode.Open))
            {
                using (var reader = new StreamReader(stream))
                {
                    dataToLoad = reader.ReadToEnd();
                }
            }

            if (useEncryption)
            {
                dataToLoad = EncryptDecrypt(dataToLoad);
            }

            loadedData = JsonUtility.FromJson<TerrainParameters>(dataToLoad);
        }
        catch (Exception e)
        {
            Debug.LogError("Error occured when trying to load data from file: " + fullPath + "\n" + e);
        }
        return loadedData;
    }
/// <summary>
/// It checks if the file exists, if it does, it reads the file, decrypts it if necessary, and then
/// converts the string into a TerrainData object
/// </summary>
/// <returns>
/// The TerrainData class is being returned.
/// </returns>
    public TerrainData LoadTerrainData()
    {
        var fullPath = Path.Combine(terrainDataDirPath, terrainDataFileName);
        TerrainData loadedData = null;
        if (!File.Exists(fullPath)) return null;
        try
        {
            string dataToLoad;
            using (var stream = new FileStream(fullPath, FileMode.Open))
            {
                using (var reader = new StreamReader(stream))
                {
                    dataToLoad =  reader.ReadToEnd();
                }
            }

            if (useEncryption)
            {
                dataToLoad = EncryptDecrypt(dataToLoad);
            }

            loadedData = JsonUtility.FromJson<TerrainData>(dataToLoad);
        }
        catch (Exception e)
        {
            Debug.LogError("Error occured when trying to load data from file: " + fullPath + "\n" + e);
        }
        return loadedData;
    }



/// <summary>
/// It takes the data from the PlayerData class and converts it to a JSON string, then it encrypts it
/// and writes it to a file.
/// </summary>
/// <param name="playerData">This is the class that contains all the data that you want to save.</param>
    public void SavePlayerData(PlayerData playerData)
    {
        var fullPath = Path.Combine(playerDataDirPath, playerDataFileName);
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

            var dataToStore = JsonUtility.ToJson(playerData, true);


            if (useEncryption)
            {
                dataToStore = EncryptDecrypt(dataToStore);
            }

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                using (var writer = new StreamWriter(stream))
                {
                    writer.Write(dataToStore);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Error occured when trying to save data to file: " + fullPath + "\n" + e);
        }
    }
/// <summary>
/// It takes a TerrainParameters object, converts it to a JSON string, encrypts it, and writes it to a file
/// </summary>
/// <param name="data">This is the data that you want to save.</param>
    public void SaveTerrainParameters(TerrainParameters data)
    {
        var fullPath = Path.Combine(terrainParametersDirPath, terrainParametersFileName);
        try
        { 
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

            string dataToStore = JsonUtility.ToJson(data, true);

            if (useEncryption)
            {
                dataToStore = EncryptDecrypt(dataToStore);
            }
            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                using (var writer = new StreamWriter(stream))
                {
                    writer.Write(dataToStore);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Error occured when trying to save data to file: " + fullPath + "\n" + e);
        }
    }
/// <summary>
/// It creates a new file in the specified directory, and writes the encrypted data to it
/// </summary>
/// <param name="terrainData">This is the class that contains all the data that you want to save.</param>
    public void SaveTerrainData(TerrainData terrainData)
    {
        var fullPath = Path.Combine(terrainDataDirPath, terrainDataFileName);
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

            var dataToStore = JsonUtility.ToJson(terrainData, true);

            if (useEncryption)
            {
                dataToStore = EncryptDecrypt(dataToStore);
            }
            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                using (var writer = new StreamWriter(stream))
                {
                    writer.Write(dataToStore);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Error occured when trying to save data to file: " + fullPath + "\n" + e);
        }
    }



/// <summary>
/// It takes a string and returns a string that is the same length as the input string, but each
/// character in the output string is the result of XORing the corresponding character in the input
/// string with the corresponding character in the encryption code word
/// </summary>
/// <param name="data">The data to be encrypted or decrypted.</param>
/// <returns>
/// The modified data is being returned.
/// </returns>
    private string EncryptDecrypt(string data)
    {
        var modifiedData = "";
        for (var i = 0; i < data.Length; i++)
        {
            modifiedData += (char)(data[i] ^ encryptionCodeWord[i % encryptionCodeWord.Length]);
        }
        return modifiedData;
    }
}