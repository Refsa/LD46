using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public struct ScoreInfo
{
    const float FuseLengthWeight = 1.5f;

    public float Score;
    public float ScoreMultiplier;
    public float BurnRate;
    public int PathLength;
    public int ConnectionsMade;
    public int OpenConnections;

    public void CalculateScore()
    {
        Score = ((((float)ConnectionsMade / (float)(OpenConnections)) * (float)PathLength * FuseLengthWeight) * ScoreMultiplier) / BurnRate;
    }
}

public class GameOverUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI scoreInfoText;

    static string initialText = null;

    private void Awake() 
    {
        if (initialText == null)
            initialText = scoreInfoText.text;
    }

    public void Show(ScoreInfo scoreInfo)
    {
        scoreInfoText.text =
            initialText
            .Replace("$Score$", scoreInfo.Score.ToString())
            .Replace("$ScoreMulti$", scoreInfo.ScoreMultiplier.ToString())
            .Replace("$BurnRate$", scoreInfo.BurnRate.ToString())
            .Replace("$FuseLength$", scoreInfo.PathLength.ToString())
            .Replace("$ConnectionsMade$", scoreInfo.ConnectionsMade.ToString())
            .Replace("$OpenConnections$", scoreInfo.OpenConnections.ToString());
    }
}
