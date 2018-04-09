using System;
using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;

public class BossBehaviour : MonoBehaviour, ICharacter, IAttackedable
{
    #region Variable
    /// <summary>
    /// Is character alive
    /// </summary>
    [SerializeField] private bool _isAlive;

    /// <summary>
    /// Is character can move
    /// </summary>
    [SerializeField] private bool _canMove;
    /// <summary>
    /// <see cref="Transform"/> trans of character
    /// </summary>
    [SerializeField] private Transform _trans;
    [SerializeField] private Rigidbody2D _rigid;
    [SerializeField] private SkeletonAnimation _skeleton;
    [SerializeField] private Collider2D _collider;
    [SerializeField] private int _maxHealth;
    [SerializeField] private int _currentHealth;
    [SerializeField] private bool _isUndead;
    [SerializeField] private BossState _bossState;
    [SerializeField] private float _timer;
    #endregion

    #region Properties
    public bool IsAlive
    {
        get { return _isAlive; }
        private set { _isAlive = value; }
    }

    public bool IsFreeze
    {
        get { return false; }
    }

    public bool CanMove
    {
        get { return _canMove; }
        private set { _canMove = value; }
    }

    public Transform Trans { get { return _trans ?? (_trans = transform); } }

    public Rigidbody2D Rigid { get { return _rigid ?? (_rigid = GetComponent<Rigidbody2D>()); } }

    public Collider2D Col { get { return _collider ?? (_collider = GetComponent<Collider2D>()); } }

    public SkeletonAnimation Skeleton { get { return _skeleton ?? (_skeleton = GetComponent<SkeletonAnimation>()); } }

    public event Action UnfreezeAction;

    public int MaxHealth
    {
        get { return _maxHealth; }
    }

    public int CurrentHealth
    {
        get { return _currentHealth; }
        protected set { _currentHealth = value; }
    }
    public bool IsUndead
    {
        get { return _isUndead; }
        protected set { _isUndead = value; }
    }

    public BossState BossState
    {
        get { return _bossState; }
        protected set
        {
            _timer = 0;
            _bossState = value;
        }
    }

    #endregion

#if UNITY_EDITOR
    public void OnDrawGizmos()
    {

    }
#endif

    public void Attacked(AttackType type, IAttackedable attacker = null)
    {
        if (!IsAlive)
            return;
        Debug.Log("herhe");
        switch (type)
        {
            case AttackType.Jump:
            case AttackType.Fire:
            case AttackType.Hit:
            case AttackType.Reflect:
            case AttackType.Ice:
            case AttackType.Dead:
                TakeDamage(type);
                break;
            default:
                Debug.Log("Dont have case " + type + "");
                break;
        }
    }

    protected virtual void TakeDamage(AttackType type)
    {
        Debug.Log("Boss " + transform.name + " take damage " + type + ": current hp = " + _currentHealth);

        _currentHealth -= 1;
        if (CurrentHealth <= 0)
            Dead();
    }

    protected virtual void Dead()
    {
        Debug.Log("Boss " + transform.name + " take damage and dead");
    }
}

public enum BossState
{
    None = 0,
    PrepareAttack  = 1,
    Attacking = 2,
    Moving = 3,
}