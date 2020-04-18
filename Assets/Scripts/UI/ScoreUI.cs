using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI text;

    string originalText;

    float scoreMultiplier = 1f;
    float burnRateMultiplier = 1f;

    private void Awake() 
    {
        originalText = text.text;
        UpdateText();
    }

    public void SetScoreMultiplier(float value)
    { 
        scoreMultiplier = value;
        UpdateText();
    }

    public void SetBurnRateMultiplier(float value)
    {
        burnRateMultiplier = value;
        UpdateText();   
    }

    void UpdateText()
    {
        text.text =
            originalText
            .Replace("$SCOREMULTI", scoreMultiplier.ToString("#.##"))
            .Replace("$BURNRATE", burnRateMultiplier.ToString("#.##"));
    }
}
