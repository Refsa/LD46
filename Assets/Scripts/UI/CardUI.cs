using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardUI : MonoBehaviour, IHoverable, IOnClick
{
    [SerializeField] Image previewImage;
    [SerializeField] Image borderImage;

    [SerializeField] Color hoverColor = Color.white;
    [SerializeField] Color selectedColor = Color.gray;

    GameObject nodePrefab;
    public GameObject NodePrefab => nodePrefab;

    bool hovering = false;
    bool selected = false;

    public void SetPreviewImage(Sprite sprite)
    {
        previewImage.sprite = sprite;
    }

    public void SetRotation(int rotation)
    {
        transform.rotation = Quaternion.Euler(0f, 0f, rotation);
    }

    public void SetPosition(Vector3 position)
    {
        transform.localPosition = position;
    }

    public void SetNodeBase(GameObject nodePrefab)
    {
        this.nodePrefab = nodePrefab;
    }

    public void HoverEnter()
    {
        if (selected) return;

        borderImage.color = hoverColor;
        hovering = true;
    }

    public void HoverExit()
    {
        if (selected) return;

        borderImage.color = Color.white;
        hovering = false;
    }

    public void MouseDown()
    {
        borderImage.color = selectedColor;

        GameManager.SetSelectedNodeCard(this);
    }

    public void MouseUp()
    {
        /* if (hovering)
            HoverEnter();
        else
            previewImage.color = Color.white; */
    }

    public void Select(bool status)
    {
        selected = status;

        if (borderImage == null) return;

        if (status)
            borderImage.color = selectedColor;
        else
            borderImage.color = Color.white;
    }
}
