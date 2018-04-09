using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITest : MonoBehaviour
{
    public InputField TxtLevel;
    public InputField TxtCoin;

    public void ButtonLoadLevelClick()
    {
        if (TxtLevel.text != string.Empty)
            MapManager.Instance.MapIndex = int.Parse(TxtLevel.text);

        GameController.Instance.StopGame();
        GameController.Instance.StartGame();
    }

    public void ButtonCloseLevel()
    {
        GameController.Instance.StopGame();
    }

    public void AddCoin()
    {

    }
}
