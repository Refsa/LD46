using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;
using Unity.Mathematics;

public enum GameState
{
    Tutorial,
    StartGame,
    Game,
    GameOver
}

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

    static bool newlyOpenedGame = true;
    public static bool NewlyOpenedGame => newlyOpenedGame;

    GameState currentGameState;
    public GameState CurrentGameState => currentGameState;
    bool gameStarted = false;
    public bool GameStarted => gameStarted;

    GridTile selectedGridTile;
    CardUI selectedCard;

    List<GridTile> placedTiles;
    GridTile startTile;

    float currentScoreMultiplier = 1f;
    float fuseBurnRateMultiplier = 1f;

    public float FuseBurnRateMultiplier => fuseBurnRateMultiplier;

    float burnRateBleepInterval = 1f;
    float currentBurnRateBleepInterval;
    float lastBurnRateBleepTime;
    float fuseLengthMultiplier = 1f;

    System.DateTime gameTimer;
    public System.DateTime GameTimer => gameTimer;
    float lastFuseBurnRateBump;

    public Vector3 LastPlacedTilePos;

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
        if (newlyOpenedGame)
        {
            UIManager.ShowTutorialUI();
            currentGameState = GameState.Tutorial;
            newlyOpenedGame = false;
            return;
        }

        StartNewGame();
    }

    private void Update()
    {
        if (currentGameState == GameState.Tutorial)
        {
            if (Input.GetKeyDown(KeyCode.S))
            {
                if (!gameStarted)
                    SetGameState(GameState.StartGame);
                else
                    SetGameState(GameState.Game);
            }
        }
        if (currentGameState != GameState.Game) return;
        if (Input.GetKeyDown(KeyCode.T))
        {
            SetGameState(GameState.Tutorial);
        }

        if(placedTiles.Count == 1)
        {
            lastFuseBurnRateBump = Time.time;
            return;
        }

        gameTimer = gameTimer.AddSeconds(Time.deltaTime);
        if (Time.time - lastFuseBurnRateBump > 10f)
        {
            AddToFuseBurnTimeMultiplier(0.3f);
            lastFuseBurnRateBump = Time.time;
        }

        placedTiles.ForEach(e => e.NodeBase.Tick());

        if (placedTiles.Count > 0 && placedTiles.TrueForAll(e => !e.NodeBase.PortsOpen))
        {
            SetGameState(GameState.GameOver);
            return;
        }
        else
        {
            if (Time.time - lastBurnRateBleepTime > currentBurnRateBleepInterval / fuseLengthMultiplier)
            {
                SoundManager.PlayBurnRateBeep();
                lastBurnRateBleepTime = Time.time;

                float startRampUpTime = 32f; // Where the ramp up starts
                float expFactor = 3f; // How much it ramps up
                float fuseTimeLeft = placedTiles.Sum(e => e.NodeBase.PortsOpenTime);
                float fuseTimeNorm = (1f - Mathf.Clamp01(fuseTimeLeft / startRampUpTime));
                
                fuseLengthMultiplier = 1f + math.exp(fuseTimeNorm * expFactor);
            }
        }
    }

    public void SetGameState(GameState gameState)
    {
        currentGameState = gameState;

        switch(gameState)
        {
            case GameState.Tutorial:
                UIManager.ShowTutorialUI();
                break;
            case GameState.StartGame:
                StartNewGame();
                break;
            case GameState.Game:
                UIManager.ShowGameUI();
                break;
            case GameState.GameOver:
                GameOverState();
                break;
        }
    }

    public void StartNewGame()
    {
        if (gameStarted) 
        {
            SetGameState(GameState.Game);
            return;
        }

        gameTimer = new System.DateTime(0);

        placedTiles = new List<GridTile>();

        startTile = mapGenerator.PlaceStartTile();
        startTile.NodeBase.Execute();
        placedTiles.Add(startTile);

        LastPlacedTilePos = startTile.transform.position;

        mapGenerator.Generate();

        for (int i = 0; i < 5; i++)
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

        SetGameState(GameState.Game);
        gameStarted = true;
    }

    void GameOverState()
    {
        SoundManager.PlayGameOverSound();

        ScoreInfo scoreInfo = new ScoreInfo();
        scoreInfo.Score = 0f;
        scoreInfo.ScoreMultiplier = currentScoreMultiplier;
        scoreInfo.BurnRate = fuseBurnRateMultiplier;
        scoreInfo.PathLength = placedTiles.Count;

        scoreInfo.OpenConnections = math.max(1, placedTiles.Sum(e => e.NodeBase.CountOpenPorts()));
        scoreInfo.ConnectionsMade = (int) math.ceil(placedTiles.Sum(e => e.NodeBase.CountUsedPorts()) / 2f);

        scoreInfo.CalculateScore();

        UIManager.ShowGameOverUI(scoreInfo);

        currentGameState = GameState.GameOver;
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

                    PropagateObjectNodeRootConnection(otherNode.NodeBase, otherNode.GridPos);
                }
            }
        }
    }

    void GenerateRandomCard()
    {
        var randomCard = 
            MapGenerator.Instance.GenerateRandomNode(ConnectorType.Fuse, 
                UIManager.HandUI.Cards.Select(e => e.NodePrefab.GetComponent<NodeBase>().ConnectorPorts).ToArray());

        randomCard.Item3.gameObject.SetActive(false);
        UIManager.HandUI.AddCard(randomCard.Item1, randomCard.Item2, randomCard.Item3);
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

            instance.LastPlacedTilePos = gridTile.transform.position;
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

        instance.currentBurnRateBleepInterval = instance.burnRateBleepInterval / (instance.fuseBurnRateMultiplier * 0.25f);

        instance.placedTiles.Where(e => e.NodeBase.PortsOpen).All(e => {e.NodeBase.BurnRateMultiplier = instance.fuseBurnRateMultiplier; return false;});

        UIManager.ScoreUI.SetBurnRateMultiplier(instance.fuseBurnRateMultiplier);

        // UnityEngine.Debug.Log($"Burn Rate Changed {instance.fuseBurnRateMultiplier}");
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
