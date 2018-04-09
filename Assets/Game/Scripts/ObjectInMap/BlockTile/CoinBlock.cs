using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class CoinBlock : BlockTile
{
    [SerializeField] private bool _canGore = true;
    [SerializeField] private Transform[] _coinTrans;
    public int NumberCoin;

    public override void Gore()
    {
        if (!_canGore)
            return;

        CallAction();

        _canGore = false;
        UpDownTile();

        Transform coinAnamtion = _coinTrans[NumberCoin % _coinTrans.Length];
        coinAnamtion.transform.DOMoveY(2.5f, .3f).SetRelative();
        DestroyObject(coinAnamtion.transform.gameObject, .3f);
        PlayerData.GetCoin();

        NumberCoin--;
        Invoke("CheckOutNumberCoin", .1f);
    }

    public void CheckOutNumberCoin()
    {
        _canGore = !(NumberCoin <= 0);
        if (!_canGore)
            Render.sprite = EmptySprite;
    }

}
