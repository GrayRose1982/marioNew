using System.Collections;
using System.Collections.Generic;
using Com.LuisPedroFonseca.ProCamera2D;
using CreativeSpore.SuperTilemapEditor;
using PathologicalGames;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapManager : MonoBehaviour
{
    private STETilemap _tileMap;

    public static MapManager Instance;
    private SpawnPool _mapPool;
    [SerializeField] private SpawnPool _bgPool;
    public GameObject BgPrefab;

    public int MapIndex;
    public List<MapData> Maps;
    public List<GameObject> TileGrounds;

    void Awake()
    {
        Instance = this;
        _mapPool = gameObject.AddComponent<SpawnPool>();

        Camera.main.transparencySortMode = TransparencySortMode.Orthographic;
    }

    void Start()
    {
#if UNITY_EDITOR
        //  Remove Scene Level
        string sceneName = GameHelper.GetAllScenes().Find(s => s.Contains("SceneLevel"));
        SceneManager.UnloadSceneAsync(sceneName);
#endif

        // StartGame();
    }

    [ContextMenu("Start Game")]
    public void StartGame()
    {
        LoadMap(MapIndex);
    }

    [ContextMenu("Stop Game")]
    public void StopGame()
    {
        _mapPool.DespawnAll();
        _bgPool.DespawnAll();
    }

    private void LoadMap(int mapIndex)
    {
        // Load Objects
        var map = Maps[mapIndex];

        // Create objs
        Transform obj;
        foreach (var item in map.NormalObjs)
            _mapPool.Spawn(item.Prefab, item.Position, Quaternion.identity);

        // Create platform
        foreach (var item in map.Platforms)
        {
            obj = _mapPool.Spawn(item.Prefab);
            obj.GetComponent<Platform>().Init(item);
        }

        // Load tile ground
        _tileMap = _mapPool.Spawn(TileGrounds[mapIndex]).GetChild(0).GetComponent<STETilemap>();
        SetBounds();

        // Load background
        Transform bgContain = transform.Find("Background");
        SpriteRenderer render;
        foreach (var bg in map.Bgs)
        {
            obj = _bgPool.Spawn(BgPrefab, bgContain);
            obj.position = bg.Position;

            render = obj.GetComponent<SpriteRenderer>();
            render.sprite = bg.Sprite;
            render.sortingOrder = bg.Order;
            render.sortingLayerName = bg.LayerName;
            render.drawMode = bg.DrawMode;
            render.size = bg.Size;
#if UNITY_EDITOR
            render.name = render.sprite.name;
#endif

            obj.localScale = bg.Scale;
        }

        // Load pipe

        foreach (var pipe in map.PipeObjs)
        {
            obj = _mapPool.Spawn(pipe.Prefab, pipe.Position, pipe.Rotation);
            obj.GetComponent<Pipe>().Init(pipe);
        }
    }

    void SetBounds()
    {
        ProCamera2DNumericBoundaries boundaries = Camera.main.GetComponent<ProCamera2DNumericBoundaries>();
        boundaries.LeftBoundary = _tileMap.MapBounds.min.x;
        boundaries.RightBoundary = _tileMap.MapBounds.max.x;
        boundaries.TopBoundary = 100;
        boundaries.BottomBoundary = _tileMap.MapBounds.min.y;
    }
}
