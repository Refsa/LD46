using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    Vector3 previousMousePos = Vector3.zero;

    void Update()
    {
        if (Input.GetKey(KeyCode.Mouse1))
        {
            Vector3 deltaValue = (previousMousePos - Input.mousePosition);
            deltaValue.z = 0f;
            transform.position += deltaValue * Time.deltaTime;
        }

        previousMousePos = Input.mousePosition;
    }
}
