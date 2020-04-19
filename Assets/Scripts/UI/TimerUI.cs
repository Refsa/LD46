using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class TimerUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI timerText;

    void Update()
    {
        if (GameManager.Instance.CurrentGameState == GameState.Game)
        {
            timerText.text = GameManager.Instance.GameTimer.ToString("mm:ss.ff");
        }        
    }
}
