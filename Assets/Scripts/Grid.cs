using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

public struct GridNode
{

}

public class Grid : MonoBehaviour
{
    [SerializeField] int gridSize;
    [SerializeField] GameObject gridPrefab;

    void Awake()
    {
        GenerateGrid();
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.Mouse0))
        {
            Vector3 mousePosWorld = math.floor(GameManager.MainCamera.ScreenToWorldPoint(Input.mousePosition));

        }
    }

    void GenerateGrid()
    {
        int halfSize = gridSize / 2;
        for (int i = -halfSize; i < halfSize; i++)
        {
            for (int j = -halfSize; j < halfSize; j++)
            {
                Vector3 pos = new Vector3(i, j, 0);
                var newTile = GameObject.Instantiate(gridPrefab, transform);
                newTile.transform.position = pos;
            }
        }
    }
}
