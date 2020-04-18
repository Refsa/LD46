using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Flags]
public enum ConnectorPorts
{
    Up = 1 << 0,
    Down = 1 << 1,
    Left = 1 << 2,
    Right = 1 << 3
}

public enum ConnectorType
{
    None = 0,
    Object,
    Connector
}

public class Node : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] ConnectorPorts activeConnectorPorts;
    [SerializeField] ConnectorType connectorType;

    [Header("Models")]
    [SerializeField] Sprite noneSprite;
    [SerializeField] Sprite[] objectSprites;
    [SerializeField] Sprite[] connectorSprites;

    ConnectorPorts openConnectorPorts;

    SpriteRenderer spriteRenderer;

    Node[] connectedNodes;

    static Sprite NoneSprite;
    static Sprite[] ConnectorSprites;
    static Sprite[] ObjectSprites;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        openConnectorPorts = activeConnectorPorts;

        NoneSprite = noneSprite;
        ConnectorSprites = connectorSprites;
        ObjectSprites = objectSprites;
    }

    public void SetSprite()
    {
        int rotation = -1;
        switch ((int)activeConnectorPorts)
        {
            case (1 << 2):
            case (1 << 1) | (1 << 2):
            case (1 << 0) | (1 << 1) | (1 << 2):
                rotation = 180;
                break;
            case (1 << 1):
            case (1 << 1) | (1 << 3):
            case (1 << 1) | (1 << 2) | (1 << 3):
                rotation = -90;
                break;
            case (1 << 0):
            case (1 << 0) | (1 << 2):
            case (1 << 0) | (1 << 1):
            case (1 << 0) | (1 << 2) | (1 << 3):
                rotation = 90;
                break;
            case (1 << 3):
            case (1 << 2) | (1 << 3):
            case (1 << 0) | (1 << 3):
            case (1 << 0) | (1 << 1) | (1 << 3):
            case (1 << 0) | (1 << 1) | (1 << 2) | (1 << 3):
            case ~0:
                rotation = 0;
                break;
        }

        int index = -1;
        switch ((int)activeConnectorPorts)
        {
            case (1 << 0):
            case (1 << 1):
            case (1 << 2):
            case (1 << 3):
            case (1 << 0) | (1 << 1):
            case (1 << 2) | (1 << 3):
                index = 0;
                break;
            case (1 << 1) | (1 << 2):
            case (1 << 1) | (1 << 3):
            case (1 << 0) | (1 << 2):
            case (1 << 0) | (1 << 3):
                index = 1;
                break;
            case (1 << 0) | (1 << 1) | (1 << 2):
            case (1 << 0) | (1 << 2) | (1 << 3):
            case (1 << 1) | (1 << 2) | (1 << 3):
            case (1 << 0) | (1 << 1) | (1 << 3):
                index = 2;
                break;
            case ~0:
                index = 3;
                break;
        }

        if (index == -1 || rotation == -1)
        {
            spriteRenderer.sprite = noneSprite;
            return;
        }

        transform.rotation = Quaternion.Euler(0f, 0f, rotation);

        switch (connectorType)
        {
            case ConnectorType.None:
                spriteRenderer.sprite = noneSprite;
                break;
            case ConnectorType.Connector:
                spriteRenderer.sprite = connectorSprites[index];
                break;
            case ConnectorType.Object:
                spriteRenderer.sprite = objectSprites[index];
                break;
        }
    }

    private void OnValidate()
    {
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();

        SetSprite();
    }

    public static (Sprite, int) GenerateRandomNode(ConnectorType connectorType)
    {
        int rotation = Random.Range(0, 4) * 90;
        int index = Random.Range(0, 4);

        Sprite sprite = NoneSprite;

        switch (connectorType)
        {
            case ConnectorType.Connector:
                sprite = ConnectorSprites[index];
                break;
            case ConnectorType.Object:
                sprite = ObjectSprites[index];
                break;
        }

        return (sprite, rotation);
    }    
}
