using UnityEngine;

public class FuseNode : NodeBase 
{
    protected new void Awake() 
    {
        base.Awake();

        connectorType = ConnectorType.Fuse;
    }
}