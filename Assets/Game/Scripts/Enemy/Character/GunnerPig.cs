using UnityEngine;

public class GunnerPig : EnemyBehavior
{
    [SerializeField] private BulletCanonPig _bullets;
    [SerializeField] private float _timePerShoot = 2.8f;

    protected override void OnEnable()
    {
        base.OnEnable();
        transform.localScale = new Vector3(IsRight ?
                Mathf.Abs(transform.localScale.x) :
                -Mathf.Abs(transform.localScale.x),
            Mathf.Abs(transform.localScale.y),
            Mathf.Abs(transform.localScale.z));
    }

    void Start()
    {
        Active();
    }

    private void Active()
    {
        SetFire();
        Skeleton.loop = true;
        Skeleton.AnimationName = EnemyAnimationName.Attack;
    }

    private void SetFire()
    {
        float timeScale = 2.8f / _timePerShoot;
        Skeleton.timeScale = timeScale;
        Invoke("Shoot", _timePerShoot/2 + .1f/timeScale);
        Invoke("SetFire", _timePerShoot);
    }

    private void Shoot()
    {
        _bullets.Fire(IsRight);
    }

    public override void Attacked(AttackType type, IAttackedable attacker = null)
    {
        if (!IsAlive && IsFreeze)
            return;

        switch (type)
        {
            case AttackType.Jump:
            case AttackType.Fire:
            case AttackType.Hit:
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

    public override void Die()
    {
        CancelInvoke();
        Skeleton.loop = false;
        Skeleton.AnimationName = EnemyAnimationName.Dead;
        Invoke("Destroy", 2f);
    }

    protected override void Freeze()
    {
        CancelInvoke();
        base.Freeze();
    }

    protected override void UnFreeze()
    {
        base.UnFreeze();
        Active();
        Skeleton.loop = true;
        Skeleton.AnimationName = EnemyAnimationName.Attack;
    }
}
