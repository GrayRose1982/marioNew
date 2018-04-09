using System;
using Com.LuisPedroFonseca.ProCamera2D;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController Instance;
    public static Transform PlayerTrans;
    [HideInInspector]
    public ProCamera2D ProCam;
    [HideInInspector]
    public ProCamera2DNumericBoundaries ProBoundariesCam;
    [HideInInspector]
    public Transform CamTrans;
    [SerializeField, HideInInspector]
    public Transform Map;

    public PlayerController Player;

    void Awake()
    {
        Instance = this;

        Application.targetFrameRate = 60;
        ProCam = ProCamera2D.Instance;
        CamTrans = ProCam.transform;
        ProBoundariesCam = ProCam.GetComponent<ProCamera2DNumericBoundaries>();
        PlayerTrans = FindObjectOfType<PlayerController>().transform;
        //Map = GameObject.Find("Map").transform;
    }

    void Start()
    {
        ProCam.CameraTargets.Clear();
        Player = GameObject.Find("Player").transform.GetComponent<PlayerController>();
        ProCam.CameraTargets.Add(new CameraTarget() { TargetTransform = Player.transform });
        Player.gameObject.SetActive(false);

        StartGame();
    }

    public void StartGame()
    {
        MapManager.Instance.StartGame();

        UpdateCameraBoundaries();

        Player.InitLife();
    }

    public void StopGame()
    {
        MapManager.Instance.StopGame();
    }

    void UpdateCameraBoundaries()
    {
        //var tile = Map.Find("Backgrounds").GetComponent<STETilemap>();
        //ProBoundariesCam.TopBoundary = tile.MapBounds.max.y;
        //ProBoundariesCam.BottomBoundary = tile.MapBounds.min.y;
        //ProBoundariesCam.LeftBoundary = tile.MapBounds.min.x;
        //ProBoundariesCam.RightBoundary = tile.MapBounds.max.x;
    }

    public void SetTimeScale(float value)
    {
        Time.timeScale = value;
    }
}
