using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandUI : MonoBehaviour
{
    [SerializeField] GameObject cardUIPrefab;
    [SerializeField] Transform container;

    int cardsInHand = 0;

    List<CardUI> cards;
    RectTransform rectTransform;

    private void Awake() 
    {
        cards = new List<CardUI>();
        rectTransform = GetComponent<RectTransform>();
    }

    public void AddCard(Sprite previewSprite, int rotation, GameObject prefab)
    {
        var newCardUi = GameObject.Instantiate(cardUIPrefab, container).GetComponent<CardUI>();

        newCardUi.SetPreviewImage(previewSprite);
        newCardUi.SetRotation(rotation);
        newCardUi.SetNodeBase(prefab);

        cardsInHand++;
    }

    public void RemoveCard(CardUI card)
    {
        Destroy(card.gameObject);
    }
}
