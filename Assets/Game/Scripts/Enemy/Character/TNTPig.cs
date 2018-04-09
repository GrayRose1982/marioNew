using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TNTPig : EnemyBehavior
{
    [SerializeField] private float _timePrepare = 2f;
    [SerializeField] private float _radiusExplosive = 3f;

#if UNITY_EDITOR
    public override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(Collider.bounds.center, _radiusExplosive);
    }
#endif

    public override void Update()
    {
        base.Update();
        MoveByHorizontal();
    }

    public override void Attacked(AttackType type, IAttackedable attacker = null)
    {
        if (!IsAlive || IsFreeze)
            return;

        switch (type)
        {
            case AttackType.Jump:
            case AttackType.Fire:
            case AttackType.Hit:
            case AttackType.Reflect:
            case AttackType.Dead:
                Debug.Log("Enemy " + transform.name + " explosive by " + type);
                StartCoroutine(Explosive(_timePrepare));
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

    private IEnumerator Explosive(float time)
    {
        // Prepare explosive
        Skeleton.loop = true;
        Skeleton.AnimationName = EnemyAnimationName.Attack;
        IsAlive = false;
        yield return new WaitForSeconds(time);

        // Explosive
        RaycastHit2D[] hits = Physics2D.CircleCastAll(Collider.bounds.center, _radiusExplosive, Vector2.zero);
        foreach (var hit in hits)
            if (hit.transform.CompareTag(ObjectTag.Player) || hit.transform.CompareTag(ObjectTag.Enemy))
            {
                IAttackedable character = hit.transform.GetComponent<IAttackedable>();
                if(character != null) character.Attacked(AttackType.Hit);
            }else if (hit.transform.CompareTag(ObjectTag.TileBlock))
            {
                IGoreable g = hit.transform.GetComponent<IGoreable>();
                if(g!= null) g.Gore();
            }

        Destroy(); 
    }

}
