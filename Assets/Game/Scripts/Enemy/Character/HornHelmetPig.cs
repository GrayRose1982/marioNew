using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HornHelmetPig : EnemyBehavior {

    void OnEnable()
    {
        base.OnEnable();

        Skeleton.AnimationName = EnemyAnimationName.Run;
    }

    void Update()
    {
        base.Update();
        MoveByHorizontal();
    }

    public override void Attacked(AttackType type, IAttackedable attacker = null)
    {
        if (!IsAlive)
            return;

        switch (type)
        {
            case AttackType.Jump:
                if(attacker!= null)
                    attacker.Attacked(AttackType.Reflect);
                break;
            case AttackType.Fire:
            case AttackType.Hit:
            case AttackType.Reflect:
            case AttackType.Dead:
                Debug.Log("Enemy " + transform.name + " die by " + type);
                IsAlive = false;
                Die();
                break;
            case AttackType.Ice:
                Skeleton.AnimationName = EnemyAnimationName.Idle;
                Freeze();
                break;
            default:
                Debug.Log("Dont have case " + type + "");
                break;
        }
    }

    protected override void UnFreeze()
    {
        base.UnFreeze();
        Skeleton.loop = true;
        Skeleton.AnimationName = EnemyAnimationName.Run;    
    }
}
