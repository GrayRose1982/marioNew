using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// For game design
/// </summary>
public class PipeController : MonoBehaviour
{
    private static List<Pipe> _pipesOut;

    public static List<Pipe> PipesOut
    {
        get { return _pipesOut ?? (_pipesOut = new List<Pipe>()); }
    }

    void OnEnable()
    {
        _pipesOut = new List<Pipe>();
    }

    public static Pipe FindCouple(int indexPipe)
    {
        foreach (var pipeOut in _pipesOut)
            if (pipeOut.Index == indexPipe && !pipeOut.IsPipeIn)
                return pipeOut;
        return null;
    }
}
