using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopVineDraw : MonoBehaviour {
    void OnDrawGizmos()
    {
        Gizmos.DrawIcon(transform.position, "Bean 4", true);
    }
}
