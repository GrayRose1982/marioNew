using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chicken : EnemyBehavior
{
    // Update is called once per frame
    public override void Update()
    {
        MoveByHorizontal();

        base.Update();
    }

    protected override void UnFreeze()
    {
        base.UnFreeze();
        Skeleton.AnimationName = EnemyAnimationName.Walk;
    }
}
