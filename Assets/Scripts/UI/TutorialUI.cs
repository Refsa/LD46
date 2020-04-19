using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TutorialUI : MonoBehaviour
{
    [SerializeField] RectTransform[] steps;
    [SerializeField] TextMeshProUGUI startGameButtonText;
    [SerializeField] Button nextButton;
    [SerializeField] Button prevButton;

    bool started = false;
    int currentStep = 0;

    public void Show()
    {
        if (GameManager.NewlyOpenedGame)
        {
            startGameButtonText.transform.parent.gameObject.SetActive(false);
        }

        if (GameManager.Instance.GameStarted)
        {
            startGameButtonText.text = "Continue";
        }
        else
        {
            startGameButtonText.text = "Start Game";
        }
    }

    public void NextState()
    {
        prevButton.gameObject.SetActive(true);

        steps[currentStep].gameObject.SetActive(false);
        currentStep++;

        if (currentStep >= steps.Length)
        {
            currentStep = steps.Length - 1;
        }
        else if (currentStep == steps.Length - 1)
        {
            nextButton.gameObject.SetActive(false);

            startGameButtonText.transform.parent.gameObject.SetActive(true);
        }

        steps[currentStep].gameObject.SetActive(true);
    }

    public void PreviousState()
    {
        nextButton.gameObject.SetActive(true);

        steps[currentStep].gameObject.SetActive(false);
        currentStep--;

        if (currentStep < 0) 
        {
            currentStep = 0;
        }
        else if (currentStep == 0)
        {
            prevButton.gameObject.SetActive(false);
        }

        steps[currentStep].gameObject.SetActive(true);
    }
}
