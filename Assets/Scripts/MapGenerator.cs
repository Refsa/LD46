using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    static MapGenerator instance;
    public static MapGenerator Instance => instance;

    [Header("Config")]
    [SerializeField] int objectsToSpawn = 1000;
    [SerializeField] float centerObjectSpawnSafezone = 5;
    [SerializeField] int distanceBetweenObjects = 2;

    [Header("Node Prefabs")]
    [SerializeField] GameObject startNodePrefab;
    [SerializeField] GameObject fuseNodePrefab;
    [SerializeField] GameObject objectNodePrefab;

    [Header("Object Node Prefabs")]
    [SerializeField] GameObject scoreMultiplierObjectNodePrefab;
    [SerializeField] GameObject connectorObjectNodePrefab;
    [SerializeField] GameObject burnerObjectNodePrefab;

    [Header("Components")]
    [SerializeField] Grid grid;
    
    public Grid Grid => grid;

    public Vector3 CenterPos;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(this);    
    }

    private void Start()
    {
        
    }

    public void Generate()
    {
        var sw = new System.Diagnostics.Stopwatch();
        sw.Restart();

        int halfGridSize = grid.GridSize / 2;
        Vector2Int gridCenter = new Vector2Int(halfGridSize, halfGridSize);

        List<GridTile> spawnedTiles = new List<GridTile>();

        for (int i = 0; i < objectsToSpawn; i++)
        {
            Vector2Int pos = new Vector2Int(Random.Range(0, grid.GridSize), Random.Range(0, grid.GridSize));

            while (grid.GetTile(pos.x, pos.y).NodeBase != null || 
                    Vector2Int.Distance(pos, gridCenter) < centerObjectSpawnSafezone ||
                    ObjectInDistance(pos, distanceBetweenObjects))
            {
                pos = new Vector2Int(Random.Range(0, grid.GridSize), Random.Range(0, grid.GridSize));
            }

            var tile = grid.GetTile(pos.x, pos.y);

            ObjectNodeType objectNodeType = ObjectNode.GetRandomObjectNodeType();
            GameObject nodePrefab = null;

            switch (objectNodeType)
            {
                case ObjectNodeType.Burner:
                    nodePrefab = GameObject.Instantiate(burnerObjectNodePrefab);
                    break;
                case ObjectNodeType.Connector:
                    nodePrefab = GameObject.Instantiate(connectorObjectNodePrefab);
                    break;
                case ObjectNodeType.ScoreMultiplier:
                    nodePrefab = GameObject.Instantiate(scoreMultiplierObjectNodePrefab);
                    break;
            }

            nodePrefab.GetComponent<NodeBase>().Randomize();

            tile.SetNodePrefab(nodePrefab);

            spawnedTiles.Add(tile);
        }

        spawnedTiles.ForEach(
            e => {
                for (int i = 0; i < 4; i++)
                {
                    ConnectorPorts flag = (ConnectorPorts)(1 << i);

                    if (e.NodeBase.ConnectorPorts.HasFlag(flag))
                    {
                        Vector2Int pos = e.GridPos + GameManager.Instance.portDirections[i];
                        var otherNode = MapGenerator.Instance.Grid.GetTile(pos.x, pos.y);
                        if(otherNode == null) continue;

                        ConnectorPorts otherFlag = (ConnectorPorts)(1 << i + GameManager.Instance.portFlagLookup[i]);

                        if (otherNode.NodeBase != null &&
                            otherNode.NodeBase.PortsOpen &&
                            otherNode.NodeBase.ConnectedToRoot &&
                            otherNode.NodeBase.ConnectorPorts.HasFlag(otherFlag))
                        {
                            e.NodeBase.ConnectedNodes[i] = otherNode.NodeBase;
                            otherNode.NodeBase.ConnectedNodes[i + GameManager.Instance.portFlagLookup[i]] = e.NodeBase;
                        }
                    }
                }
            }
        );

        sw.Stop();
        UnityEngine.Debug.Log($"Map Gen. Time: {sw.ElapsedMilliseconds} ms");
    }

    public bool ObjectInDistance(Vector2Int center, int distance)
    {
        int halfDist = distance / 2;
        for (int i = -halfDist; i <= halfDist; i++)
        {
            for (int j = -halfDist; j <= halfDist; j++)
            {
                Vector2Int pos = center + new Vector2Int(i, j);

                if (grid.GetTile(pos.x, pos.y)?.NodeBase != null) 
                    return true;
            }
        }

        return false;
    }

    public GridTile PlaceStartTile()
    {
        int mapCenter = grid.GridSize / 2;
        
        GridTile startTile = grid.GetTile(mapCenter, mapCenter);
        startTile.SetNodePrefab(GameObject.Instantiate(startNodePrefab));
        startTile.NodeBase.ConnectedToRoot = true;

        return startTile;
    }

    public (Sprite, int, GameObject) GenerateRandomNode(ConnectorType connectorType, params ConnectorPorts[] avoidConfigurations)
    {
        GameObject instance = null;

        switch (connectorType)
        {
            case ConnectorType.Fuse:
                instance = GameObject.Instantiate(fuseNodePrefab);
                break;
            case ConnectorType.Object:
                instance = GameObject.Instantiate(objectNodePrefab);
                break;
        }

        if (instance == null)
            return (null, -1, null);

        var nodeBase = instance.GetComponent<NodeBase>();
        nodeBase.Setup();
        nodeBase.Randomize();

        if (avoidConfigurations.Length > 0)
        {
            float startTime = Time.time;
            int currentPortCount = avoidConfigurations.Sum(e => CountPorts(e));

            // while (avoidConfigurations.Contains(nodeBase.ConnectorPorts))
            while (avoidConfigurations.Count(e => e == nodeBase.ConnectorPorts) > 2 && currentPortCount + CountPorts(nodeBase.ConnectorPorts) > 13)
            {
                nodeBase.Randomize();
                if (Time.time - startTime > 1f)
                {
                    UnityEngine.Debug.Log($"Card Gen Timeout");
                    break;
                }
            }
        }

        return (
            nodeBase.GetSprite,
            NodeBase.GetRotation(nodeBase.ConnectorPorts),
            instance
        );
    }

    int CountPorts(ConnectorPorts connectorPorts)
    {
        int count = 0;
        for (int i = 0; i < 4; i++)
        {
            ConnectorPorts flag = (ConnectorPorts)(1 << i);

            if (connectorPorts.HasFlag(flag))
            {
                count++;
            }
        }
        return count;
    }
}
