using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

/* It's a class that handles the saving and loading of data to and from files */
public class FileDataHandler
{
    private string playerDataDirPath = "";
    private string worldDataDirPath = "";
    private string gameDataDirPath = "";

    private string playerDataFileName = "";
    private string worldDataFileName = "";
    private string gameDataFileName = "";



    private bool useEncryption = false;
    private readonly string encryptionCodeWord = "word";

    public FileDataHandler(string dataDirPath, string playerDataFileName, string worldDataFileName,string gameDataFileName, bool useEncryption)
    {
        this.playerDataDirPath = dataDirPath;
        this.worldDataDirPath = dataDirPath;
        this.gameDataDirPath = dataDirPath;

        this.playerDataFileName = playerDataFileName;
        this.worldDataFileName = worldDataFileName;
        this.gameDataFileName = gameDataFileName;

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
        string fullPath = Path.Combine(playerDataDirPath, playerDataFileName);
        PlayerData loadedData = null;
        if (File.Exists(fullPath))
        {
            try
            {
                string dataToLoad = "";
                using (FileStream stream = new FileStream(fullPath, FileMode.Open))
                {
                    using (StreamReader reader = new StreamReader(stream))
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
        }
        return loadedData;
    }
/// <summary>
/// It checks if the file exists, if it does, it reads the file, decrypts it if necessary, and then
/// converts the string into a WorldData object
/// </summary>
/// <returns>
/// The WorldData object is being returned.
/// </returns>
    public WorldData LoadWorldData()
    {
        string fullPath = Path.Combine(worldDataDirPath, worldDataFileName);
        WorldData loadedData = null;
        if (File.Exists(fullPath))
        {
            try
            {
                string dataToLoad = "";
                using (FileStream stream = new FileStream(fullPath, FileMode.Open))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        dataToLoad = reader.ReadToEnd();
                    }
                }

                if (useEncryption)
                {
                    dataToLoad = EncryptDecrypt(dataToLoad);
                }

                loadedData = JsonUtility.FromJson<WorldData>(dataToLoad);
            }
            catch (Exception e)
            {
                Debug.LogError("Error occured when trying to load data from file: " + fullPath + "\n" + e);
            }
        }
        return loadedData;
    }
/// <summary>
/// It checks if the file exists, if it does, it reads the file, decrypts it if necessary, and then
/// converts the string into a GameData object
/// </summary>
/// <returns>
/// The GameData class is being returned.
/// </returns>
    public GameData LoadGameData()
    {
        string fullPath = Path.Combine(gameDataDirPath, gameDataFileName);
        GameData loadedData = null;
        if (File.Exists(fullPath))
        {
            try
            {
                string dataToLoad = "";
                using (FileStream stream = new FileStream(fullPath, FileMode.Open))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        dataToLoad =  reader.ReadToEnd();
                    }
                }

                if (useEncryption)
                {
                    dataToLoad = EncryptDecrypt(dataToLoad);
                }

                loadedData = JsonUtility.FromJson<GameData>(dataToLoad);
            }
            catch (Exception e)
            {
                Debug.LogError("Error occured when trying to load data from file: " + fullPath + "\n" + e);
            }
        }
        return loadedData;
    }



/// <summary>
/// It takes the data from the PlayerData class and converts it to a JSON string, then it encrypts it
/// and writes it to a file.
/// </summary>
/// <param name="PlayerData">This is the class that contains all the data that you want to save.</param>
    public void SavePlayerData(PlayerData data)
    {
        string fullPath = Path.Combine(playerDataDirPath, playerDataFileName);
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

            string dataToStore = JsonUtility.ToJson(data, true);


            if (useEncryption)
            {
                dataToStore = EncryptDecrypt(dataToStore);
            }

            using (FileStream stream = new FileStream(fullPath, FileMode.Create))
            {
                using (StreamWriter writer = new StreamWriter(stream))
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
/// It takes a WorldData object, converts it to a JSON string, encrypts it, and writes it to a file
/// </summary>
/// <param name="WorldData">This is the data that you want to save.</param>
    public void SaveWorldData(WorldData data)
    {
        string fullPath = Path.Combine(worldDataDirPath, worldDataFileName);
        try
        { 
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

            string dataToStore = JsonUtility.ToJson(data, true);

            if (useEncryption)
            {
                dataToStore = EncryptDecrypt(dataToStore);
            }
            using (FileStream stream = new FileStream(fullPath, FileMode.Create))
            {
                using (StreamWriter writer = new StreamWriter(stream))
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
/// <param name="GameData">This is the class that contains all the data that you want to save.</param>
    public void SaveGameData(GameData data)
    {
        string fullPath = Path.Combine(gameDataDirPath, gameDataFileName);
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

            string dataToStore = JsonUtility.ToJson(data, true);

            if (useEncryption)
            {
                dataToStore = EncryptDecrypt(dataToStore);
            }
            using (FileStream stream = new FileStream(fullPath, FileMode.Create))
            {
                using (StreamWriter writer = new StreamWriter(stream))
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
        string modifiedData = "";
        for (int i = 0; i < data.Length; i++)
        {
            modifiedData += (char)(data[i] ^ encryptionCodeWord[i % encryptionCodeWord.Length]);
        }
        return modifiedData;
    }
}