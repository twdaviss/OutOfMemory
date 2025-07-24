using UnityEngine;
using System.IO;
using System;
public class SaveDataManager
{
    private static SaveDataManager instance;
    private static SaveData saveData;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public SaveDataManager Instance()
    {
        if (instance == null)
        {
            instance = new SaveDataManager();
        }
        else
        {
            instance = this;
        }
        return instance;
    }
    [Serializable]
    public struct SaveData
    {
        public PlayerSaveData playerSaveData;
    }

    public static string SaveFileName()
    {
        string saveFile = Application.persistentDataPath + "SaveFile" + ".save";
        return saveFile;
    }

    public static void Save()
    {
        HandleSaveData();

        File.WriteAllText(SaveFileName(), JsonUtility.ToJson(saveData, true));
    }

    public static void HandleSaveData()
    {
        GameManager.Instance.playerController.Save(ref saveData.playerSaveData);
    }

    public static void Load()
    {
        string saveContent = File.ReadAllText(SaveFileName());
        saveData = JsonUtility.FromJson<SaveData>(saveContent);

        HandleLoadData();
    }

    public static void HandleLoadData()
    {
        GameManager.Instance.playerController.Load(saveData.playerSaveData);
    }
}
