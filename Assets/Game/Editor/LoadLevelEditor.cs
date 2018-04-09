using System.Collections;
using UnityEditor;
using UnityEngine;

public class LoadLevelEditor : EditorWindow
{
    private int _currentLevelLoad = 0;

    [MenuItem("GameTool/LoadLevelTool")]
    public static void Init()
    {
        LoadLevelEditor w = GetWindow<LoadLevelEditor>();
        w.Show();
    }

    private void OnGUI()
    {
        EditorGUITool.Label("LOAD LEVEL TOOl", this.position.size.x, this.position.size.x, true);
        DisplayMenu();
    }

    private void DisplayMenu()
    {
        _currentLevelLoad = int.Parse(EditorGUILayout.TextField("Current level", _currentLevelLoad.ToString()));

        if (GUILayout.Button("Load All"))
        {
            Debug.Log("Load level " + _currentLevelLoad);
            FindObjectOfType<ConvertOldData>().LoadLevel(_currentLevelLoad);
        }

        if (GUILayout.Button("Delete All object"))
        {
            Debug.Log("Delete All object " + _currentLevelLoad);
            FindObjectOfType<ConvertOldData>().RemoveObject();
        }

        if (GUILayout.Button("Load object only"))
        {
            Debug.Log("Load object only " + _currentLevelLoad);
            FindObjectOfType<ConvertOldData>().LoadOjbectInMap(_currentLevelLoad);
        }

        if (GUILayout.Button("Load Enemy"))
        {
            Debug.Log("Load Enemy " + _currentLevelLoad);
            FindObjectOfType<ConvertOldData>().LoadEnemies(_currentLevelLoad);
        }

        if (GUILayout.Button("Load ground"))
        {
            Debug.Log("Load ground " + _currentLevelLoad);
            FindObjectOfType<ConvertOldData>().LoadGround(_currentLevelLoad);
        }


    }

}
