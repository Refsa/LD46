using UnityEngine;

public class ScoreMultiplierObjectNode : ObjectNode 
{
    [SerializeField] float scoreMultiplierBonus = 0.25f;

    protected new void Awake()
    {
        base.Awake();

        objectNodeType = ObjectNodeType.ScoreMultiplier;
    }

    public override void Execute()
    {
        base.Execute();
        if (!started) return;

        SoundManager.PlayBuffPickupSound();

        GameManager.AddToScoreMultiplier(scoreMultiplierBonus);
    }
}