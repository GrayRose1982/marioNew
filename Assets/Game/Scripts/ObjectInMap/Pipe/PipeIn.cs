using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipeIn : MonoBehaviour
{
    public int index = -1;
    public DirectionPipe Direction;

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Transform pipeTo = null;

        var pipeOut = PipeController.FindCouple(index);

        if (pipeOut != null)
            Gizmos.DrawLine(transform.position, pipeOut.transform.position);

    }
#endif


    public void Init(Vector2 position, int index)
    {
        transform.position = position;
        this.index = index;
    }
}
