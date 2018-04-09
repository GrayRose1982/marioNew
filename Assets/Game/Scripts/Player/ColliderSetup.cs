using UnityEngine;

public class ColliderSetup : MonoBehaviour
{
    [SerializeField] private bool isBig = false;
    [SerializeField] private BoxCollider2D _collider;        // Collider of player
    [SerializeField] private Vector2 _offsetPlayerUp;           // Offset collider when player become big
    [SerializeField] private float _radiusSizeBig;              // Radius of collider when player become big
    [SerializeField] private Vector2 _offsetPlayerDown;         // Offset collider when player become small
    [SerializeField] private float _radiusPlayerDown;           // Radius of collider when player become small

    /// <summary>
    /// Set <see cref="CircleCollider2D"/> to small to fix with small player
    /// </summary>
    public void ToSmall()
    {
        _collider.offset = _collider.offset/2;
        _collider.size = _collider.size / 2;
    }

    /// <summary>
    /// Set <see cref="CircleCollider2D"/> to big to fix with big player
    /// </summary>
    public void ToBig()
    {
        _collider.offset = _offsetPlayerUp;
    }
}
