using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DataOld", menuName = "Datas/DataOld")]
public class Data : ScriptableObject
{
    public List<ObjectInMap> ObjsList = new List<ObjectInMap>();
    public List<ObjectInMap> Enemies = new List<ObjectInMap>();
    public List<ObjectInMap> Clouds = new List<ObjectInMap>();
    public List<ObjectInMap> Platforms = new List<ObjectInMap>();
    public List<EdgePoint> EdgeList = new List<EdgePoint>();

    public void AddNewObject(List<Transform> objs, Vector2 parent)
    {
        foreach (var o in objs)
        {
            ObjsList.Add(new ObjectInMap()
            {
                name = o.name,
                position = ((Vector2)o.position + parent),
            });
        }
    }

    public void LoadEnemy(Transform parentEnemy)
    {
        for (int i = 0; i < parentEnemy.childCount; i++)
            if (parentEnemy.GetChild(i).name.Contains("Enemy"))
                Enemies.Add(new ObjectInMap()
                {
                    name = parentEnemy.GetChild(i).name.Substring(6, 2),
                    position = ((Vector2)parentEnemy.GetChild(i).position),
                });
    }

    public void LoadCollider(Transform parentAllCollider)
    {
        for (int i = 0; i < parentAllCollider.childCount; i++)
        {
            var child = parentAllCollider.GetChild(i);
            var ecs = child.GetComponents<EdgeCollider2D>();

            foreach (var ec in ecs)
                EdgeList.Add(new EdgePoint()
                {
                    position = child.position,
                    points = new List<Vector2>(ec.points)
                });
        }
    }

    public void Reset()
    {
        ObjsList.Clear();
        Enemies.Clear();
    }
}

[Serializable]
public class ObjectInMap
{
    public string name;
    public Vector2 position;
}

[Serializable]
public class EdgePoint
{
    public Vector2 position;
    public List<Vector2> points;
}