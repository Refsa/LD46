using UnityEngine;

public class ObjectNode : NodeBase 
{
    protected new void Awake() 
    {
        base.Awake();

        connectorType = ConnectorType.Object;
    }
}