using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GridTile : MonoBehaviour, IOnClick, IHoverable
{
    [SerializeField] Color defaultColor;
    [SerializeField] Color hoverColor;
    [SerializeField] Color clickColor;

    SpriteRenderer spriteRenderer;
    public SpriteRenderer SpriteRenderer => spriteRenderer;

    Vector2Int gridPos;

    NodeBase nodeBase;
    public NodeBase NodeBase => nodeBase;

    public Vector2Int GridPos { get => gridPos; set => gridPos = value; }

    bool hovered = false;
    bool clicked = false;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.color = defaultColor;
    }

    public void SetNodePrefab(GameObject nodePrefab)
    {
        nodePrefab.SetActive(true);
        
        this.nodeBase = nodePrefab.GetComponent<NodeBase>();
        nodePrefab.transform.parent = transform;
        nodePrefab.transform.localPosition = Vector3.zero;
        // nodeBase.Start();

        this.spriteRenderer.enabled = false;
    }

    public void MouseDown()
    {
        if (EventSystem.current.IsPointerOverGameObject()) return;

        spriteRenderer.color = clickColor;
        clicked = true;

        GameManager.SetSelectedGridTile(this);
    }

    public void MouseUp()
    {
        if (!hovered)
            spriteRenderer.color = defaultColor;
        else
            spriteRenderer.color = hoverColor;
        clicked = false;

        GameManager.SetSelectedGridTile(null);
    }

    public void HoverEnter()
    {
        if (EventSystem.current.IsPointerOverGameObject()) return;

        spriteRenderer.color = hoverColor;
        hovered = true;
    }

    public void HoverExit()
    {
        spriteRenderer.color = defaultColor;
        hovered = false;
    }
}
