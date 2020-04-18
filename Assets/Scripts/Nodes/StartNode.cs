using UnityEngine;

public class StartNode : NodeBase
{
    protected new void Awake() 
    {
        base.Awake();

        connectorType = ConnectorType.Start;
    }

    protected override void SetSprite()
    {
        spriteRenderer.sprite = noneSprite;
    }
}