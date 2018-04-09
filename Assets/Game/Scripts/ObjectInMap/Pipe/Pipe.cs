using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pipe : MonoBehaviour
{
    [SerializeField] private int _index;
    [SerializeField] private DirectionPipe _direction;
    [SerializeField] private bool _isPipeIn;

    public int Index
    {
        get { return _index; }
    }

    public DirectionPipe Direction
    {
        get { return _direction; }
    }

    public bool IsPipeIn
    {
        get { return _isPipeIn; }
    }


#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        if (!_isPipeIn)
        {
            if (!PipeController.PipesOut.Contains(this))
                PipeController.PipesOut.Add(this);
            return;
        }

        Gizmos.color = Color.yellow;

        var pipeOut = PipeController.FindCouple(_index);

        if (pipeOut != null)
            Gizmos.DrawLine(transform.position, pipeOut.transform.position);

    }
#endif

    void OnEnable()
    {
        PipeController.PipesOut.Add(this);
    }

    void OnDisable()
    {
        PipeController.PipesOut.Remove(this);
    }

    public void Init(PipeObj initData)
    {
        _direction = initData.DirectionPipe;
        _isPipeIn = initData.IsPipeIn;
        _index = initData.IndexPipe;
    }
}
