using UnityEngine;
using UnityEngine.EventSystems;
#if UNITY_EDITOR
using UnityEditor;
using System;
using System.Linq;
using UnityEditorInternal;
using System.Reflection;

public class EditorGUITool
{
    /// <summary>
    /// Make single press button
    /// </summary>
    /// <param name="labelName">Name button</param>
    /// <param name="width">Width button</param>
    /// <returns></returns>
    public static bool MakeButton(string labelName, float width, bool isCenter)
    {
        return MakeButton(labelName, width, new Color(.7f, 1f, 1f), isCenter, 0.0f);
    }

    public static bool MakeButton(string labelName, float width, float height, bool isCenter)
    {
        return MakeButton(labelName, width, height, new Color(.7f, 1f, 1f), isCenter, 0.0f);
    }

    public static bool MakeButton(string labelName, float width)
    {
        return MakeButton(labelName, width, new Color(.7f, 1f, 1f), false, 0.0f);
    }

    /// <summary>
    /// Make single press button 
    /// </summary>
    /// <param name="labelName">Name button</param>
    /// <param name="width">Width button</param>
    /// <param name="color">Color button</param>
    /// <param name="isCenter">Button is center or not</param>
    /// <param name="windowSize">Need size window contain button to make button in center</param>
    /// <returns></returns>
    public static bool MakeButton(string labelName, float width, Color color, bool isCenter, float windowSize)
    {
        GUILayout.BeginHorizontal();
        if (isCenter)
            GUILayout.Space((windowSize - width) / 2);
        GUI.color = color;
        if (GUILayout.Button(labelName, EditorStyles.toolbarButton, new GUILayoutOption[] { GUILayout.Width(width) }))
        {
            GUILayout.EndHorizontal();
            return true;
        }
        GUI.color = Color.white;
        GUILayout.EndHorizontal();
        return false;
    }

    public static bool MakeButton(string labelName, float width, float height, Color color, bool isCenter, float windowSize)
    {
        GUILayout.BeginHorizontal();
        if (isCenter)
            GUILayout.Space((windowSize - width) / 2);
        GUI.color = color;
        if (GUILayout.Button(labelName, EditorStyles.toolbarButton, GUILayout.Width(width), GUILayout.Height(height)))
        {
            GUILayout.EndHorizontal();
            return true;
        }
        GUI.color = Color.white;
        GUILayout.EndHorizontal();
        return false;
    }

    public static void DrawHeader(string labelName, float width, Color color, Rect rect)
    {
        rect.x += (rect.width - width) / 2;
        GUILayout.BeginHorizontal();
        GUI.color = Color.cyan;
        GUI.Label(rect, labelName, EditorStyles.boldLabel);
        GUI.color = Color.white;
        GUILayout.EndHorizontal();
    }

    /// <summary>
    /// Method draw box with inside and outside border
    /// </summary>
    public static void BorderBox(float outdSideBorder, float inSideBorder, System.Action inside)
    {
        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(inSideBorder);
        Box((int)inSideBorder, inside);
        GUILayout.Space(outdSideBorder);
        EditorGUILayout.EndHorizontal();
    }

    public static void Box(int aBorder, System.Action inside)
    {
        Box(aBorder, inside, 0, 0, Color.white, false);
    }

    public static void Box(int aBorder, System.Action inside, bool isReorderList)
    {
        Box(aBorder, inside, 0, 0, Color.white, true);
    }

    public static void Box(int aBorder, System.Action inside, float aWidthOverride, float aHeightOverride, Color color, bool isReorderList)
    {
        Rect r = EditorGUILayout.BeginHorizontal(GUILayout.Width(aWidthOverride));
        if (aWidthOverride != 0)
        {
            r.width = aWidthOverride;
        }
        GUI.color = color;
        GUI.Box(r, GUIContent.none);
        GUI.color = Color.white;

        GUILayout.Space(aBorder);
        if (aHeightOverride != 0)
            EditorGUILayout.BeginVertical(GUILayout.Height(aHeightOverride));
        else
            EditorGUILayout.BeginVertical();
        //GUILayout.Space(aBorder);
        inside();
        //GUILayout.Space(aBorder);
        EditorGUILayout.EndVertical();
        if (isReorderList)
            GUILayout.Space(aBorder * 1.5f);
        else
            GUILayout.Space(aBorder);
        EditorGUILayout.EndHorizontal();
    }

    public static void Parallel(System.Action inside01, float width01, System.Action inside02, float width02)
    {
        GUILayout.BeginHorizontal();
        GUILayout.BeginHorizontal(GUILayout.Width(width01));
        inside01();
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal(GUILayout.Width(width02));
        inside02();
        GUILayout.EndHorizontal();
        EditorGUILayout.EndHorizontal();
    }

    /// <summary>
    /// Make a toggle
    /// </summary>
    /// <param name="value">    Show state of toggle    </param>
    /// <param name="label">    Label if toggle         </param>
    /// <param name="labelWith">Label width of toggle   </param>
    /// <param name="color">    Color of label          </param>
    /// <returns></returns>
    public static bool Toggle(bool value, string label, float labelWith, Color color)
    {
        EditorGUILayout.BeginHorizontal();
        GUI.color = color;
        EditorGUILayout.LabelField(label, GUILayout.Width(labelWith));
        GUI.color = Color.white;
        bool _enableService = EditorGUILayout.Toggle(value, GUILayout.Width(10));
        EditorGUILayout.EndHorizontal();
        return _enableService;
    }

    public static string TextField(string label, float labelWidth, string textInput, float textWidth)
    {
        EditorGUILayout.BeginHorizontal(GUILayout.Width(labelWidth + textWidth));
        GUILayout.Label(label, GUILayout.Width(labelWidth));
        var _str = EditorGUILayout.TextField(GUIContent.none, textInput, GUILayout.Width(textWidth));
        EditorGUILayout.EndHorizontal();
        return _str;
    }

