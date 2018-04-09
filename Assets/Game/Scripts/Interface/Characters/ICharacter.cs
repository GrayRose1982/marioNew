using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICharacter
{
    bool IsAlive { get; }
    bool IsFreeze { get; }
    bool CanMove { get; }

    Transform Trans{get;}
    Rigidbody2D Rigid { get; }
    Collider2D Col { get; }

    event Action UnfreezeAction;
}
