using UnityEngine;

public class ConnectorObjectNode : ObjectNode 
{
    protected new void Awake()
    {
        base.Awake();

        objectNodeType = ObjectNodeType.Connector;
    }
}