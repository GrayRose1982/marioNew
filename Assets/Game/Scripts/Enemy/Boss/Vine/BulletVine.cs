using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BulletVine : MonoBehaviour {
    [SerializeField] private Vector3 _basePosition;
    [SerializeField] private Rigidbody2D _rigid;
    [SerializeField] private Vector2 _velo;

    void OnEnable()
    {
        _basePosition = transform.localPosition;
        StartCoroutine(Disable());
        Fire(false);
    }

    private IEnumerator Disable()
    {
        yield return new WaitForSeconds(3f);
        gameObject.SetActive(false);
    }

    public void Fire(bool isRight)
    {
        transform.localPosition = _basePosition;
        _velo.x = isRight ? Mathf.Abs(_velo.x) : -Mathf.Abs(_velo.x);
        _rigid.velocity = _velo;
    }

    void OnTriggerEnter2D(Collider2D hit)
    {
        if (hit.CompareTag(ObjectTag.Player))
        {
            transform.localPosition = _basePosition;
        }
    }
}
