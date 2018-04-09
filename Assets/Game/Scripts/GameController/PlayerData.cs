using System;
using Spine.Unity;
using UnityEngine;

public class PlayerData : MonoBehaviour
{
    [SerializeField] private static bool isNeverLose = true;

    #region Skeleton data asset 
    public SkeletonDataAsset Small;
    public SkeletonDataAsset Grown;
    public SkeletonDataAsset Ice;
    public SkeletonDataAsset Fire;
    public SkeletonDataAsset Winged;

    public Material[] SkeletonMaterials;

    #endregion

    #region Variable
    private static int _coin = 0;
    private static int _life = 3;
    private static int _score = 0;
    private static long _time = 0;
    private static int NumberCoinLevelUp = 100;
    private static bool isNew = true;

    public static Action<string> _updateLife;
    public static Action<string> _updateCoin;
    public static Action<string> _updateTime;
    public static Action<string> _updateScore;
    #endregion

    #region Properties

    public static int Coin
    {
        get { return _coin; }
        private set
        {
            _coin = value; 

            if(_updateCoin != null)
                _updateCoin.Invoke(_coin.ToString());
        }
    }

    public static int Life
    {
        get { return _life; }
        private set
        {
            _life = value;
            if (_updateLife != null)
                _updateLife.Invoke(_life.ToString());
        }
    }

    public static int Score
    {
        get { return _score; }
        private set
        {
            _score = value;
            if (_updateScore != null)
                _updateScore.Invoke(_score.ToString());
        }
    }

    public static long CurrentTime
    {
        get { return _time; }
        private set
        {
            _time = value;
            if (_updateTime != null)
                _updateTime.Invoke(_time.ToString());
        }
    }

    #endregion

    #region Function

    void OnEnable()
    {
        Coin = PlayerPrefs.GetInt("Coin", 0);
        Life = PlayerPrefs.GetInt("Life", 3);
        Score = PlayerPrefs.GetInt("Score", 0);
    }

    void OnDisable()
    {
        PlayerPrefs.SetInt("Coin", _coin);
        PlayerPrefs.SetInt("Life", _life);
        PlayerPrefs.SetInt("Score", _life);

        foreach (Material skeletonMat in SkeletonMaterials)
        {
            skeletonMat.color = Color.white;
        }
    }

    void Update()
    {

    }

    public static void AddCoin(int number)
    {
        if (Coin + number >= int.MaxValue)
            Coin = int.MaxValue;
        else
            Coin += number;
            
        if (Coin > NumberCoinLevelUp)
            LiveUp();
    }

    public static void GetCoin()
    {
        AddCoin(1);
    }

    public static void AddScore(int number)
    {
        if (Score + number >= int.MaxValue)
            Score = int.MaxValue;
        else
            Score += number;
    }

    private static void LiveUp()
    {
        Life++;
        Coin = 0;
        Debug.Log("Life up " + Life + " current life " + Life);
    }

    public static bool RemoveLife()
    {
        if (Life > 0)
        {
            Life--;
            Debug.Log("Lose life");
            return true;
        }

        if (isNeverLose)
            return true;

        return false;
    }

    public static bool SpawnPlayer()
    {
        if (isNew)
        {
            isNew = false;
            return true;
        }

        return RemoveLife();
    }
    #endregion
}
