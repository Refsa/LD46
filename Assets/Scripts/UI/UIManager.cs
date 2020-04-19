using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] HandUI handUI;
    [SerializeField] ScoreUI scoreUI;
    [SerializeField] GameOverUI gameOverUI;
    [SerializeField] TutorialUI tutorialUI;

    static UIManager instance;

    public static HandUI HandUI;
    public static ScoreUI ScoreUI;
    public static GameOverUI GameOverUI;
    public static TutorialUI TutorialUI;

    private void Awake() 
    {
        if (instance == null) instance = this;
        else Destroy(this);

        HandUI = handUI;
        ScoreUI = scoreUI;
        GameOverUI = gameOverUI;
        TutorialUI = tutorialUI;
    }

    public static void ShowGameUI()
    {
        HandUI.gameObject.SetActive(true);
        ScoreUI.gameObject.SetActive(true);
        GameOverUI.gameObject.SetActive(false);
        TutorialUI.gameObject.SetActive(false);
    }

    public static void ShowGameOverUI(ScoreInfo scoreInfo)
    {
        HandUI.gameObject.SetActive(false);
        ScoreUI.gameObject.SetActive(false);
        GameOverUI.gameObject.SetActive(true);
        TutorialUI.gameObject.SetActive(false);

        GameOverUI.Show(scoreInfo);
    }

    public static void ShowTutorialUI()
    {
        HandUI.gameObject.SetActive(false);
        ScoreUI.gameObject.SetActive(false);
        GameOverUI.gameObject.SetActive(false);
        TutorialUI.gameObject.SetActive(true);
        TutorialUI.Show();
    }
}
