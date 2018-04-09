using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class SnakeHole : EnemyBehavior, ISnake
{
    /// <summary>
    /// is player stand on hole
    /// </summary>
    private bool _isPlayerStandOn = false;

    /// <summary>
    /// sequence action of snake
    /// </summary>
    private Sequence _actionOfSnake;

    protected override void OnEnable()
    {
        base.OnEnable();
        SetActionForSnake();
    }

    public override void Update()
    {
    }

    /// <summary>
    /// Action up and down of snake
    /// </summary>
    private void SetActionForSnake()
    {
        _actionOfSnake = DOTween.Sequence();

        _actionOfSnake.Append(Trans.DOMoveY(0f, 1f).SetRelative().SetEase(Ease.Linear).
            OnUpdate(CheckForGoOut));

        _actionOfSnake.Append(Trans.DOMoveY(2f, 1f).SetSpeedBased(true).
            SetRelative().SetEase(Ease.Linear).SetUpdate(UpdateType.Fixed));

        _actionOfSnake.Append(Trans.DOMoveY(-2f, 2f).SetSpeedBased(true)
            .SetRelative().SetEase(Ease.Linear).SetUpdate(UpdateType.Fixed).SetDelay(1f));

        _actionOfSnake.SetLoops(-1, LoopType.Restart);
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
}
