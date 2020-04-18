using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

public class Grid : MonoBehaviour
{
    [SerializeField] int gridSize;
    [SerializeField] GameObject gridTilePrefab;

    GridTile[,] grid;

    public int GridSize => gridSize;

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
        grid = new GridTile[gridSize, gridSize];

        int halfSize = gridSize / 2;
        int x = 0;
        int y = 0;

        for (int i = -halfSize; i < halfSize; i++)
        {
            for (int j = -halfSize; j < halfSize; j++)
            {
                Vector3 pos = new Vector3(i, j, 0);
                var newTile = GameObject.Instantiate(gridTilePrefab, transform);
                newTile.name = $"Grid_{x}_{y}";
                newTile.transform.position = pos;

                grid[x, y++] = newTile.GetComponent<GridTile>();
                grid[x, y - 1].GridPos = new Vector2Int(x, y - 1);
            }
            y = 0;
            x++;
        }
    }

    public GridTile GetTile(int x, int y)
    {
        if (x < 0 || x >= gridSize || y < 0 || y >= gridSize) return null;

        return grid[x, y];
    }
}
