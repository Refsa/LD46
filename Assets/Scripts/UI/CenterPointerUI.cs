using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CenterPointerUI : MonoBehaviour
{
    [SerializeField] RectTransform marker;

    void Update()
    {
        Vector3 cameraPos = (GameManager.Instance.LastPlacedTilePos - CameraManager.CameraWorldPos());
        if (cameraPos.magnitude > 8f)
        {
            marker.gameObject.SetActive(true);
            Vector3 centerDir = cameraPos.normalized;

            marker.localPosition = centerDir * 350f;
        }
        else
        {
            marker.gameObject.SetActive(false);
        }
    }
}
