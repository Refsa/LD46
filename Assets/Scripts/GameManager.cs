using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;
using Unity.Mathematics;

public class GameManager : MonoBehaviour
{
    public readonly Vector2Int[] portDirections = 
        new Vector2Int[4]
        {
            new Vector2Int(0, 1),
            new Vector2Int(0, -1),
            new Vector2Int(-1, 0),
            new Vector2Int(1, 0)
        };

    public readonly int[] portFlagLookup = new int[4] { 1, -1, 1, -1 };

    static GameManager instance;
    public static GameManager Instance => instance;

    static MapGenerator mapGenerator;
    static FuseBurnEffectManager fuseBurnEffectManager;

    [SerializeField] Camera mainCamera;
    public static Camera MainCamera;

    GridTile selectedGridTile;
    CardUI selectedCard;

    List<GridTile> placedTiles;
    GridTile startTile;

    bool gameOver = false;

    float currentScoreMultiplier = 1f;
    float fuseBurnRateMultiplier = 1f;
    float score = 0f;

    float burnRateBleepInterval = 1f;
    float currentBurnRateBleepInterval;
    float lastBurnRateBleepTime;
    float fuseLengthMultiplier = 1f;

    private void Awake() 
    {
        if (instance == null) instance = this;
        else Destroy(this);

        MainCamera = mainCamera;
        mapGenerator = FindObjectOfType<MapGenerator>();
        fuseBurnEffectManager = FindObjectOfType<FuseBurnEffectManager>();

        currentBurnRateBleepInterval = burnRateBleepInterval * fuseBurnRateMultiplier;
    }

    private void Start() 
    {
        gameOver = false;
        placedTiles = new List<GridTile>();

        startTile = mapGenerator.PlaceStartTile();
        startTile.NodeBase.Execute();
        placedTiles.Add(startTile);

        mapGenerator.Generate();

        for (int i = 0; i < 4; i++)
        {
            GenerateRandomCard();
        }

        UIManager.ShowGameUI();

        // Spawn Start Tile Fuses
        for (int i = 0; i < 4; i++)
        {
            var fuseObject = fuseBurnEffectManager.GetFuseObject().GetComponent<FuseBurnEffectController>();
            fuseObject.gameObject.SetActive(true);

            fuseObject.CurrentTile = startTile;

            Vector2Int nextTilePos = startTile.GridPos + portDirections[i];
            fuseObject.NextTile = MapGenerator.Instance.Grid.GetTile(nextTilePos.x, nextTilePos.y);

            fuseBurnEffectManager.AddActiveFuse(fuseObject);
        }
    }

    private void Update()
    {
        if (gameOver) return;

        if (placedTiles.Count > 0 && placedTiles.TrueForAll(e => !e.NodeBase.PortsOpen))
        {
            SoundManager.PlayGameOverSound();

            ScoreInfo scoreInfo = new ScoreInfo();
            scoreInfo.Score = 0f;
            scoreInfo.ScoreMultiplier = currentScoreMultiplier;
            scoreInfo.BurnRate = fuseBurnRateMultiplier;
            scoreInfo.PathLength = placedTiles.Count;
            scoreInfo.OpenConnections = placedTiles.Sum(e => e.NodeBase.UsedPorts);
            scoreInfo.ConnectionsMade = placedTiles.Sum(e => e.NodeBase.OpenPortsLeft);

            scoreInfo.CalculateScore();

            UIManager.ShowGameOverUI(scoreInfo);

            gameOver = true;
            return;
        }
        else
        {
            if (Time.time - lastBurnRateBleepTime > currentBurnRateBleepInterval / fuseLengthMultiplier)
            {
                SoundManager.PlayBurnRateBeep();
                lastBurnRateBleepTime = Time.time;

                float startRampUpTime = 8f; // Where the ramp up starts
                float expFactor = 3f; // How much it ramps up
                float fuseTimeLeft = placedTiles.Sum(e => e.NodeBase.PortsOpenTime);
                float fuseTimeNorm = (1f - Mathf.Clamp01(fuseTimeLeft / startRampUpTime));
                fuseLengthMultiplier = 1f + math.exp(fuseTimeNorm * expFactor);
            }
        }
    }

    void GenerateRandomCard()
    {
        var randomCard = MapGenerator.Instance.GenerateRandomNode(ConnectorType.Fuse);
        randomCard.Item3.gameObject.SetActive(false);
        UIManager.HandUI.AddCard(randomCard.Item1, randomCard.Item2, randomCard.Item3);
    }

