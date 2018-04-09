using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonMove : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private PlayerMoveDirection _direction;

    public void OnPointerEnter(PointerEventData eventData)
    {
        PlayerController.Direction = _direction;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (PlayerController.Direction == _direction)
            PlayerController.Direction = PlayerMoveDirection.None;
    }
}
