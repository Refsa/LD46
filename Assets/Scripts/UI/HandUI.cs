using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandUI : MonoBehaviour
{
    [SerializeField] GameObject cardUIPrefab;
    [SerializeField] Transform container;

    int cardsInHand = 0;

    List<CardUI> cards;

    private void Awake() 
    {
        cards = new List<CardUI>();    
    }

    public void AddCard(Sprite previewSprite, int rotation)
    {
        var newCardUi = GameObject.Instantiate(cardUIPrefab, container).GetComponent<CardUI>();

        newCardUi.SetPreviewImage(previewSprite);
        newCardUi.SetRotation(rotation);
        newCardUi.SetPosition(Vector3.right * cardsInHand * 100f);

        cardsInHand++;
    }
}
