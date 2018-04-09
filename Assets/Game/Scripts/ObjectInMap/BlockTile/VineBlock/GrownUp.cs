using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrownUp : AMoveable, IGetable
{
    [SerializeField] private PlayerType _typeGroupTo;

    void Start()
    {
        Spawn();
    }

    /// <summary>
    /// Spawn item when was gored
    /// </summary>
    public override void Spawn()
    {
        gameObject.SetActive(true);
        Rigid.isKinematic = true;
        Rigid.velocity = new Vector2(0, 2.5f);
        StartCoroutine(SpawnObject());
    }

    public override void Update()
    {
        if (CanMove)
            Move(IsRight);

        CheckBlockByRaycast();
    }

    IEnumerator SpawnObject()
    {
        yield return new WaitForSeconds(.5f);
        CanMove = true;
        Rigid.isKinematic = false;
    }

    /// <summary>
    /// Add effect to player 
    /// </summary>
    /// <param name="player"><see cref="PlayerController"/> object was effected</param>
    public virtual void EffectGetItem(PlayerController player)
    {
        if(!CanMove)
            return;
        
        player.GrownTo(_typeGroupTo);
        DestroyObject(this.gameObject);
    }
}