    bool CheckForRootNodeConnections(NodeBase nodeA, Vector2Int _pos)
    {
        bool connected = false;

        for (int i = 0; i < 4; i++)
        {
            ConnectorPorts flag = (ConnectorPorts)(1 << i);

            if (nodeA.ConnectorPorts.HasFlag(flag))
            {
                Vector2Int pos = _pos + portDirections[i];
                var otherNode = MapGenerator.Instance.Grid.GetTile(pos.x, pos.y);
                ConnectorPorts otherFlag = (ConnectorPorts)(1 << i + portFlagLookup[i]);

                if (otherNode.NodeBase != null &&
                    otherNode.NodeBase.PortsOpen &&
                    otherNode.NodeBase.ConnectedToRoot &&
                    otherNode.NodeBase.ConnectorPorts.HasFlag(otherFlag))
                {
                    nodeA.ConnectedNodes[i] = otherNode.NodeBase;
                    otherNode.NodeBase.ConnectedNodes[i + portFlagLookup[i]] = nodeA;
                    connected = true;
                }
            }
        }

        nodeA.ConnectedToRoot = connected;

        return connected;
    }

    void PropagateObjectNodeRootConnection(NodeBase nodeA, Vector2Int _pos)
    {
        for (int i = 0; i < 4; i++)
        {
            ConnectorPorts flag = (ConnectorPorts)(1 << i);

            if (nodeA.ConnectorPorts.HasFlag(flag))
            {
                Vector2Int pos = _pos + portDirections[i];
                var otherNode = MapGenerator.Instance.Grid.GetTile(pos.x, pos.y);
                ConnectorPorts otherFlag = (ConnectorPorts)(1 << i + portFlagLookup[i]);

                if (otherNode.NodeBase != null &&
                    otherNode.NodeBase.PortsOpen &&
                    !otherNode.NodeBase.ConnectedToRoot &&
                    otherNode.NodeBase.ConnectorPorts.HasFlag(otherFlag))
                {
                    nodeA.ConnectedNodes[i] = otherNode.NodeBase;
                    otherNode.NodeBase.ConnectedNodes[i + portFlagLookup[i]] = nodeA;
                    
                    otherNode.NodeBase.ConnectedToRoot = true;
                    placedTiles.Add(otherNode);
                }
            }
        }
    }

    public static void SetSelectedGridTile(GridTile gridTile)
    {
        instance.selectedGridTile = gridTile;

        if (instance.selectedCard != null && gridTile != null && gridTile.NodeBase == null &&
            instance.CheckForRootNodeConnections(instance.selectedCard.NodePrefab.GetComponent<NodeBase>(), gridTile.GridPos))
        {
            instance.PropagateObjectNodeRootConnection(instance.selectedCard.NodePrefab.GetComponent<NodeBase>(), gridTile.GridPos);

            gridTile.SetNodePrefab(instance.selectedCard.NodePrefab);

            instance.placedTiles.Add(gridTile);

            UIManager.HandUI.RemoveCard(instance.selectedCard);
            instance.GenerateRandomCard();
        }
    }

    public static void SetSelectedNodeCard(CardUI cardUi)
    {
        instance.selectedCard?.Select(false);

        instance.selectedCard = cardUi;
        instance.selectedCard.Select(true);
    }

    public static void AddToScoreMultiplier(float value)
    {
        instance.currentScoreMultiplier += value;

        UIManager.ScoreUI.SetScoreMultiplier(instance.currentScoreMultiplier);

        // UnityEngine.Debug.Log($"Score Multi Changed {instance.currentScoreMultiplier}");
    }

    public static void AddToFuseBurnTimeMultiplier(float value)
    {
        instance.fuseBurnRateMultiplier += value;

        instance.currentBurnRateBleepInterval = instance.burnRateBleepInterval / instance.fuseBurnRateMultiplier;

        instance.placedTiles.Where(e => e.NodeBase.PortsOpen).All(e => {e.NodeBase.BurnRateMultiplier = instance.fuseBurnRateMultiplier; return false;});

        UIManager.ScoreUI.SetBurnRateMultiplier(instance.currentScoreMultiplier);

        // UnityEngine.Debug.Log($"Burn Rate Changed {instance.fuseBurnRateMultiplier}");
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
