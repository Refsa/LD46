using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FuseBurnEffectManager : MonoBehaviour
{
    static FuseBurnEffectManager instance;

    [SerializeField] GameObject fuseEffectPrefab;
    [SerializeField] int fuseObjectPoolSize = 100;

    List<FuseBurnEffectController> activeFuses;
    Queue<GameObject> fuseObjectPool;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(this);

        activeFuses = new List<FuseBurnEffectController>();
        fuseObjectPool = new Queue<GameObject>();

        ExpandFuseObjectPool();
    }

    void Update()
    {
        List<FuseBurnEffectController> toRelease = new List<FuseBurnEffectController>();
        List<FuseBurnEffectController> toAdd = new List<FuseBurnEffectController>();

        foreach(var fuse in activeFuses)
        {
            if (fuse.Tick())
            {
                if (fuse.NextTile.NodeBase != null)
                {
                    var nextTiles = FindOpenConnections(fuse.CurrentTile, fuse.NextTile, fuse.NextTile.GridPos);

                    if (nextTiles.Count == 0) 
                    {
                        UnityEngine.Debug.Log($"Release Fuse");
                        toRelease.Add(fuse);
                    }
                    else
                    {
                        if (nextTiles.Count > 1)
                        {
                            for (int i = 1; i < nextTiles.Count; i++)
                            {
                                var fuseObject = GetFuseObject().GetComponent<FuseBurnEffectController>();
                                fuseObject.gameObject.SetActive(true);

                                fuseObject.CurrentTile = fuse.CurrentTile;
                                fuseObject.NextTile = fuse.NextTile;
                                fuseObject.SetNextTile(nextTiles[i]);

                                toAdd.Add(fuseObject);
                            }
                        }
                        fuse.SetNextTile(nextTiles[0]);
                    }
                }
                else
                {
                    toRelease.Add(fuse);
                }
            }
        }

        toRelease.ForEach(e =>{
            ReleaseFuseObject(e.gameObject);
            activeFuses.Remove(e);
        });

        toAdd.ForEach(e => AddActiveFuse(e));
    }

    List<GridTile> FindOpenConnections(GridTile current, GridTile next, Vector2Int _pos)
    {
        List<GridTile> nextTiles = new List<GridTile>();
        for (int i = 0; i < 4; i++)
        {
            ConnectorPorts flag = (ConnectorPorts)(1 << i);

            if (next.NodeBase.ConnectorPorts.HasFlag(flag))
            {
                Vector2Int pos = _pos + GameManager.Instance.portDirections[i];
                var otherNode = MapGenerator.Instance.Grid.GetTile(pos.x, pos.y);
                // ConnectorPorts otherFlag = (ConnectorPorts)(1 << i + GameManager.Instance.portFlagLookup[i]);

                if (otherNode != current && otherNode != next)
                    nextTiles.Add(otherNode);
            }
        }
        return nextTiles;
    }

    void ExpandFuseObjectPool()
    {
        for (int i = 0; i < fuseObjectPoolSize; i++)
        {
            var newFuseObject = GameObject.Instantiate(fuseEffectPrefab, Vector3.zero, Quaternion.identity);
            newFuseObject.SetActive(false);
            fuseObjectPool.Enqueue(newFuseObject);
        }
    }

    public void AddActiveFuse(FuseBurnEffectController fuseObject)
    {
        activeFuses.Add(fuseObject);
    }

    public GameObject GetFuseObject()
    {
        if (fuseObjectPool.Count > 0)
        {
            return fuseObjectPool.Dequeue();
        }

        ExpandFuseObjectPool();
        return GetFuseObject();
    }

    public void ReleaseFuseObject(GameObject fuseObject)
    {
        fuseObject.SetActive(false);
        fuseObjectPool.Enqueue(fuseObject);
    }
}
