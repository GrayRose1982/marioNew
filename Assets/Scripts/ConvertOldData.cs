using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ConvertOldData : MonoBehaviour
{
    public float SizeOld = .8f;
    public float SizeNew = 1.28f;
    public Transform ObjectLoad;
    public List<OldToNew> Definitions;
    public List<OldToNew> DefinitionEnemies;
    public List<Data> OldsData;

    public GameObject baseForGround;
    public GameObject baseGO;

    public void LoadLevel(int levelLoad)
    {
        ObjectLoad.RemoveAllChildsOnEditor();
        GameObject ol = null;

        // Load object
        foreach (var objectInMap in OldsData[levelLoad].ObjsList)
        {
            bool isFound = false;
            foreach (var definition in Definitions)
            {
                if (string.Compare(definition.nameInOld, objectInMap.name, StringComparison.Ordinal) == 0)
                {
                    ol = definition.prefabInNew;
                    isFound = true;
                    break;
                }
            }

            if (isFound)
                PrefabUtility.ConnectGameObjectToPrefab(Instantiate(ol, ObjectLoad), ol).transform.position = objectInMap.position / SizeOld * SizeNew - new Vector2(1.28f / 2, 1.28f / 2);
            else
                Debug.LogError("Dont founr object name " + objectInMap.name + " in list definition");
        }

        // Load clouds
        foreach (var objectInMap in OldsData[levelLoad].Clouds)
        {
            bool isFound = false;
            foreach (var definition in Definitions)
            {
                if (string.Compare(definition.nameInOld, objectInMap.name, StringComparison.Ordinal) == 0)
                {
                    ol = definition.prefabInNew;
                    isFound = true;
                    break;
                }
            }

            if (isFound)
                PrefabUtility.ConnectGameObjectToPrefab(Instantiate(ol, ObjectLoad), ol).transform.position = objectInMap.position / SizeOld * SizeNew - new Vector2(1.28f / 2, 1.28f / 2);
            else
                Debug.LogError("Dont founr clound name " + objectInMap.name + " in list definition");
        }

        // Load Enemy
        foreach (var objectInMap in OldsData[levelLoad].Enemies)
        {
            bool isFound = false;
            foreach (var definition in DefinitionEnemies)
            {
                if (string.Compare(definition.nameInOld, objectInMap.name, StringComparison.Ordinal) == 0)
                {
                    ol = definition.prefabInNew;
                    isFound = true;
                    break;
                }
            }

            if (isFound)
                PrefabUtility.ConnectGameObjectToPrefab(Instantiate(ol, ObjectLoad), ol).transform.position = objectInMap.position / SizeOld * SizeNew - new Vector2(1.28f / 2, 1.28f / 2);
            else
                Debug.LogError("Dont found enemy name " + objectInMap.name + " in list definition");
        }

        if (baseForGround.transform.childCount > 0)
            baseForGround.transform.RemoveAllChildsOnEditor();

        // Load skeleton of ground
        foreach (var edgePoint in OldsData[levelLoad].EdgeList)
        {
            GameObject go = Instantiate(baseGO, edgePoint.position / SizeOld * SizeNew - new Vector2(1.28f / 4, 1.28f / 4), Quaternion.identity, baseForGround.transform);

            Vector2[] newPoint = edgePoint.points.ToArray();

            for (int i = 0; i < newPoint.Length; i++)
                newPoint[i] = newPoint[i] * SizeNew / SizeOld - new Vector2(1.28f / 4, 1.28f / 4);

            go.AddComponent<EdgeCollider2D>().points = newPoint;
        }
    }

    public void RemoveObject()
    {
        ObjectLoad.RemoveAllChildsOnEditor();
    }

    public void LoadOjbectInMap(int levelLoad)
    {
        GameObject ol = null;

        // Load object
        foreach (var objectInMap in OldsData[levelLoad].ObjsList)
        {
            bool isFound = false;
            foreach (var definition in Definitions)
            {
                if (string.Compare(definition.nameInOld, objectInMap.name, StringComparison.Ordinal) == 0)
                {
                    ol = definition.prefabInNew;
                    isFound = true;
                    break;
                }
            }

            if (isFound)
                PrefabUtility.ConnectGameObjectToPrefab(Instantiate(ol, ObjectLoad), ol).transform.position = objectInMap.position / SizeOld * SizeNew - new Vector2(1.28f / 2, 1.28f / 2);
            else
                Debug.LogError("Dont founr object name " + objectInMap.name + " in list definition");
        }

        // Load clouds
        foreach (var objectInMap in OldsData[levelLoad].Clouds)
        {
            bool isFound = false;
            foreach (var definition in Definitions)
            {
                if (string.Compare(definition.nameInOld, objectInMap.name, StringComparison.Ordinal) == 0)
                {
                    ol = definition.prefabInNew;
                    isFound = true;
                    break;
                }
            }

            if (isFound)
                PrefabUtility.ConnectGameObjectToPrefab(Instantiate(ol, ObjectLoad), ol).transform.position = objectInMap.position / SizeOld * SizeNew - new Vector2(1.28f / 2, 1.28f / 2);
            else
                Debug.LogError("Dont founr clound name " + objectInMap.name + " in list definition");
        }
    }

    public void LoadEnemies(int levelLoad)
    {
        GameObject ol = null;
        // Load Enemy
        foreach (var objectInMap in OldsData[levelLoad].Enemies)
        {
            bool isFound = false;
            foreach (var definition in DefinitionEnemies)
            {
                if (string.Compare(definition.nameInOld, objectInMap.name, StringComparison.Ordinal) == 0)
                {
                    ol = definition.prefabInNew;
                    isFound = true;
                    break;
                }
            }

            if (isFound && ol != null)
                PrefabUtility.ConnectGameObjectToPrefab(Instantiate(ol, ObjectLoad), ol).transform.position = objectInMap.position / SizeOld * SizeNew - new Vector2(1.28f / 2, 1.28f / 2);
            else
                Debug.LogError("Dont found enemy name " + objectInMap.name + " in list definition");
        }

    }

    public void LoadGround(int levelLoad)
    {
        if (baseForGround.transform.childCount > 0)
            baseForGround.transform.RemoveAllChildsOnEditor();

        // Load skeleton of ground
        foreach (var edgePoint in OldsData[levelLoad].EdgeList)
        {
            GameObject go = Instantiate(baseGO, edgePoint.position / SizeOld * SizeNew - new Vector2(1.28f / 4, 1.28f / 4), Quaternion.identity, baseForGround.transform);

            Vector2[] newPoint = edgePoint.points.ToArray();

            for (int i = 0; i < newPoint.Length; i++)
                newPoint[i] = newPoint[i] * SizeNew / SizeOld - new Vector2(1.28f / 4, 1.28f / 4);

            go.AddComponent<EdgeCollider2D>().points = newPoint;
        }
    }
}

[Serializable]
public class OldToNew
{
    public string nameInOld;
    public GameObject prefabInNew;
}