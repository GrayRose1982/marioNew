using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelEditor : EditorWindow
{
    public const string LevelScenePath = "Assets/Game/Levels/LevelScenes";
    public const string LevelObjectPath = "Assets/Game/Levels/LevelObjests";
    public const string LevelTilePath = "Assets/Game/Levels/LevelTiles";
    public List<string> LevelName = new List<string>();
    public int CurLevelSelected;
    public Scene CurScene;
    public MapData MapData;

    [MenuItem("GameTool/LevelTool")]
    public static void Init()
    {
        LevelEditor w = GetWindow<LevelEditor>();
        w.Show();
    }

    private void OnGUI()
    {
        EditorGUITool.Label("LEVELTOOL", this.position.size.x, this.position.size.x, true);
        DisplayMenu();
    }

    private void DisplayMenu()
    {
        GUILayout.BeginHorizontal();
        CurLevelSelected = EditorGUILayout.Popup(CurLevelSelected, LevelName.ToArray(), GUILayout.Width(100));
        GUILayoutOption w = GUILayout.Width(80);
        if (GUILayout.Button("Load", w))
        {
            LoadLevel();
        }
        if (GUILayout.Button("Save", w))
        {
            SaveLevel();
        }
        if (GUILayout.Button("SaveOnlyBg", w))
        {
            SaveLevelOnlyBg();
        }
        if (GUILayout.Button("New", w))
        {
            CreateNewLevel();
        }
        if (GUILayout.Button("ResetLevel", w))
        {
            ResetLevel();
        }
        if (GUILayout.Button("Refresh", w))
        {
            Refresh();
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Focus Object", w))
        {
            AssetDatabase.LoadAssetAtPath<Object>(LevelObjectPath + "/" + "MapData" + CurLevelSelected + ".asset");
        }
        if (GUILayout.Button("Focus Scene", w))
        {

        }
        if (GUILayout.Button("Focus Tile", w))
        {

        }
        GUILayout.EndHorizontal();
    }



    private void Refresh()
    {
        // Get list level
        LevelName = GameHelper.GetAllAssetsAtPath(LevelScenePath).Select(s => s.name).ToList();


        var data = GameObject.FindObjectOfType<MapManager>();
        // Map
        data.Maps.Clear();
        data.Maps = GameHelper.GetAllAssetsAtPath(LevelObjectPath).Select(s => (MapData)s).ToList();
        // Ground tile
        data.TileGrounds = GameHelper.GetAllPrefabsAtPath(LevelTilePath);
        EditorUtility.SetDirty(data);
    }

    private void LoadLevel()
    {
        // Close current scene
        string sceneName = GameHelper.GetAllScenes().Find(s => s.Contains("SceneLevel"));
        CurScene = SceneManager.GetSceneByName(sceneName);
        EditorSceneManager.CloseScene(CurScene, true);
        // Load new scene
        CurScene = EditorSceneManager.OpenScene(LevelScenePath + "/" + LevelName[CurLevelSelected] + ".unity", OpenSceneMode.Additive);
    }

    private void SaveLevel()
    {
        string sceneName = GameHelper.GetAllScenes().Find(s => s.Contains("SceneLevel"));
        CurScene = SceneManager.GetSceneByName(sceneName);
        // Save level scene
        EditorSceneManager.SaveScene(CurScene);
        Transform ground =
            CurScene.GetRootGameObjects().ToList().Find(s => s.name.Contains("Background")).transform;
        var prefab = PrefabUtility.GetPrefabParent(ground.gameObject) as GameObject;
        PrefabUtility.ReplacePrefab(ground.gameObject, prefab);
        PrefabUtility.ConnectGameObjectToPrefab(ground.gameObject, prefab);

        MapData = AssetDatabase.LoadAssetAtPath<MapData>(LevelObjectPath + "/" + "MapData" + (CurLevelSelected + 1) + ".asset");
        MapData.ResetData();

        Transform objects = CurScene.GetRootGameObjects().ToList().Find(s => s.name.Contains("Objects")).transform;
        Pipe p;

        foreach (Transform t in objects)
        {
            prefab = PrefabUtility.GetPrefabParent(t.gameObject) as GameObject;
            if (prefab == null)
                continue;

            p = t.GetComponent<Pipe>();
            if (p == null)
                MapData.NormalObjs.Add(new MapObjBase()
                {
                    Position = t.position,
                    Prefab = prefab,
                });
            else
                MapData.PipeObjs.Add(new PipeObj()
                {
                    Position = t.position,
                    Rotation = t.rotation,
                    Prefab = prefab,
                    IndexPipe = p.Index,
                    IsPipeIn = p.IsPipeIn,
                    DirectionPipe = p.Direction,
                });
        }

        // Save background
        Transform background = CurScene.GetRootGameObjects().ToList().Find(s => s.name.Contains("GameBackground")).transform;
        SpriteRenderer render;
        foreach (Transform child in background)
        {
            render = child.GetComponent<SpriteRenderer>();
            MapData.Bgs.Add(new BackgroundInfor()
            {
                Position = child.position,
                Scale = child.localScale,
                Sprite = render.sprite,
                Size = render.size,
                LayerName = render.sortingLayerName,
                DrawMode = render.drawMode,
                Order = render.sortingOrder,
            });
        }

        // Save platforms
        Transform platforms = CurScene.GetRootGameObjects().ToList().Find(s => s.name.Contains("Platforms")).transform;
        Platform path;
        foreach (Transform child in platforms)
        {
            prefab = PrefabUtility.GetPrefabParent(child.gameObject) as GameObject;
            if (child.name.Contains("Platform"))
            {
                path = child.GetComponent<Platform>();
                MapData.Platforms.Add(new PlatformObj
                {
                    Prefab = prefab,
                    Position = child.position,
                    Path = path.ListPath(),
                    Speed = path.GetSpeed(),
                    LoopType = path.GetLoopType(),
                    EaseType = path.GetEaseType(),
                });
            }
        }

    }

    private void SaveLevelOnlyBg()
    {
        MapData.Bgs.Clear();
        Transform ground = FindObjectOfType<MapManager>().transform.Find("Background");

        SpriteRenderer render;
        foreach (Transform child in ground)
        {
            render = child.GetComponent<SpriteRenderer>();
            MapData.Bgs.Add(new BackgroundInfor()
            {
                Position = child.position,
                Scale = child.localScale,
                Sprite = render.sprite,
                Size = render.size,
                LayerName = render.sortingLayerName,
                DrawMode = render.drawMode,
                Order = render.sortingOrder,
            });
        }
    }

    private void ResetLevel()
    {

    }

    public void CreateNewLevel()
    {

    }
}
