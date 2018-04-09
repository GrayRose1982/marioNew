using DG.Tweening;
using UnityEngine;

public class SnakeHoleFire : EnemyBehavior, ISnake
{
    /// <summary>
    /// is player stand on hole
    /// </summary>
    private bool _isPlayerStandOn = false;

    /// <summary>
    /// Bullet of the snakeS
    /// </summary>
    [SerializeField] private Bullet _bullet;
    /// <summary>
    /// Attack type of snake
    /// </summary>
    [SerializeField] private AttackType _attackType = AttackType.Fire;

    /// <summary>
    /// sequence action of snake
    /// </summary>
    private Sequence _actionOfSnake;

    [SerializeField] private Transform _parent;

    protected override void OnEnable()
    {
        base.OnEnable();
        SetActionForSnake();
    }

    void OnDisable()
    {
        _actionOfSnake.Kill();
    }

    public override void Update()
    {
        base.Update();
        if (!IsAlive || IsFreeze)
            _actionOfSnake.Pause();
    }

    /// <summary>
    /// Action up and down of snake
    /// </summary>
    private void SetActionForSnake()
    {
        _actionOfSnake = DOTween.Sequence();

        _actionOfSnake.Append(Trans.DOMoveY(0f, 1f).SetRelative().SetEase(Ease.Linear).
            OnUpdate(CheckForGoOut));

        _actionOfSnake.Append(Trans.DOMoveY(2f, 1f).SetSpeedBased(true)
            .SetRelative().SetEase(Ease.Linear).SetUpdate(UpdateType.Fixed)
            .OnComplete(Fire).OnStepComplete(LookToPlayer));

        _actionOfSnake.Append(Trans.DOMoveY(-2f, 2f).SetSpeedBased(true)
            .SetRelative().SetEase(Ease.Linear).SetUpdate(UpdateType.Fixed).SetDelay(1f)
        );

        _actionOfSnake.SetLoops(-1, LoopType.Restart);
    }

    private void Fire()
    {
        if (!_bullet)
            return;
        Vector2 direction = (PlayerController.Instance.Trans.position - Trans.position).normalized;
        Vector2 positionFire = Trans.position;
        positionFire.y += 1f;
        _bullet.FireBullet(_attackType, direction, positionFire);
    }

    private void LookToPlayer()
    {
        Vector2 direction = (PlayerController.Instance.Trans.position - Trans.position);
        if (direction.x > 0 ^ Trans.localScale.x > 0)
        {
            Trans.localScale = new Vector3(-Trans.localScale.x, Trans.localScale.y, Trans.localScale.z);
        }
    }

    /// <summary>
    /// Snake check is player stant on? if not, snake and go out
    /// </summary>
    private void CheckForGoOut()
    {
        if (_isPlayerStandOn)
            _actionOfSnake.Pause();
    }


    public void CanGoOut()
    {
        _isPlayerStandOn = false;
        _actionOfSnake.Play();
    }

    public void CantGoOut()
    {
        _isPlayerStandOn = true;
    }

    protected override void Freeze()
    {
        base.Freeze();
        _actionOfSnake.Pause();
    }

    protected override void UnFreeze()
    {
        base.UnFreeze();
        _actionOfSnake.Play();
        Rigid.isKinematic = true;
    }

    public override void Attacked(AttackType type, IAttackedable attacker = null)
    {
        if (!IsAlive || IsFreeze)
            return;

        switch (type)
        {
            case AttackType.Jump:
                if (attacker != null)
                    attacker.Attacked(AttackType.Reflect, this);
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
                if (Skeleton)
                    Skeleton.AnimationName = EnemyAnimationName.Idle;
                Freeze();
                break;
            default:
                Debug.Log("Dont have case " + type + "");
                break;
        }
    }

    public override void Destroy()
    {
        base.Destroy();
        Trans.SetParent(_parent);
    }

    protected override void Crash()
    {
        base.Crash();
        Trans.SetParent(_parent);
    }
}
