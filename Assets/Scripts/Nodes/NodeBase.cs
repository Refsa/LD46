using System.Linq;
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
    Fuse,
    Start
}

public class NodeBase : MonoBehaviour 
{
    [Header("Config")]
    [SerializeField] ConnectorPorts activeConnectorPorts;
    [SerializeField] int minPortAmount;
    [SerializeField] float portOpenDuration;
    [SerializeField] Color portsOpenColor;
    [SerializeField] Color portsClosedColor;

    [Header("Sprites")]
    [SerializeField] protected Sprite noneSprite;
    [SerializeField] protected Sprite[] directionSprites;

    protected SpriteRenderer spriteRenderer;

    protected ConnectorType connectorType;
    ConnectorPorts openConnectorPorts;

    NodeBase[] connectedNodes;
    public NodeBase[] ConnectedNodes => connectedNodes;

    public ConnectorType ConnectorType => connectorType;
    public ConnectorPorts ConnectorPorts => activeConnectorPorts;
    public Sprite GetSprite => spriteRenderer.sprite;
    public bool PortsOpen => portsOpen;


    static System.Array enumValues;

    float portsOpenTime;
    bool portsOpen = true;
    bool started = false;
    bool isSetup = false;

    bool connectedToRoot = false;
    public bool ConnectedToRoot { get => connectedToRoot; set => connectedToRoot = value; }

    public void Setup()
    {
        if (isSetup) return;

        connectedNodes = new NodeBase[4];
        spriteRenderer = GetComponent<SpriteRenderer>();
        openConnectorPorts = activeConnectorPorts;
        spriteRenderer.color = portsOpenColor;

        if (enumValues == null)
            enumValues = System.Enum.GetValues(typeof(ConnectorPorts));

        isSetup = true;
    }

    protected void Awake() 
    {
        Setup();
    }

    void Update() 
    {
        if (!started) return;

        if (portsOpen)
        {
            portsOpenTime -= Time.deltaTime;
            portsOpen = portsOpenTime >= 0f;

            spriteRenderer.color = Color.Lerp(portsClosedColor, portsOpenColor, portsOpenTime / portOpenDuration);

            if (!portsOpen)
            {
                spriteRenderer.color = portsClosedColor;
                FuseBurned();
            }
        }
    }

    public void StartFuse()
    {
        if (started) return;

        started = true;

        portsOpenTime = portOpenDuration;
        spriteRenderer.color = portsOpenColor;
    }

    public virtual void FuseBurned()
    {
        for (int i = 0; i < 4; i++)
        {
            connectedNodes[i]?.StartFuse();
        }
    }

    protected virtual void SetSprite()
    {
        transform.rotation = Quaternion.Euler(0f, 0f, GetRotation(activeConnectorPorts));

        int spriteIndex = GetSpriteIndex(activeConnectorPorts);
        if (spriteIndex != -1)
            spriteRenderer.sprite = directionSprites[spriteIndex];
        else
            spriteRenderer.sprite = noneSprite;
    }

    public static int GetRotation(ConnectorPorts connectorPorts)
    {
        switch ((int)connectorPorts)
        {
            case (1 << 2):
            case (1 << 1) | (1 << 2):
            case (1 << 0) | (1 << 1) | (1 << 2):
                return 180;
            case (1 << 1):
            case (1 << 1) | (1 << 3):
            case (1 << 1) | (1 << 2) | (1 << 3):
                return -90;
            case (1 << 0):
            case (1 << 0) | (1 << 2):
            case (1 << 0) | (1 << 1):
            case (1 << 0) | (1 << 2) | (1 << 3):
                return 90;
            case (1 << 3):
            case (1 << 2) | (1 << 3):
            case (1 << 0) | (1 << 3):
            case (1 << 0) | (1 << 1) | (1 << 3):
            case (1 << 0) | (1 << 1) | (1 << 2) | (1 << 3):
            case ~0:
                return 0;
        }

        return 0;
    }

    public static int GetSpriteIndex(ConnectorPorts connectorPorts)
    {
        switch ((int)connectorPorts)
        {
            case (1 << 0):
            case (1 << 1):
            case (1 << 2):
            case (1 << 3):
            case (1 << 0) | (1 << 1):
            case (1 << 2) | (1 << 3):
                return 0;
            case (1 << 1) | (1 << 2):
            case (1 << 1) | (1 << 3):
            case (1 << 0) | (1 << 2):
            case (1 << 0) | (1 << 3):
                return 1;
            case (1 << 0) | (1 << 1) | (1 << 2):
            case (1 << 0) | (1 << 2) | (1 << 3):
            case (1 << 1) | (1 << 2) | (1 << 3):
            case (1 << 0) | (1 << 1) | (1 << 3):
                return 2;
            case ~0:
                return 3;
        }

        return -1;
    }

    public void Randomize()
    {
        int openPorts = Random.Range(minPortAmount, 4);

        for (int i = 0; i < openPorts; i++)
        {
            ConnectorPorts newPort = default;
            do
            {
                newPort = (ConnectorPorts) enumValues.GetValue(Random.Range(0, enumValues.Length));
            } while ((newPort & activeConnectorPorts) != 0);

            activeConnectorPorts |= newPort;
        }


        openConnectorPorts = activeConnectorPorts;
        SetSprite();
    }

    public (Sprite, int) GenerateRandomNode()
    {
        int rotation = Random.Range(0, 4) * 90;
        int index = Random.Range(0, 4);
        
        Sprite sprite = noneSprite;
        if (index != -1)
            sprite = directionSprites[index];

        return (sprite, rotation);
    }
}