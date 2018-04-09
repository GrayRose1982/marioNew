using System.Collections;
using System.Collections.Generic;
using System.Timers;
using UnityEngine;

public class Stalactite : MonoBehaviour
{
    [SerializeField] private Transform _trans;
    [SerializeField] private float _timeShake = .5f;
    [SerializeField] private float _angleShake = 10f;
    [SerializeField] private float _deltaTime = .01f;
    [SerializeField] private float _distanceStartFall = 1.28f / 2 * 3;
    [SerializeField] private float _gravityScaleBase = 2f;
    [SerializeField] private bool _isStart = false;

    void OnEnable()
    {
        _isStart = false;
    }

    void Start()
    {
        if (!_trans)
            _trans = transform;
    }

    void Update()
    {
        if (PlayerController.Instance && !_isStart)
        {
            var distancePlayer = PlayerController.Instance.Trans.position.x - _trans.position.x;

            if (Mathf.Abs(distancePlayer) <= _distanceStartFall)
                StartCoroutine(StartShake());
        }
    }

    private IEnumerator StartShake()
    {
        _isStart = true;
        float timer = 0;
        var toRight = true;
        var speedRotate = 150f;
        float currentRotate = 0f;
        while (true)
        {
            timer += _deltaTime;

            float angle = toRight ? speedRotate : -speedRotate;

            _trans.Rotate(0, 0, angle * _deltaTime);

            currentRotate = _trans.rotation.eulerAngles.z;
            currentRotate = currentRotate > 180 ? currentRotate - 360 : currentRotate;

            if (Mathf.Abs(currentRotate) > _angleShake)
                toRight = !toRight;

            yield return new WaitForSeconds(_deltaTime);

            if (timer > _timeShake)
            {
                Rigidbody2D rigid = GetComponent<Rigidbody2D>();
                rigid.isKinematic = false;
                rigid.gravityScale = 2;
                break;
            }
        }
    }

    void OnTriggerEnter2D(Collider2D obj)
    {
        gameObject.SetActive(false);
    }
}
