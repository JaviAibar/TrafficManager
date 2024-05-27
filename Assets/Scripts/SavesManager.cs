using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SavesManager : MonoBehaviour
{
    public Transform playerPos;

    private void Start()
    {
        LoadValues();
    }
    public void SaveValues()
    {
        // TODO: Are you sure? window
        string filename = "savefile.json";
        ConfigFile configFile = new ConfigFile();
        // Obtain data from 
        configFile.playerPos = playerPos.position;
        configFile.playerRot = playerPos.eulerAngles;
        string jsonString = JsonUtility.ToJson(configFile);

        // Create / update save file
        File.WriteAllText(Application.persistentDataPath + "/" + filename, jsonString);
        print("Saved in " + Application.persistentDataPath + "/" + filename);
    }

    public void LoadValues()
    {
        string filename = "savefile.json";
        if (File.Exists(Application.persistentDataPath + "/" + filename))
        {
            string jsonString = File.ReadAllText(Application.persistentDataPath + "/" + filename);
            ConfigFile configFile = JsonUtility.FromJson<ConfigFile>(jsonString);
            playerPos.position = configFile.playerPos;
            playerPos.eulerAngles = configFile.playerRot;
            print("Cargado " + Application.persistentDataPath + "/" + filename);
        }
        else
        {
            print("No existe " + Application.persistentDataPath + "/" + filename);

        }
    }
}

[Serializable]
public class ConfigFile
{
    public Vector3 playerPos;
    public Vector3 playerRot;
    public int lifes;

    public ConfigFile(Vector3 playerPos, Vector3 playerRot, int lifes)
    {
        this.playerPos = playerPos;
        this.playerRot = playerRot;
        this.lifes = lifes;
    }
    public ConfigFile() { }
}