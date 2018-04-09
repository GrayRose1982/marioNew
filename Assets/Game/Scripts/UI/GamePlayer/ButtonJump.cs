using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonJump : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (PlayerController.JumpState == JumpState.None)
            PlayerController.JumpState = JumpState.Press;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        PlayerController.JumpState = JumpState.Release;
    }
}
