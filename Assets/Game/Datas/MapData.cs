using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

[CreateAssetMenu(fileName = "MapData", menuName = "Datas/MapData")]
public class MapData : ScriptableObject
{
    public List<MapObjBase> NormalObjs = new List<MapObjBase>();
    public List<BackgroundInfor> Bgs;
    public List<PlatformObj> Platforms = new List<PlatformObj>();
    public List<PipeObj> PipeObjs = new List<PipeObj>();
    public void ResetData()
    {
        NormalObjs.Clear();
        Bgs.Clear();
        Platforms.Clear();
        PipeObjs.Clear();
    }
}

[Serializable]
public class MapObjBase
{
    public GameObject Prefab;
    public Vector3 Position;
}

[Serializable]
public class BackgroundInfor
{
    public Vector3 Position;
    public Vector3 Scale;
    public Sprite Sprite;
    public int Order;
    public string LayerName;
    public SpriteDrawMode DrawMode;
    public Vector2 Size;
}

[Serializable]
public class PlatformObj
{
    public GameObject Prefab;
    public Vector3 Position;
    public List<Vector3> Path;
    public LoopType LoopType;
    public Ease EaseType;
    public float Speed;
}

[Serializable]
public class PipeObj
{
    public GameObject Prefab;
    public Vector3 Position;
    public Quaternion Rotation;
    public int IndexPipe;
    public bool IsPipeIn;
    public DirectionPipe DirectionPipe;
}