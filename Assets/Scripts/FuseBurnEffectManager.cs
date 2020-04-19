using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FuseBurnEffectManager : MonoBehaviour
{
    const int MaxFuseEffectPoolSize = 200;

    static FuseBurnEffectManager instance;

    [Header("Effect")]
    [SerializeField] GameObject fuseEffectPrefab;
    [SerializeField] int fuseObjectPoolSize = 100;
    [SerializeField] Transform fuseObjectPoolContainer;

    [Header("Sound")]
    [SerializeField] float fuseBurnSoundInterval = 0.25f;

    List<FuseBurnEffectController> activeFuses;
    Queue<GameObject> fuseObjectPool;

    float lastFuseBurnSoundTime;
    float currentFuseBurnSoundInterval;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(this);

        activeFuses = new List<FuseBurnEffectController>();
        fuseObjectPool = new Queue<GameObject>();

        ExpandFuseObjectPool();

        currentFuseBurnSoundInterval = fuseBurnSoundInterval;
    }

    void Update()
    {
        List<FuseBurnEffectController> toRelease = new List<FuseBurnEffectController>();
        List<FuseBurnEffectController> toAdd = new List<FuseBurnEffectController>();

        foreach(var fuse in activeFuses)
        {
            if (fuse.Tick())
            {
                if (NextTileHasMatchingConnections(fuse.CurrentTile, fuse.NextTile))
                {
                    var nextTiles = FindOpenConnections(fuse.CurrentTile, fuse.NextTile, fuse.NextTile.GridPos);

                    if (nextTiles.Count == 0) 
                    {
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

        if (activeFuses.Count > 0 && Time.time - lastFuseBurnSoundTime > currentFuseBurnSoundInterval)
        {
            SoundManager.PlayFuseBrunBeep();
            lastFuseBurnSoundTime = Time.time;
        }

        toAdd.ForEach(e => AddActiveFuse(e));

        toRelease.ForEach(e => {
            ReleaseFuseObject(e.gameObject);
            activeFuses.Remove(e);
        });
    }

    bool NextTileHasMatchingConnections(GridTile current, GridTile next)
    {
        if (next.NodeBase == null || !next.NodeBase.PortsOpen) return false;

        Vector2Int nextTileDir = next.GridPos - current.GridPos;
        
        if (nextTileDir.x > 0)
        {
            ConnectorPorts nextFlag = ConnectorPorts.Left;

            if (next.NodeBase.ConnectorPorts.HasFlag(nextFlag))
            {
                return true;
            }
        }
        else if (nextTileDir.x < 0)
        {
            ConnectorPorts nextFlag = ConnectorPorts.Right;

            if (next.NodeBase.ConnectorPorts.HasFlag(nextFlag))
            {
                return true;
            }
        }
        else if (nextTileDir.y > 0)
        {
            ConnectorPorts nextFlag = ConnectorPorts.Down;

            if (next.NodeBase.ConnectorPorts.HasFlag(nextFlag))
            {
                return true;
            }
        }
        else if (nextTileDir.y < 0)
        {
            ConnectorPorts nextFlag = ConnectorPorts.Up;

            if (next.NodeBase.ConnectorPorts.HasFlag(nextFlag))
            {
                return true;
            }
        }

        return false;
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
                {
                    if (otherNode.NodeBase != null && otherNode.NodeBase.PortsOpenNormalizedTime > 0.9f) continue;

                    nextTiles.Add(otherNode);
                }
            }
        }
        return nextTiles;
    }

    bool ExpandFuseObjectPool()
    {
        if (activeFuses.Count + fuseObjectPool.Count > MaxFuseEffectPoolSize) return false;

        for (int i = 0; i < fuseObjectPoolSize; i++)
        {
            var newFuseObject = GameObject.Instantiate(fuseEffectPrefab, fuseObjectPoolContainer);
            newFuseObject.SetActive(false);
            fuseObjectPool.Enqueue(newFuseObject);
        }

        return true;
    }

    public void AddActiveFuse(FuseBurnEffectController fuseObject)
    {
        // if (activeFuses.Find(e => e.NextTile == fuseObject.NextTile) == null)
            // activeFuses.Add(fuseObject);
        // else
            // ReleaseFuseObject(fuseObject.gameObject);

        if (activeFuses.Find(e => e.NextTile == fuseObject.NextTile) != null)
            fuseObject.Mirror = true;

        activeFuses.Add(fuseObject);
    }

    public GameObject GetFuseObject()
    {
        if (fuseObjectPool.Count > 0)
        {
            return fuseObjectPool.Dequeue();
        }
        
        if (!ExpandFuseObjectPool())
        {
            ReleaseFuseObject(activeFuses[0].gameObject);
            activeFuses.RemoveAt(0);
        }
        return GetFuseObject();
    }

    public void ReleaseFuseObject(GameObject fuseObject)
    {
        fuseObject.SetActive(false);
        fuseObjectPool.Enqueue(fuseObject);
    }
}
