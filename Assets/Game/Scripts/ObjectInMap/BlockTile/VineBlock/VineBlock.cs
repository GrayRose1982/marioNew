using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VineBlock : BlockTile
{
    [SerializeField] private Vine _vine;
#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector2 startPos = transform.position;
        startPos.y += 1.29f;
        Vector2 endpos = transform.position;
        endpos.y += 10000f;
        RaycastHit2D[] hits = Physics2D.LinecastAll(startPos, endpos);

        foreach (var hit in hits)
            if (hit.transform.CompareTag(ObjectTag.ClimbBlock))
            {
                Gizmos.DrawLine(transform.position, hit.transform.position);
                break;
            }
    }
#endif

    public override void Gore()
    {
        if (!CanGore)
            return;

        base.Gore();
        UpDownTile();
        CanGore = false;
        Render.sprite = EmptySprite;
        Debug.Log("Hit vine");

        Vector2 startPos = transform.position;
        startPos.y += 1.29f;
        Vector2 endpos = transform.position;
        endpos.y += 10000f;
        RaycastHit2D[] hits = Physics2D.LinecastAll(startPos, endpos);

        foreach (var hit in hits)
            if (hit.transform.CompareTag(ObjectTag.ClimbBlock))
            {
                _vine.StartUp(hit.transform.position.y - transform.position.y);
                break;
            }

    }
}
