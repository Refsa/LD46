using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardUI : MonoBehaviour, IHoverable, IOnClick
{
    [SerializeField] Image previewImage;

    bool hovering = false;

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
        transform.localPosition += position;
    }

    public void HoverEnter()
    {
        previewImage.color = new Color(0.5f, 1f, 0.9f, 1f);
        hovering = true;
    }

    public void HoverExit()
    {
        previewImage.color = Color.white;
        hovering = false;
    }

    public void MouseDown()
    {
        previewImage.color = new Color(1f, 0.5f, 0.9f, 1f);
    }

    public void MouseUp()
    {
        if (hovering)
            HoverEnter();
        else
            previewImage.color = Color.white;
    }
}
