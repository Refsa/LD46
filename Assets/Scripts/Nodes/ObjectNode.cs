using UnityEngine;

public enum ObjectNodeType
{
    Connector, // Connector with more fuse time
    ScoreMultiplier, // Increases score multiplier
    Burner // Makes fuses burn faster
}

public class ObjectNode : NodeBase
{
    protected ObjectNodeType objectNodeType;

    protected new void Awake() 
    {
        base.Awake();

        connectorType = ConnectorType.Object;
    }

    public static ObjectNodeType GetRandomObjectNodeType()
    {
        var objectNodeTypeValues = System.Enum.GetValues(typeof(ObjectNodeType));

        return (ObjectNodeType) objectNodeTypeValues.GetValue(Random.Range(0, objectNodeTypeValues.Length));
    }
}