using System;
using System.Collections;
using System.Collections.Generic;
using Spine;
using Spine.Unity;
using UnityEngine;

public class EnemyBehavior : AMoveable, ICharacter, IAttackedable, IThrowable
{
    /// <summary>
    /// Animation controller of enemey
    /// </summary>
    [SerializeField] protected SkeletonAnimation Skeleton;

    /// <summary>
    /// Transform of enemy
    /// </summary>
    [SerializeField] protected Transform enemyTrans;

    /// <summary>
    /// Ice block of enemy
    /// </summary>
    [SerializeField] protected GameObject _iceBlock;

    /// <summary>
    /// State of enemy, is throw by player
    /// </summary>
    [SerializeField] protected bool _isThrow = false;

    /// <summary>
    /// Platform enemy stand on
    /// </summary>
    protected Platform _platform;
    protected Vector3 _oldPlatformPosition;
    [SerializeField] protected List<IGoreable> TileBlockStand;

    #region Properties

    public bool IsAlive { get; protected set; }
    public bool IsFreeze { get; protected set; }
    public new bool CanMove
    {
        get { return base.CanMove; }
        set { base.CanMove = value; }
    }

    public Transform Trans { get { return enemyTrans; } }

    public Rigidbody2D Rigid
    {
        get
        {
            if (!base.Rigid)
                base.Rigid = GetComponent<Rigidbody2D>();
            return base.Rigid;
        }
    }

    public Collider2D Col
    {
        get { return Collider; }
    }

    public event Action UnfreezeAction;

    #endregion

#if UNITY_EDITOR
    public virtual void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        if (!_isThrow)
        {
            Vector2 boxSize = Collider.bounds.size;
            boxSize.x = .1f;
            boxSize.y *= .5f;
            Vector2 position = new Vector2(IsRight ? Collider.bounds.max.x : Collider.bounds.min.x,
                Collider.bounds.max.y - boxSize.y / 2);

            position.x += IsRight
                ? boxSize.x / 2
                : -(boxSize.x / 2);
            Gizmos.DrawWireCube(position, boxSize);

            boxSize = Collider.bounds.size;
            boxSize.x = .6f;
            boxSize.y = 2f;
            position = new Vector2(IsRight ? Collider.bounds.max.x + .1f : Collider.bounds.min.x - .1f,
                Collider.bounds.min.y);

            //position.x += IsRight ? boxSize.x / 2 : -(boxSize.x / 2);
            Gizmos.DrawWireCube(position, boxSize);

            boxSize = Collider.bounds.size;
            boxSize.y = .1f;
            position = new Vector2(Collider.bounds.center.x, Collider.bounds.min.y - boxSize.y);

            Gizmos.DrawWireCube(position, boxSize);

        }
        else
        {
            Gizmos.DrawWireCube(Col.bounds.center, Col.bounds.size);
        }
    }
