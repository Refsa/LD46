using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    static GameManager instance;

    [SerializeField] Camera mainCamera;

    public static Camera MainCamera;

    private void Awake() 
    {
        if (instance == null) instance = this;
        else Destroy(this);

        MainCamera = mainCamera;
    }

    private void Start() 
    {
        for (int i = 0; i < 4; i++)
        {
            var randomCard = Node.GenerateRandomNode(ConnectorType.Connector);
            UIManager.HandUI.AddCard(randomCard.Item1, randomCard.Item2);
        }    
    }
}
