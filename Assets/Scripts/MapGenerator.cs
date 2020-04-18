using System.Collections;
using System.Collections.Generic;
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
        }

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

    public (Sprite, int, GameObject) GenerateRandomNode(ConnectorType connectorType)
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

        return (
            nodeBase.GetSprite,
            NodeBase.GetRotation(nodeBase.ConnectorPorts),
            instance
        );
    }
}
