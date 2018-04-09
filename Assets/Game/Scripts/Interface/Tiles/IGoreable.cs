using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGoreable
{
    void Gore();
    event Action<AttackType, IAttackedable> Gored;
}
