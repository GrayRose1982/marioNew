using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class BlockTile : MonoBehaviour, IGoreable
{
    /// <summary>
    /// Default position of block
    /// </summary>
    private Vector3 _defaultPosition;
    /// <summary>
    /// Is block can gore
    /// </summary>
    [SerializeField] protected bool CanGore = true;
    /// <summary>
    /// Sprite when block cant gore
    /// </summary>
    [SerializeField] private Sprite _emptySprite;
    [SerializeField] private SpriteRenderer _render;

    [SerializeField] private bool isBlock;
    public event Action<AttackType, IAttackedable> Gored;

    public SpriteRenderer Render
    {
        get { return _render; }
    }

    public Sprite EmptySprite
    {
        get { return _emptySprite; }
    }

    void Awake()
    {
        if (transform.Find("Demo") != null)
            Destroy(transform.Find("Demo").gameObject);
    }

    void Start()
    {
        InitBlock(transform.position);
    }

    public virtual void InitBlock(Vector3 position)
    {
        transform.position = position;
        _defaultPosition = position;
    }

    /// <summary>
    /// Action when was gored by player
    /// </summary>
    public virtual void Gore()
    {
        CallAction();
        if (isBlock)
            UpDownTile();
    }

    /// <summary>
    /// Action of block when break
    /// </summary>
    protected virtual void BreakTile()
    {
        //TODO: add animation break tile
    }

    /// <summary>
    /// Action of block when unbreak
    /// </summary>
    protected virtual void UpDownTile()
    {
        //TODO: add animation up and down, still unbreak
        //_tween.DOPlay();
        //_tween.DORestart();

        transform.DOKill();
        transform.position = _defaultPosition;
        transform.DOMoveY(.3f, .1f).SetLoops(2, LoopType.Yoyo).SetRelative(true);
    }

    protected void CallAction()
    {
        if (Gored != null)
        {
            print(Gored.Target.ToString());
            if (!Gored.Target.ToString().Equals("null"))
                Gored.Invoke(AttackType.Hit, null);
        }
    }
}
