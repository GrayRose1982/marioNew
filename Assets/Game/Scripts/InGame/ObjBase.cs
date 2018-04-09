using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjBase : MonoBehaviour
{

    public virtual void Awake()
    {
        transform.OrderByY();
    }
}
