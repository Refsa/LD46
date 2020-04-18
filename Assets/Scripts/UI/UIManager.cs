using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] HandUI handUI;
    [SerializeField] ScoreUI scoreUI;
    [SerializeField] GameOverUI gameOverUI;

    static UIManager instance;

    public static HandUI HandUI;
    public static ScoreUI ScoreUI;
    public static GameOverUI GameOverUI;

    private void Awake() 
    {
        if (instance == null) instance = this;
        else Destroy(this);

        HandUI = handUI;
        ScoreUI = scoreUI;
        GameOverUI = gameOverUI;
    }

    public static void ShowGameUI()
    {
        HandUI.gameObject.SetActive(true);
        ScoreUI.gameObject.SetActive(true);
        GameOverUI.gameObject.SetActive(false);
    }

    public static void ShowGameOverUI(ScoreInfo scoreInfo)
    {
        HandUI.gameObject.SetActive(false);
        ScoreUI.gameObject.SetActive(false);
        GameOverUI.gameObject.SetActive(true);

        GameOverUI.Show(scoreInfo);
    }
}
