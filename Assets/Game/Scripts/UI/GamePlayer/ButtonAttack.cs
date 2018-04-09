using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonAttack : MonoBehaviour , IPointerEnterHandler, IPointerDownHandler{

    public void OnPointerEnter(PointerEventData eventData)
    {
        PlayerController.AttackAction.Invoke();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        PlayerController.AttackAction.Invoke();
    }

}