    public static void PropertyField(string label, SerializedObject serObj, string properName, float widthLabel, float widthContent)
    {
        EditorGUITool.Parallel(() => { GUILayout.Label(label); }, widthLabel, () =>
        {
            EditorGUILayout.PropertyField(serObj.FindProperty(properName), GUIContent.none);
        }, widthContent);
    }

    public static int Toolbar<T>(int selected, float width, Color color)
    {
        GUI.color = color;
        string[] arrStrings = System.Enum.GetValues(typeof(T)).Cast<Enum>().Select(v => v.ToString()).ToArray();
        selected = GUILayout.Toolbar(selected, arrStrings, EditorStyles.toolbarButton, new GUILayoutOption[] { GUILayout.Width(width) });
        GUI.color = Color.white;
        return selected;
    }

    /// <summary>
    /// Open folder in editor unity with default path from asset
    /// </summary>
    /// <returns></returns>
    public static string OpenFolderPanel(string path = null)
    {
        string _folderPath = null;
        string _path = Application.dataPath;
        if (_path == null)
            _folderPath = _path.Substring(0, _path.Length - 7);
        else
            _folderPath = path;
        return _folderPath = EditorUtility.OpenFolderPanel("Folder", _folderPath, "");
    }


    /// <summary>
    /// Draw label with good style and custome center position
    /// </summary>
    /// <param name="name"></param>
    /// <param name="width"></param>
    /// <param name="windowSize"></param>
    /// <param name="isCenter"></param>
    public static void Label(string name, float width, float windowSize, bool isCenter)
    {
        Rect _rect = EditorGUILayout.BeginHorizontal(GUILayout.Width(width));
        GUI.color = new Color(.8f, .8f, .8f);
        GUI.Label(new Rect(_rect.x + (windowSize - width) / 2, _rect.y, width, _rect.height), GUIContent.none, EditorStyles.toolbarButton);
        GUI.color = Color.white;
        EditorGUILayout.BeginVertical();
        GUI.color = Color.cyan;
        EditorGUILayout.BeginHorizontal();
        if (isCenter)
            GUILayout.Space((windowSize - width) / 2);
        EditorGUILayout.LabelField(name, EditorStyles.boldLabel, GUILayout.Width(width));
        EditorGUILayout.EndHorizontal();
        GUI.color = Color.white;
        EditorGUILayout.EndVertical();
        GUILayout.EndHorizontal();
        GUILayout.Space(10.0f);
    }

    /// <summary>
    /// Draw GUI in border
    /// </summary>
    /// <param name="borderLeft"></param>
    /// <param name="width"></param>
    /// <param name="actions"></param>
    public static void Border(float borderLeft, float width, System.Action actions)
    {
        EditorGUILayout.BeginHorizontal(GUILayout.Width(width));
        GUILayout.Space(borderLeft);
        EditorGUILayout.BeginVertical();
        actions();
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();
    }

    /// <summary>
    /// Initial interger Pop up
    /// </summary>
    public static int InitPop(string label, float widthLabel, int selected, string[] displayOptions, int[] optionValues, float widthPopup)
    {
        return InitPop(label, widthLabel, selected, displayOptions, optionValues, widthPopup, Color.white);
    }

    /// <summary>
    /// Initial interger Pop up
    /// </summary>
    public static int InitPop(string label, float widthLabel, int selected, string[] displayOptions, int[] optionValues, float widthPopup, Color color)
    {
        EditorGUILayout.BeginHorizontal();
        GUI.color = color;
        EditorGUILayout.LabelField(label, GUILayout.Width(widthLabel));
        GUI.color = Color.white;
        int _selected = EditorGUILayout.IntPopup(selected, displayOptions, optionValues, GUILayout.Width(widthPopup));
        EditorGUILayout.EndHorizontal();
        return _selected;
    }

    public static void ShowDialogError(string message)
    {
        EditorUtility.DisplayDialog("Error!", message, "Ok");
    }
}

public class EditorUtilityLib
{
    /// <summary>
    /// Method get sorting layer name
    /// </summary>
    /// <returns></returns>
    public static string[] GetSortingLayerNames()
    {
        Type internalEditorUtilityType = typeof(InternalEditorUtility);
        PropertyInfo sortingLayersProperty = internalEditorUtilityType.GetProperty("sortingLayerNames", BindingFlags.Static | BindingFlags.NonPublic);
        return (string[])sortingLayersProperty.GetValue(null, new object[0]);
    }

    public int[] GetSortingLayerUniqueIDs()
    {
        Type internalEditorUtilityType = typeof(InternalEditorUtility);
        PropertyInfo sortingLayerUniqueIDsProperty = internalEditorUtilityType.GetProperty("sortingLayerUniqueIDs", BindingFlags.Static | BindingFlags.NonPublic);
        return (int[])sortingLayerUniqueIDsProperty.GetValue(null, new object[0]);
    }

    /// <summary>
    /// Method load data in editor
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="const_key"></param>
    /// <returns></returns>
    public static T LoadData<T>(string const_key) where T : new()
    {
        if (EditorPrefs.HasKey(const_key))
            return JsonUtility.FromJson<T>(EditorPrefs.GetString(const_key));
        else
        {
            SaveData(new T(), const_key);
            return new T();
        }
    }

    /// <summary>
    /// Method save data on editor
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="data"></param>
    /// <param name="const_key"></param>
    public static void SaveData<T>(T data, string const_key)
    {
        string _convertStr = JsonUtility.ToJson(data);
        EditorPrefs.SetString(const_key, _convertStr);
    }
}
#endif
