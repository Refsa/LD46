using UnityEngine;

public class BurnerObjectNode : ObjectNode 
{
    [SerializeField] float fuseBurnMultiplier = 0.25f;

    protected new void Awake()
    {
        base.Awake();

        objectNodeType = ObjectNodeType.Burner;
    }

    public override void Execute()
    {
        base.Execute();
        if (!started) return;

        GameManager.AddToFuseBurnTimeMultiplier(fuseBurnMultiplier);
    }
}