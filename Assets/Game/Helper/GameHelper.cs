using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class GameHelper
{
    private static Vector3 Temp;
    public static void OrderByY(this Transform t)
    {
        Temp = t.position;
        Temp.z = Temp.y * .01f;
        t.position = Temp;
    }

    /// <summary>
    /// Delete all childs of transform
    /// </summary>
    /// <param name="t"></param>
    public static void RemoveAllChildsOnEditor(this Transform t)
    {
        int c = t.childCount;
        for (int i = 0; i < c; i++)
            Object.DestroyImmediate(t.GetChild(0).gameObject);
    }

    /// <summary>
    /// Delete all childs of transform
    /// </summary>
    /// <param name="t"></param>
    public static void RemoveAllChilds(this Transform t)
    {
        int c = t.childCount;
        for (int i = 0; i < c; i++)
            Object.DestroyImmediate(t.GetChild(0).gameObject);
    }

    public static List<Vector3> FlipXList(this List<Vector3> l)
    {
        return l.Select(s => new Vector3(-s.x, s.y)).ToList();
    }

    public static List<Vector3> FlipYList(this List<Vector3> l)
    {
        return l.Select(s => new Vector3(s.x, -s.y)).ToList();
    }

    public static void FlipList<T>(List<List<T>> list)
    {
        list.Reverse();
    }

    public static List<string> GetAllScenes()
    {
        List<string> scenes = new List<string>();
        for (int i = 0; i < SceneManager.sceneCount; i++)
            scenes.Add(SceneManager.GetSceneAt(i).name);
        return scenes;
    }

    //public static Scene GetSceneByName()
    //{
        
    //}

    /// <summary>
    /// Random current list
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="l"></param>
    public static void RandomList<T>(this List<T> l)
    {
        T temp;
        int count = l.Count;
        int randomIndex;
        for (int i = 0; i < count; i++)
        {
            randomIndex = Random.Range(1, count);
            temp = l[i];
            l[i] = l[randomIndex];
            l[randomIndex] = temp;
        }
    }

    // EDITOR ------------------------------------------------------------------------------------------------------------------------------------------

#if UNITY_EDITOR

    #region Path

    public static List<GameObject> GetAllPrefabsAtPath(string path)
    {
        string[] paths = { path };
        var assets = UnityEditor.AssetDatabase.FindAssets("t:prefab", paths);
        var assetsObj = assets.Select(s => UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(UnityEditor.AssetDatabase.GUIDToAssetPath(s))).ToList();
        return assetsObj;
    }

    public static List<Sprite> GetAllSpritesAtPath(string path)
    {
        string[] paths = { path };
        var assets = UnityEditor.AssetDatabase.FindAssets("t:sprite", paths);
        List<Sprite> assetsObj = assets.Select(s => UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>(UnityEditor.AssetDatabase.GUIDToAssetPath(s))).ToList();
        return assetsObj;
    }

    public static List<Texture> GetAllTexturesAtPath(string path)
    {
        string[] paths = { path };
        var assets = UnityEditor.AssetDatabase.FindAssets("t:textures", paths);
        List<Texture> assetsObj = assets.Select(s => UnityEditor.AssetDatabase.LoadAssetAtPath<Texture>(UnityEditor.AssetDatabase.GUIDToAssetPath(s))).ToList();
        return assetsObj;
    }

    public static List<Object> GetAllAssetsAtPath(string path)
    {
        string[] paths = { path };
        var assets = UnityEditor.AssetDatabase.FindAssets(null, paths);
        List<Object> assetsObj = assets.Select(s => UnityEditor.AssetDatabase.LoadAssetAtPath<Object>(UnityEditor.AssetDatabase.GUIDToAssetPath(s))).ToList();
        return assetsObj;
    }

    #endregion


#endif
}
