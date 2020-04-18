using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IOnClick
{
    void MouseDown();
    void MouseUp();
}

public class Clickable : MonoBehaviour
{
    public void OnMouseDown() 
    {
        SendMessage("MouseDown", SendMessageOptions.DontRequireReceiver);
    }

    public void OnMouseUp() 
    {
        SendMessage("MouseUp", SendMessageOptions.DontRequireReceiver);
    }
}
