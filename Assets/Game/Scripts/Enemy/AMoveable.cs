using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AMoveable : MonoBehaviour
{
    [SerializeField] protected float Speed = 2f;            // Base speed of object
    [SerializeField] protected Collider2D Collider;         // Collider bound object
    [SerializeField] protected Rigidbody2D Rigid;            // Rigidbody2d of object
    [SerializeField] protected bool CanMove = true;        // does object can move
    [SerializeField] protected bool IsRight = true;         // is object move righ

    [SerializeField] protected LayerMask BlockLayer ;

    /// <summary>
    /// Moving object in axis X follow speed
    /// </summary>
    /// <param name="isRight">is enemy moving right</param>
    public virtual void Move(bool isRight)
    {
        Vector2 currentVelocity = Rigid.velocity;
        currentVelocity.x = isRight ? Speed : -Speed;
        Rigid.velocity = currentVelocity;
    }

    /// <summary>
    /// How object spawn
    /// </summary>
    public virtual void Spawn()
    {
        gameObject.SetActive(true);
        CanMove = true;
    }

    public virtual void Update()
    {
       
    }

    /// <summary>
    /// Check in left and right by raycast.
    /// Detect barrack to change direction
    /// </summary>
    public virtual void CheckBlockByRaycast()
    {
        bool change = false;
        Vector2 boxSize = Collider.bounds.size;
        boxSize.x = .1f;
        boxSize.y *= .5f;
        Vector2 position = Rigid.position;
        if (IsRight)
        {
            position.x += Collider.bounds.size.x / 2 + boxSize.x / 2;
            RaycastHit2D[] hitsBlock = Physics2D.BoxCastAll(position, boxSize, 0f, Vector2.zero, .1f, BlockLayer);
            change = hitsBlock.Length > 0;
        }
        else
        {
            position.x -= Collider.bounds.size.x / 2 + boxSize.x / 2;
            RaycastHit2D[] hitsBlock = Physics2D.BoxCastAll(position, boxSize, 0f, Vector2.zero, .1f, BlockLayer);
            change = hitsBlock.Length > 0;
        }

        if (change && CanMove)
            IsRight = !IsRight;
    }


#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Vector2 boxSize = Collider.bounds.size;
        boxSize.x = .1f;
        boxSize.y *= .5f;

        Vector2 position = Rigid.position;
        position.x += IsRight
            ? Collider.bounds.size.x / 2 + boxSize.x / 2
            : -(Collider.bounds.size.x / 2 + boxSize.x / 2);
        Gizmos.DrawWireCube(position, boxSize);
    }
#endif
}
