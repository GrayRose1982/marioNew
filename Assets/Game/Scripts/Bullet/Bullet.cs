using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting;
using UnityEngine;
using UnityEngine.Assertions.Must;

public class Bullet : MonoBehaviour
{
    /// <summary>
    /// Type attack of bullet
    /// </summary>
    [SerializeField] private AttackType _type;

    /// <summary>
    /// Collider of bullet
    /// </summary>
    [SerializeField] private Collider2D _collider;

    /// <summary>
    /// Renderer image
    /// </summary>
    [SerializeField] private SpriteRenderer _rendder;

    /// <summary>
    /// Self game object
    /// </summary>
    [SerializeField] private GameObject _gameObject;

    /// <summary>
    /// Self rigid body2d
    /// </summary>
    [SerializeField] private Rigidbody2D _rigid;
    [SerializeField] private bool _isRight;

    /// <summary>
    /// Is this bullet fire by player
    /// </summary>
    [SerializeField] private bool _isPlayerFire;

    [SerializeField] private Color _red;
    [SerializeField] private Color _blue;

    public bool IsActive
    {
        get { return _gameObject.activeInHierarchy; }
    }

#if UNITY_EDITOR
    public void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(_collider.bounds.center, _collider.bounds.size.x / 2 * 1.05f);
    }
#endif

    void OnEnable()
    {
        Invoke("DestroyBullet", 2f);
    }

    void OnDisable()
    {
        CancelInvoke();
    }

    void Update()
    {
        CheckCollision();
        DrawRaycast();
        transform.Rotate(Vector3.forward * Time.deltaTime * 1000f * (_isRight ? -1 : 1));
    }

    public void FireBullet(AttackType type, bool isRight, Vector2 position)
    {
        _gameObject.transform.position = position;
        _gameObject.SetActive(true);
        float speed = 20f;
        _isRight = isRight;
        _rigid.velocity = Vector2.right * (_isRight ? speed : -speed);
        _type = type;

        if (_type == AttackType.Ice)
            _rendder.color = _blue;
        else if (_type == AttackType.Fire)
            _rendder.color = _red;

    }

    public void FireBullet(AttackType type, Vector2 direction, Vector2 position)
    {
        _rigid.gravityScale = 0f;
        _gameObject.SetActive(true);
        transform.position = position;
        float speed = 10f;
        _rigid.velocity = speed * direction;

        _isRight = direction.x > 0;

        _type = type;

        if (_type == AttackType.Ice)
            _rendder.color = _blue;
        else if (_type == AttackType.Fire)
            _rendder.color = _red;

    }

    void DrawRaycast()
    {
        RaycastHit2D[] hits = Physics2D.CircleCastAll(_collider.bounds.center, _collider.bounds.size.x / 2 * 1.05f, Vector2.zero);
        foreach (var obj in hits)
        {
            bool isHitCharacter = false;
            if (_isPlayerFire)
                isHitCharacter = obj.transform.CompareTag(ObjectTag.Enemy);
            else
                isHitCharacter = obj.transform.CompareTag(ObjectTag.Player);

            if (isHitCharacter)
            {
                IAttackedable attacked = obj.transform.GetComponent<IAttackedable>();
                Debug.Log(obj.transform.name);
                attacked.Attacked(_type);
                DestroyBullet();
            }
            else if (!obj.transform.CompareTag(ObjectTag.Bullet) &&
                     !obj.transform.CompareTag(ObjectTag.Enemy) &&
                     !obj.transform.CompareTag(ObjectTag.Player))
                if (_isPlayerFire)
                    _rigid.velocity = new Vector2(_isRight ? 20f : -20f, 10f);
                else
                    DestroyBullet();
        }
    }

    /// <summary>
    /// Destroy bullet. can just disable game object
    /// </summary>
    void DestroyBullet()
    {
        _gameObject.SetActive(false);
    }

    void CheckCollision()
    {
        if (Mathf.Abs(_rigid.velocity.x) <= float.Epsilon)
            DestroyBullet();
    }
}
