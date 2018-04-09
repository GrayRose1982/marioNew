using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoleHaveSnake : MonoBehaviour
{
    private ISnake _snake;

    public ISnake Snake
    {
        get { return _snake ?? (_snake = GetComponentInChildren<ISnake>()); }
    }


    void OnTriggerEnter2D(Collider2D hit)
    {
        if (hit.CompareTag(ObjectTag.Player))
            if (Snake != null)
                Snake.CantGoOut();
    }

    void OnTriggerExit2D(Collider2D hit)
    {
        if (hit.CompareTag(ObjectTag.Player))
            if (Snake != null)
                Snake.CanGoOut();
    }

}
