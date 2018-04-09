using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGamePlay : MonoBehaviour
{
    [SerializeField] private Text _currentLife;
    [SerializeField] private Text _currentCoin;
    [SerializeField] private Text _currentTime;
    [SerializeField] private Text _currentScore;

        public void SetLife(string number)
    {
        _currentLife.text = number;
    }

    public void SetTime(string number)
    {
        _currentTime.text = number;
    }

    public void SetScore(string number)
    {
        _currentScore.text = number;
    }

    public void SetCoin(string number)
    {
        _currentCoin.text = number;
    }

    void OnEnable()
    {
        PlayerData._updateLife += SetLife;
        PlayerData._updateCoin += SetCoin;
        PlayerData._updateScore += SetScore;
        PlayerData._updateTime += SetTime;

    }

    void OnDisable()
    {
        PlayerData._updateLife -= SetLife;
        PlayerData._updateCoin -= SetCoin;
        PlayerData._updateScore -= SetScore;
        PlayerData._updateTime -= SetTime;
    }

}
