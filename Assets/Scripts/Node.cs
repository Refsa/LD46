using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        var spriteData = GetSprite(connectorType, activeConnectorPorts);

        transform.rotation = Quaternion.Euler(0f, 0f, spriteData.Item2);
        spriteRenderer.sprite = spriteData.Item1;
    }

    private void OnValidate()
    {
        Awake();

        SetSprite();
    }

    public static (Sprite, int) GetSprite(ConnectorType connectorType, ConnectorPorts connectorPorts)
    {
        int rotation = -1;
        switch ((int)connectorPorts)
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
        switch ((int)connectorPorts)
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

        Sprite sprite = NoneSprite;

        switch (connectorType)
        {
            case ConnectorType.None:
                sprite = NoneSprite;
                break;
            case ConnectorType.Fuse:
                sprite = ConnectorSprites[index];
                break;
            case ConnectorType.Object:
                sprite = ObjectSprites[index];
                break;
        }

        return (sprite, rotation);
    }

    public static (Sprite, int) GenerateRandomNode(ConnectorType connectorType)
    {
        int rotation = Random.Range(0, 4) * 90;
        int index = Random.Range(0, 4);

        Sprite sprite = NoneSprite;

        switch (connectorType)
        {
            case ConnectorType.Fuse:
                sprite = ConnectorSprites[index];
                break;
            case ConnectorType.Object:
                sprite = ObjectSprites[index];
                break;
        }

        return (sprite, rotation);
    }    
}
