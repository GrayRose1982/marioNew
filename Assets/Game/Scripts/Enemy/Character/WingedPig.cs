using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WingedPig : EnemyBehavior, IWinged
{
    [SerializeField] private bool _isWinged = true;
    [SerializeField] private float _speedUp = 8f;
    public bool IsWinged
    {
        get { return _isWinged; }
        set { _isWinged = value; }
    }

#if UNITY_EDITOR
    public override void OnDrawGizmos()
    {
        Vector2 boxSize = Collider.bounds.size;
        boxSize.x = .1f;
        boxSize.y *= .5f;
        Vector2 position = Collider.bounds.center;

        position.y += boxSize.y / 2;
        if (IsRight)
            position.x = Collider.bounds.max.x + boxSize.x / 2 + .01f;
        else
            position.x = Collider.bounds.min.x - boxSize.x / 2 - .01f;

        Gizmos.DrawCube(position, boxSize);

        boxSize = Collider.bounds.size;
        boxSize.x *= .9f;
        boxSize.y = .2f;
        position = new Vector2(Collider.bounds.center.x, Collider.bounds.min.y);
        Gizmos.DrawCube(position, boxSize);

    }
#endif

    // Update is called once per frame
    public override void Update()
    {
        if (IsAlive && !IsFreeze)
        {
            CheckObjectStand();
            CheckBlockByRaycast();

            if (IsWinged)
            {
                if (Rigid.velocity.y < 0)
                    Jump();
            }
            else
            {
                Move(IsRight);
            }

            if (_platform)
            {
                bool inAxisX = transform.position.x < _platform.Collider.bounds.max.x &&
                               transform.position.x > _platform.Collider.bounds.min.x;
                bool inAxisY = transform.position.y > _platform.Collider.bounds.center.y;


                if (!(inAxisX && inAxisY))
                    _platform = null;

                if (!_platform) return;
                enemyTrans.position += _platform.Trans.position - _oldPlatformPosition;
                _oldPlatformPosition = _platform.Trans.position;
            }
        }
        else
            base.Update();
    }

    public override void CheckBlockByRaycast()
    {
        bool change = false;
        Vector2 boxSize = Collider.bounds.size;
        boxSize.x = .1f;
        boxSize.y *= .95f;
        Vector2 position = Collider.bounds.center;

        position.y += boxSize.y / 2;
        if (IsRight)
            position.x = Collider.bounds.max.x + boxSize.x / 2 + .01f;
        else
            position.x = Collider.bounds.min.x - boxSize.x / 2 - .01f;

        RaycastHit2D[] hitsBlock = Physics2D.BoxCastAll(position, boxSize, 0f, Vector2.zero, .1f, BlockLayer);
        change = hitsBlock.Length > 0;

        if (change && CanMove)
        {
            Vector2 velo = Rigid.velocity;
            velo.x = -velo.x;
            Rigid.velocity = velo;
            IsRight = !IsRight;
            Trans.localScale = new Vector3(-Trans.localScale.x, Trans.localScale.y, Trans.localScale.z);
        }
    }

    public void Jump()
    {
        Vector2 boxSize = Collider.bounds.size;
        boxSize.x *= .9f;
        boxSize.y = .2f;
        Vector2 position = new Vector2(Collider.bounds.center.x, Collider.bounds.min.y);

        RaycastHit2D[] hits = Physics2D.BoxCastAll(position, boxSize, 0f, Vector2.zero, 0, BlockLayer);

        if (hits.Length > 0)
            Rigid.velocity = new Vector2(IsRight ? Speed : -Speed, _speedUp);
    }

    public override void Attacked(AttackType type, IAttackedable attacker = null)
    {
        if (!IsAlive)
            return;

        switch (type)
        {
            case AttackType.Hit:
            case AttackType.Dead:
                IsAlive = false;
                Die();
                break;
            case AttackType.Jump:
            case AttackType.Fire:
            case AttackType.Reflect:
                Debug.Log("Enemy " + transform.name + " die by " + type);
                if (IsWinged)
                {
                    IsWinged = false;
                    Skeleton.AnimationName = EnemyAnimationName.Walk;
                }
                else
                {
                    IsAlive = false;
                    Die();
                }
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

    protected override void Freeze()
    {
        base.Freeze();
        Rigid.gravityScale = 3f;
    }

    protected override void UnFreeze()
    {
        base.UnFreeze();
        Rigid.gravityScale = 1f;

        Skeleton.AnimationName = IsWinged ? EnemyAnimationName.TempFly : EnemyAnimationName.Walk;
    }

    public override void Die()
    {
        Rigid.gravityScale = 3f;
        base.Die();
    }
}
