using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Platform : MonoBehaviour
{
    [SerializeField] private DOTweenPath _tweenPath;
    [SerializeField] private Transform _trans;
    [SerializeField] private Collider2D _collider;
    private Vector2 oldPosition;

    public Transform Trans
    {
        get
        {
            if (_trans)
                _trans = transform;
            return _trans;
        }
    }

    public Collider2D Collider
    {
        get
        {
            if (_collider)
                _collider = GetComponent<Collider2D>();

            return _collider;
        }
    }

    void OnDisable()
    {
        Trans.DOKill();
    }

    /// <summary>
    /// Create start data for platform like path, start position, type path move...
    /// </summary>
    /// <param name="obj"></param>
    public void Init(PlatformObj obj)
    {
        Trans.position = obj.Position;

        if (!_tweenPath)
            _tweenPath = GetComponent<DOTweenPath>();
        if (obj.Path != null && obj.Path.Count > 0)
            Trans.DOPath(obj.Path.ToArray(), 5,PathType.Linear, PathMode.TopDown2D)
                .SetSpeedBased().SetLoops(-1, obj.LoopType).SetUpdate(UpdateType.Fixed)
                .SetEase(obj.EaseType);
    }

    public void StartRun()
    {
        _tweenPath.DOPlay();
    }

    public List<Vector3> ListPath()
    {
        if (!_tweenPath)
            _tweenPath = GetComponent<DOTweenPath>();

        return _tweenPath.wps;
    }

    public Ease GetEaseType()
    {
        if (!_tweenPath)
            _tweenPath = GetComponent<DOTweenPath>();
        return _tweenPath.easeType;
    }

    public LoopType GetLoopType()
    {
        if (!_tweenPath)
            _tweenPath = GetComponent<DOTweenPath>();

        return _tweenPath.loopType;
    }

    public float GetSpeed()
    {
        if (!_tweenPath)
            _tweenPath = GetComponent<DOTweenPath>();

        return _tweenPath.duration;
    }
}
