using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossVine : BossBehaviour
{
    [SerializeField] private SpriteRenderer _root;
    [SerializeField] private Transform _head;
    [SerializeField] private bool canUpDown = true;
    [SerializeField] private float maximumLength = 5f;
    [SerializeField] private bool _isUp = false;
    [SerializeField] private BulletVine[] bullets;
    [SerializeField] private BombVine[] bombs;
    [SerializeField] private float _timePrepareAttack;

    [SerializeField] private float[] sizeGetFire;

    void OnEnable()
    {
        sizeGetFire = new float[2];
        for (int i = 0; i < sizeGetFire.Length; i++)
            sizeGetFire[i] = maximumLength / (sizeGetFire.Length + 1) * (i + 1);
    }

    void Update()
    {
        switch (BossState)
        {
            case BossState.None:
                BossState = BossState.Moving;
                break;
            case BossState.Moving:
                UpDown();
                break;
            case BossState.PrepareAttack:
                break;
            case BossState.Attacking:
                break;

        }
    }

    private void UpDown()
    {
        var positionX = Trans.position.x;
        var sizeY = _root.size.y;

        var deltaIncrease = _isUp ? Time.deltaTime * 5 : -Time.deltaTime * 5;

        sizeY = Mathf.Clamp(sizeY + deltaIncrease, 0, maximumLength);
        var positionY = sizeY / 2;
        Vector2 newSize = new Vector2(_root.size.x, sizeY);

        _root.size = newSize;
        _root.transform.localPosition = new Vector2(0, positionY);
        _head.localPosition = new Vector2(0f, positionY + .24f);

        if (sizeY >= maximumLength - float.Epsilon)
        {
            _isUp = !_isUp;
            BossState = BossState.Attacking;
            StartCoroutine(AttackByFire());
        }
        else if (sizeY <= float.Epsilon)
        {
            _isUp = !_isUp;
            BossState = BossState.Attacking;
            StartCoroutine(AttackByBomb());
        }
    }

    private IEnumerator AttackByFire()
    {
        var positionFire = Vector3.zero;

        var count = 0;
        while (true)
        {
            foreach (var b in bullets)
            {
                if (b.gameObject.activeSelf)
                    continue;
                positionFire.y = sizeGetFire[count % sizeGetFire.Length];
                b.gameObject.SetActive(true);
                b.transform.localPosition = positionFire;
                count++;
                break;
            }

            if (count >= 5)
                break;

            yield return new WaitForSeconds(.5f);
        }

        yield return new WaitForSeconds(2f);

        BossState = BossState.Moving;
    }

    private IEnumerator AttackByBomb()
    {
        ThrowBomb();

        yield return new WaitForSeconds(2f);

        BossState = BossState.Moving;
    }

    private void ThrowBomb()
    {
    }

    private void PrepareAttack()
    {

    }

    private void Attack()
    {

    }
}

