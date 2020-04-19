using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class FuseBurnEffectController : MonoBehaviour, IEqualityComparer
{
    [HideInInspector] public GridTile CurrentTile;
    [HideInInspector] public GridTile NextTile;
    [HideInInspector] public GridTile PreviousTile;

    [HideInInspector] public bool PassedCenter;

    [HideInInspector] public bool Mirror;

    public bool Tick()
    {
        if (CurrentTile.NodeBase.PortsOpenNormalizedTime >= 1f) return true;
        else if (CurrentTile.NodeBase.PortsOpenNormalizedTime >= 0.5f) PassedCenter = true;


        if (PassedCenter && PreviousTile != null)
        {
            Vector3 previousPos = CurrentTile.transform.position;
            Vector3 nextPos = (NextTile.transform.position + CurrentTile.transform.position) / 2f;
            Vector3 dir = (nextPos - previousPos).normalized;
            transform.right = dir;

            transform.position = Vector3.Lerp(previousPos, nextPos, math.remap(0.5f, 1f, 0f, 1f, CurrentTile.NodeBase.PortsOpenNormalizedTime));
        }
        else if (PreviousTile != null)
        {
            Vector3 nextPos = CurrentTile.transform.position;
            Vector3 previousPos = (CurrentTile.transform.position + PreviousTile.transform.position) / 2f;
            Vector3 dir = (nextPos - previousPos).normalized;
            transform.right = dir;

            transform.position = Vector3.Lerp(previousPos, nextPos, math.remap(0f, 0.5f, 0f, 1f, CurrentTile.NodeBase.PortsOpenNormalizedTime));
        }
        else
        {
            Vector3 previousPos = CurrentTile.transform.position;
            Vector3 nextPos = (NextTile.transform.position + CurrentTile.transform.position) / 2f;
            Vector3 dir = (nextPos - previousPos).normalized;
            transform.right = dir;

            transform.position = Vector3.Lerp(previousPos, nextPos, CurrentTile.NodeBase.PortsOpenNormalizedTime);
        }

        return false;
    }

    public void SetNextTile(GridTile nextTile)
    {
        PreviousTile = CurrentTile;
        CurrentTile = NextTile;
        NextTile = nextTile;
        PassedCenter = false;
        Mirror = false;
    }

    public new bool Equals(object x, object y)
    {
        var xAs = (FuseBurnEffectController) x;
        var yAs = (FuseBurnEffectController) y;

        return xAs.NextTile == yAs.NextTile && xAs.PreviousTile == yAs.PreviousTile && xAs.CurrentTile == yAs.CurrentTile;
    }

    public int GetHashCode(object obj)
    {
        var objAs = (FuseBurnEffectController) obj;

        return
            objAs.CurrentTile.GetHashCode() + objAs.NextTile.GetHashCode() + objAs.PreviousTile.GetHashCode();
    }
}
