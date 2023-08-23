using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Text;
using Newtonsoft.Json;

public class DataManager : MonoBehaviour
{
    public PlayerData playerData;
    public class PlayerData
    {
        public bool[] isRegist;
        public string[] registDate;
        public int rodLevel;
        public int boxLevel;
        public int strengthLevel;
        public int fish;
        public int gold;
        public int pearl;
        public float masterAudioValue;
        public float bgmValue;
        public float sfxValue;
        public PlayerData() { }

        public PlayerData(bool isSet)
        {
            if (isSet)
            {
                isRegist = new bool[11];
                registDate = new string[11];
                rodLevel = 0;
                boxLevel = 0;
                strengthLevel = 0;
                fish = 0;
                gold = 0;
                pearl = 0;
                masterAudioValue = 0.5f;
                bgmValue = 0.5f;
                sfxValue = 0.5f;
            }
        }
    }

    private void Awake()
    {

        if (File.Exists(string.Format("{0}/{1}.json", Application.persistentDataPath, "PlayerInfo")))
        {
            playerData = new PlayerData();
            playerData = LoadJsonFile<PlayerData>(Application.persistentDataPath, "PlayerInfo");
        }
        else
        {
            PlayerData player = new PlayerData(true);
            playerData = player;
            string jsonData = ObjectToJson(player);
            CreateJsonFile(Application.persistentDataPath, "PlayerInfo", jsonData);
        }
    }

    string ObjectToJson(object obj)
    {
        return JsonUtility.ToJson(obj);
    }

    T JsonToObject<T>(string jsonData)
    {
        return JsonUtility.FromJson<T>(jsonData);
    }

    void CreateJsonFile(string createPath, string fileName, string jsonData)
    {
        FileStream fileStream =
            new FileStream(string.Format("{0}/{1}.json", createPath, fileName), FileMode.Create);
        byte[] data = Encoding.UTF8.GetBytes(jsonData);
        fileStream.Write(data, 0, data.Length);
        fileStream.Close();
    }

    [ContextMenu("Save")]
    public void Save()
    {
        string jsonData = ObjectToJson(playerData);
        SaveJsonFile(Application.persistentDataPath, "PlayerInfo", jsonData);
    }

    void SaveJsonFile(string savePath, string fileName, string jsonData)
    {
        FileStream fileStream =
            new FileStream(string.Format("{0}/{1}.json", savePath, fileName), FileMode.Create);
        byte[] data = Encoding.UTF8.GetBytes(jsonData);
        fileStream.Write(data, 0, data.Length);
        fileStream.Close();
    }

    T LoadJsonFile<T>(string loadPath, string fileName)
    {
        FileStream fileStream =
            new FileStream(string.Format("{0}/{1}.json", loadPath, fileName), FileMode.Open);
        byte[] data = new byte[fileStream.Length];
        fileStream.Read(data, 0, data.Length);
        fileStream.Close();
        string jsonData = Encoding.UTF8.GetString(data);
        return JsonToObject<T>(jsonData);
    }
}
