using System.Collections;
using System.Collections.Generic;
using System.Timers;
using UnityEngine;

public class ChainFire : MonoBehaviour
{
    [SerializeField] private Transform _trans;
    [SerializeField] private bool _isRight;
    [SerializeField] private float _speedRotate = 10f;

    // Update is called once per frame
    void Update()
    {
        _trans.Rotate(0, 0, (_isRight ? _speedRotate : -_speedRotate)* Time.deltaTime);
    }

    public void Init()
    {

    }
}
