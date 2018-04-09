using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData : MonoBehaviour
{
    public static GameData Instance;
    public LevelInforData LevelInfor;
    public PlayerData Player;
    void Awake()
    {
        Instance = this;
        LoadData();
        if (GameData.Instance.LevelInfor.Levels[1].IsOpen)
        {
            GameData.Instance.LevelInfor.Levels[1].Point = 100;
            GameData.Instance.LevelInfor.SaveLevel();
        }
    }

    void LoadData()
    {
        LevelInfor.LoadLevel();
    }

    [ContextMenu("SaveLevelData")]
    public void SaveLevelData()
    {
        LevelInfor.SaveLevel();
    }
}