#endif

    protected virtual void OnEnable()
    {
        IsAlive = true;
        Collider.isTrigger = false;
        TileBlockStand = new List<IGoreable>();
    }

    void Start()
    {
        InitBase();
    }

    void OnDisable()
    {
        CancelInvoke();
    }

    // Update is called once per frame
    public override void Update()
    {
        if (_isThrow)
            CreateDamage();
    }

    protected void MoveByHorizontal()
    {
        if (IsAlive && !IsFreeze)
        {
            if (CanMove)
                Move(IsRight);

            CheckBlockByRaycast();
            CheckHoleByRayCast();
            CheckObjectStand();
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

    public virtual void Attacked(AttackType type, IAttackedable attacker = null)
    {
        if (!IsAlive)
            return;

        switch (type)
        {
            case AttackType.Jump:
            case AttackType.Fire:
            case AttackType.Hit:
            case AttackType.Reflect:
            case AttackType.Dead:
                Debug.Log("Enemy " + transform.name + " die by " + type);
                IsAlive = false;
                Die();
                break;
            case AttackType.Ice:
                if (Skeleton)
                    Skeleton.AnimationName = EnemyAnimationName.Idle;
                Freeze();
                break;
            default:
                Debug.Log("Dont have case " + type + "");
                break;
        }
    }

    /// <summary>
    /// override funtion move to flip character when move
    /// </summary>
    /// <param name="isRight"></param>
    public override void Move(bool isRight)
    {
        Vector2 currentVelocity = Rigid.velocity;
        currentVelocity.x = isRight ? Speed : -Speed;
        Rigid.velocity = currentVelocity;
        float currentScale = enemyTrans.localScale.x;
        if (currentVelocity.x < Mathf.Epsilon ^ currentScale < 0)
            enemyTrans.localScale = new Vector3(-currentScale, enemyTrans.localScale.y, enemyTrans.localScale.z);
    }

    /// <summary>
    /// Change raycast because of size
    /// </summary>
    public override void CheckBlockByRaycast()
    {
        Vector2 boxSize = Collider.bounds.size;
        boxSize.x = .1f;
        boxSize.y *= .5f;
        Vector2 position = new Vector2(IsRight ? Collider.bounds.max.x : Collider.bounds.min.x, Collider.bounds.max.y - boxSize.y / 2);

        position.x += IsRight
            ? boxSize.x / 2
            : -(boxSize.x / 2);

        RaycastHit2D[] hitsBlock = Physics2D.BoxCastAll(position, boxSize, 0f, Vector2.zero, .1f, BlockLayer);
        var change = hitsBlock.Length > 0;

        if (change)
            IsRight = !IsRight;
    }

    /// <summary>
    /// Create raycast to check is in front of character have hole
    /// </summary>
    public virtual void CheckHoleByRayCast()
    {
        Vector2 boxSize = Collider.bounds.size;
        boxSize.x = .6f;
        boxSize.y = 2f;
        Vector2 position = new Vector2(IsRight ? Collider.bounds.max.x + .1f : Collider.bounds.min.x - .1f, Collider.bounds.min.y);

        RaycastHit2D[] hits = Physics2D.BoxCastAll(position, boxSize, 0f, Vector2.zero);

        if (hits.Length <= 1)
            IsRight = !IsRight;
    }

    /// <summary>
    /// Create raycast to check is character stand on block
    /// </summary>
    public virtual void CheckObjectStand()
    {
        Vector2 boxSize = Collider.bounds.size;
        boxSize.y = .1f;
        Vector2 position = new Vector2(Collider.bounds.center.x, Collider.bounds.min.y - boxSize.y);

        RaycastHit2D[] hits = Physics2D.BoxCastAll(position, boxSize, 0f, Vector2.zero);

        bool[] itemBlock = new bool[TileBlockStand.Count];
        for (int i = 0; i < itemBlock.Length; i++)
            itemBlock[i] = false;

        foreach (var hit in hits)
            if (hit.transform.CompareTag(ObjectTag.TileBlock))
            {
                IGoreable t = hit.transform.GetComponent<IGoreable>();
                int indexTile = TileBlockStand.IndexOf(t);
                if (t != null && indexTile == -1)
                {
                    t.Gored += Attacked;
                    TileBlockStand.Add(t);
                }
                else if (indexTile != -1)
                    itemBlock[indexTile] = true;
            }
            else if (hit.transform.CompareTag(ObjectTag.Platform))
            {
                if (_platform)
                    continue;
                Debug.Log("Stand on platform");

                _platform = hit.transform.GetComponent<Platform>();
                _oldPlatformPosition = _platform.Trans.position;
            }


        for (int i = itemBlock.Length - 1; 0 <= i; i--)
            if (!itemBlock[i])
            {
                TileBlockStand[i].Gored -= Attacked;
                TileBlockStand.RemoveAt(i);
            }
    }


    /// <summary>
    /// Init base data to character
    /// </summary>
    protected virtual void InitBase()
    {
        IsAlive = true;
    }

    /// <summary>
    /// Action when enemy die becase attack of player
    /// </summary>
    public virtual void Die()
    {
        if(Collider)
        Collider.isTrigger = true;
        foreach (var goreable in TileBlockStand)
            goreable.Gored -= Attacked;

        Rigid.velocity = new Vector2(Rigid.velocity.x > 0 ? 5f : -5f, 5f);
        Invoke("Destroy", 2f);
    }

    public virtual void Destroy()
    {
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Freeze character 
    /// </summary>
    protected virtual void Freeze()
    {
        CancelInvoke();
        gameObject.layer = LayerMask.NameToLayer("Block");
        IsFreeze = true;
        _iceBlock.SetActive(true);
        Rigid.mass = 10000f;
        Invoke("UnFreeze", 10f);
    }

    /// <summary>
    /// UnfreezeAction character 
    /// </summary>
    protected virtual void UnFreeze()
    {
        gameObject.layer = LayerMask.NameToLayer("Enemy");
        Rigid.bodyType = RigidbodyType2D.Dynamic;
        _iceBlock.SetActive(false);
        IsFreeze = false;
        Rigid.mass = 1f;
        Trans.SetParent(null);
        Col.enabled = true;
        Skeleton.loop = true;
        CanMove = true;
        Skeleton.AnimationName = EnemyAnimationName.Walk;

        if (UnfreezeAction != null)
        {
            UnfreezeAction.Invoke();
            UnfreezeAction = null;
        }
    }

    public void Throw(Vector2 direction)
    {
        CancelInvoke();
        Rigid.bodyType = RigidbodyType2D.Dynamic;
        Rigid.mass = 1f;
        Rigid.velocity = direction;
        Col.enabled = true;
        gameObject.layer = LayerMask.NameToLayer("Enemy");
        gameObject.tag = ObjectTag.Bullet;
        _isThrow = true;
    }

    /// <summary>
    /// Create damage after throw
    ///  /// </summary>
    protected void CreateDamage()
    {
        RaycastHit2D[] hits = Physics2D.BoxCastAll(Col.bounds.center, Col.bounds.size, 0, Vector2.zero);
        Debug.Log("Create raycast");
        foreach (var hit in hits)
        {
            Debug.Log(hit.transform.name);
            if (hit.transform.CompareTag(ObjectTag.Enemy))
            {
                Crash();

                IAttackedable attacked = hit.transform.GetComponent<IAttackedable>();
                attacked.Attacked(AttackType.Hit);
            }
            else
            if (!(hit.transform.CompareTag(ObjectTag.Bullet) ||
                 hit.transform.CompareTag(ObjectTag.Player)))
            {
                Crash();
            }
        }
    }

    /// <summary>
    /// Destroy character after throw
    /// </summary>
    protected virtual void Crash()
    {
        gameObject.SetActive(false);
    }
}
