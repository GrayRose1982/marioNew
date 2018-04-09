using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class HelmetChicken : EnemyBehavior, IShield
{
    private int _numberShield = 1;
    private bool _isRole = false;
    [SerializeField] private float _speedRole = 8f;

    public int NumberShield
    {
        get { return _numberShield; }
        private set { _numberShield = value; }
    }

    public bool IsShield
    {
        get { return _numberShield > 0; }
    }

    public bool IsRole
    {
        get { return _isRole; }
    }

    void OnDisable()
    {
        CancelInvoke();
    }

#if UNITY_EDITOR
    public override void OnDrawGizmos()
    {
        if (IsRole)
        {
            Vector2 size = Collider.bounds.size;
            Vector2 position = Collider.bounds.center;
            size.y *= .2f;
            size.x *= .8f;
            position.x += IsRight ? size.x / 4 : -size.x / 4;

            Gizmos.DrawCube(position, size);
        }
        else
            base.OnDrawGizmos();
    }
#endif


    public override void Update()
    {
        if (!IsShield && _isRole && IsAlive)
        {
            Role(IsRight);
            RoleDamage();
        }
        else
        {
            MoveByHorizontal();
            base.Update();
        }
    }

    protected override void UnFreeze()
    {
        base.UnFreeze();
        PrepareMove();
        Skeleton.loop = true;
        Skeleton.AnimationName = EnemyAnimationName.Run;
    }

    public override void Attacked(AttackType type, IAttackedable attacker = null)
    {
        if (!IsAlive && IsFreeze)
            return;

        switch (type)
        {
            case AttackType.Jump:
                if (IsShield)
                {
                    // Character was hit by player, become idle, lose shield
                    Skeleton.loop = false;
                    Skeleton.AnimationName = EnemyAnimationName.Attack;
                    CanMove = false;
                    _numberShield -= 1;
                    Invoke("PrepareMove", 2f);
                }
                else
                {
                    // Character jumped again by player, role or stop role
                    CancelInvoke();
                    _isRole = !_isRole;

                    if (attacker != null)
                        IsRight = (((ICharacter)attacker).Trans.position.x - Trans.position.x) < 0;

                    if (!_isRole)
                        StopRole();
                }
                break;
            case AttackType.Fire:
            case AttackType.Hit:
            case AttackType.Reflect:
                IsAlive = false;
                Die();
                Debug.Log("Enemy " + transform.name + " die by " + type);
                break;
            case AttackType.Ice:
                Skeleton.loop = false;
                Skeleton.AnimationName = EnemyAnimationName.Attack;
                _isRole = false;
                    StopRole();
                Freeze();
                break;
            default:
                Debug.Log("Dont have case " + type + "");
                break;
        }
    }

    /// <summary>
    /// Prepare move again after jumped
    /// </summary>
    private void PrepareMove()
    {
        _numberShield = 1;
        Skeleton.loop = false;
        Skeleton.AnimationName = EnemyAnimationName.Prepare;
        Invoke("StartMove", 1f);
    }

    /// <summary>
    /// Start move again after jumped
    /// </summary>
    private void StartMove()
    {
        CanMove = true;
        Skeleton.loop = true;
        Skeleton.AnimationName = EnemyAnimationName.Walk;
    }

    /// <summary>
    /// Start role after jump twice
    /// </summary>
    /// <param name="isRight"></param>
    private void Role(bool isRight)
    {
        Vector2 currentVelocity = Rigid.velocity;
        currentVelocity.x = isRight ? _speedRole : -_speedRole;
        Rigid.velocity = currentVelocity;
        float currentScale = enemyTrans.localScale.x;
        if (currentVelocity.x < Mathf.Epsilon ^ currentScale < 0)
            enemyTrans.localScale = new Vector3(-currentScale, enemyTrans.localScale.y, enemyTrans.localScale.z);
    }

    private void StopRole()
    {
        Rigid.velocity = Vector2.zero;
        Invoke("PrepareMove", 2f);
    }

    /// <summary>
    /// Create damage when role
    /// </summary>
    protected void RoleDamage()
    {
        Vector2 size = Collider.bounds.size;
        Vector2 position = Collider.bounds.center;
        size.y *= .2f;
        size.x *= .8f;
        position.x += IsRight ? size.x / 4 : -size.x /4;
        RaycastHit2D[] hits = Physics2D.BoxCastAll(position, size, 0, Vector2.zero);

        bool changeDirection = false;

        foreach (var hit in hits)
            if (hit.transform.CompareTag(ObjectTag.Player))
            {
                IAttackedable character = hit.transform.GetComponent<IAttackedable>();
                character.Attacked(AttackType.Reflect, this);
            }
            else if (hit.transform.CompareTag(ObjectTag.Enemy))
            {
                if (hit.transform.name == Trans.name)
                    continue;

                IAttackedable character = hit.transform.GetComponent<IAttackedable>();
                character.Attacked(AttackType.Reflect);
            }
            else if (hit.transform.CompareTag(ObjectTag.TileBlock))
            {
                changeDirection = true;
                IGoreable tile = hit.transform.GetComponent<IGoreable>();
                tile.Gore();
            }
            else if (hit.transform.CompareTag(ObjectTag.Ground))
                changeDirection = true;

        if (changeDirection)
            IsRight = !IsRight;
    }
}
