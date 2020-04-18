using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    static GameManager instance;
    static MapGenerator mapGenerator;

    [SerializeField] Camera mainCamera;

    public static Camera MainCamera;

    GridTile selectedGridTile;
    CardUI selectedCard;

    List<GridTile> placedTiles;
    GridTile startTile;

    private void Awake() 
    {
        if (instance == null) instance = this;
        else Destroy(this);

        MainCamera = mainCamera;
        mapGenerator = FindObjectOfType<MapGenerator>();
    }

    private void Start() 
    {
        placedTiles = new List<GridTile>();

        startTile = mapGenerator.PlaceStartTile();
        startTile.NodeBase.StartFuse();
        placedTiles.Add(startTile);

        mapGenerator.Generate();

        for (int i = 0; i < 4; i++)
        {
            GenerateRandomCard();
        }
    }

    private void Update()
    {
        if (placedTiles.Count > 0 && placedTiles.TrueForAll(e => !e.NodeBase.PortsOpen))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    void GenerateRandomCard()
    {
        var randomCard = MapGenerator.Instance.GenerateRandomNode(ConnectorType.Fuse);
        randomCard.Item3.gameObject.SetActive(false);
        UIManager.HandUI.AddCard(randomCard.Item1, randomCard.Item2, randomCard.Item3);
    }

    readonly Vector2Int[] portDirections = 
        new Vector2Int[4]
        {
            new Vector2Int(0, 1),
            new Vector2Int(0, -1),
            new Vector2Int(-1, 0),
            new Vector2Int(1, 0)
        };

    readonly int[] portFlagLookup = new int[4] { 1, -1, 1, -1 };

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
        instance.selectedCard = cardUi;
    }
}
