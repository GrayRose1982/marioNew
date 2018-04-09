using DG.Tweening;
using UnityEngine;

public class ItemBlock : BlockTile
{
    /// <summary>
    /// Object help player grown up
    /// </summary>
    [SerializeField] public AMoveable GrownObject;

    /// <summary>
    /// Action when was gored by player
    /// </summary>
    public override void Gore()
    {
        if(!CanGore)
            return;

        CanGore = false;
        UpDownTile();
        Render.sprite = EmptySprite;
        // Spawn item
        if (GrownObject != null)
            GrownObject.Spawn();
    } 
}
