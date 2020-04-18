using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridTile : MonoBehaviour, IOnClick, IHoverable
{
    [SerializeField] Color defaultColor;
    [SerializeField] Color hoverColor;
    [SerializeField] Color clickColor;

    SpriteRenderer spriteRenderer;

    bool hovered = false;
    bool clicked = false;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void MouseDown()
    {
        spriteRenderer.color = clickColor;
        clicked = true;
    }

    public void MouseUp()
    {
        if (!hovered)
            spriteRenderer.color = defaultColor;
        else
            spriteRenderer.color = hoverColor;
        clicked = false;
    }

    public void HoverEnter()
    {
        spriteRenderer.color = hoverColor;
        hovered = true;
    }

    public void HoverExit()
    {
        spriteRenderer.color = defaultColor;
        hovered = false;
    }
}
