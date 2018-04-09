using System;
using System.Collections.Generic;
using UnityEngine;

public class PipeList<T> : List<T>
{
    public static Action AutoFindPipe;

    public new void Add(T item)
    {
        base.Add(item);
        Debug.Log("Add new item " + Count);
        if(AutoFindPipe != null)
            AutoFindPipe.Invoke();
    }
}
