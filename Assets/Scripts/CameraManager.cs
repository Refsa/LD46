using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    static CameraManager instance;

    Vector3 previousMousePos = Vector3.zero;

    Camera mainCamera;

    private void Awake() 
    {
        if (instance == null) instance = this;
        else Destroy(this);

        mainCamera = GetComponent<Camera>();
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.Mouse1))
        {
            Vector3 deltaValue = (previousMousePos - Input.mousePosition);
            deltaValue.z = 0f;
            transform.position += deltaValue * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.W))
        {
            transform.position += Vector3.up * 10f * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            transform.position += Vector3.down * 10f * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.A))
        {
            transform.position += Vector3.left * 10f * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            transform.position += Vector3.right * 10f * Time.deltaTime;
        }

        previousMousePos = Input.mousePosition;
    }

    public static Vector3 CameraWorldPos()
    {
        return Vector3.Scale(instance.transform.position, new Vector3(1f, 1f, 0f));
    }

    public static bool CameraContainsPoint(Vector3 pos)
    {
        return instance.mainCamera.pixelRect.Contains(instance.mainCamera.WorldToScreenPoint(pos));
    }
}
