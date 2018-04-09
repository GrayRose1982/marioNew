using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonSpeedUp :MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        PlayerController.IsSpeedUp = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        PlayerController.IsSpeedUp = false;
    }
}
