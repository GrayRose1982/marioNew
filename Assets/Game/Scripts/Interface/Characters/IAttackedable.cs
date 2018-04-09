using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAttackedable
{
    /// <summary>
    /// Aplly effect after attack
    /// </summary>
    /// <param name="type">Type of attack</param>
    /// <param name="attacker">Call back for attacker</param>
    void Attacked(AttackType type, IAttackedable attacker = null);
}
