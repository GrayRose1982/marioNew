using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Line : MonoBehaviour
{
    public Transform Target;
    public LineRenderer LineRenderer;
    public int NumPoint;
    public Vector3 Direct;
    public float Delta = .5f;
    public float RandomRange = .2f;
    public float OldTime;

    void Update()
    {
        UpdateLine();
    }

    void UpdateLine()
    {
        if (Time.time - OldTime > .1f)
        {
            OldTime = Time.time;
            NumPoint = (int)(Vector2.Distance(Target.position, transform.position) / Delta);
            Direct = (Vector2)(Target.position - transform.position);
            Direct.Normalize();
            LineRenderer.positionCount = NumPoint;
            for (int i = 0; i < NumPoint; i++)
            {
                LineRenderer.SetPosition(i, transform.position + Direct * i + new Vector3(Random.Range(-RandomRange, RandomRange), Random.Range(-RandomRange, RandomRange)));
            }
        }
        Debug.DrawLine(transform.position, transform.position + Direct);
        LineRenderer.material.mainTextureOffset += new Vector2(-Time.deltaTime, 0);
    }

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        UnityEditor.Handles.Label((transform.position + Target.position) / 2, Vector2.Distance(Target.position, transform.position).ToString());
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, Target.position);
    }
#endif
}
