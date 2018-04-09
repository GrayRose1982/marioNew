using System.Collections;
using UnityEngine;

public class Vine : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _root;
    [SerializeField] private Transform _top;
    [SerializeField] private BoxCollider2D _box;

    public void StartUp(float lenght)
    {
        _root.tileMode = SpriteTileMode.Continuous;
        StartCoroutine(GrownBean(.02f, lenght));
    }

    IEnumerator GrownBean(float time, float lenght)
    {
        WaitForSeconds w = new WaitForSeconds(time);
        while (true)
        {
            float sizeY = _root.size.y;
            float positionY;
            sizeY = Mathf.Clamp(sizeY + Time.fixedDeltaTime * 5, 0, lenght);
            positionY = sizeY / 2 ;
            Vector2 newSize = new Vector2(_root.size.x, sizeY);
            _root.size = newSize ;
            _root.transform.localPosition = new Vector2(0, positionY);
            _box.size = newSize;
            _top.localPosition = new Vector2(-.05f, positionY * 2 + 1.28f/2);
            yield return w;

            if (sizeY >= lenght)
                break;
        }
    }
}
