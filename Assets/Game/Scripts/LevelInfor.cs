using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LevelInforData
{
    public const string FileName = "LevelInfor";
    public List<LevelInfor> Levels;

    public void LoadLevel()
    {
        string data = PlayerPrefs.GetString(FileName);
        if (data == string.Empty)
            Levels = new List<LevelInfor>();
        else
            Levels = JsonHelper.FromJson<LevelInfor>(data);

        while (Levels.Count < GameConfig.NumLevel)
            Levels.Add(new LevelInfor());
        Levels[0].IsOpen = true;
    }

    public void SaveLevel()
    {
        string data = JsonHelper.ToJson(Levels);
        PlayerPrefs.SetString(FileName, data);
    }
}

[Serializable]
public class LevelInfor
{
    public bool IsOpen;
    public int Point;
    public int Time;
}
