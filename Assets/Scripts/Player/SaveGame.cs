using System;
using System.IO;
using TMPro;
using UnityEngine;
[Serializable]
public class GameData
{
    public int text;
    public GameData(int text)
    {
        this.text = text;
    }
}
public class SaveGame : MonoBehaviour
{
    [SerializeField] protected TextMeshProUGUI text;
    private void OnEnable()
    {
        Load();
    }
    protected void Save()
    {
        string destination = Application.persistentDataPath + "/save.dat";
        string json = JsonUtility.ToJson(new GameData(Int32.Parse(text.text)));
        File.WriteAllText(destination, json);
    }
    protected void Load()
    {
        string destination = Application.persistentDataPath + "/save.dat";
        string json = File.ReadAllText(destination);
        text.text = JsonUtility.FromJson<GameData>(json).text.ToString();
    }
    private void OnApplicationQuit()
    {
        Save();
    }
}
