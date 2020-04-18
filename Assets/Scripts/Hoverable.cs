using UnityEngine;

public interface IHoverable
{
    void HoverEnter();
    void HoverExit();
}

public class Hoverable : MonoBehaviour 
{
    private void OnMouseEnter()
    {
        SendMessage("HoverEnter", SendMessageOptions.DontRequireReceiver);
    }

    private void OnMouseExit() 
    {
        SendMessage("HoverExit", SendMessageOptions.DontRequireReceiver);
    }
}