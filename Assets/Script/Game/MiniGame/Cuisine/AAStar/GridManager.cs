using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using Grid; 

public class GridManager : MonoBehaviour
{

    [SerializeField] private Texture2D model;
    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private Vector3 startPosition;

    private Tile[,] _grid;

    public Tile[,] Grid
    {
        get => _grid;
        private set => _grid = value;
    }
    
    private const int DIAGONAL_COST = 14;
    private const int STRAIGHT_COST = 10;

    private static GridManager _instance;

    public static GridManager Instance
    {
        get => _instance;
        private set => _instance = value;
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void Start()
    {
        GenerateGrid();

    }

    [ContextMenu("Generate Grid")]
    private void GenerateGrid()
    {
        GameObject debug = new GameObject("Debug");


        Grid = new Tile[model.width, model.height];
        for (int i = 0; i < model.height; i++)
        {
            for (int j = 0; j < model.width; j++)
            {
                Vector3 tilePosition = startPosition;
                tilePosition.x -= Vector3.right.magnitude * i * (tilePrefab.transform.localScale.x * 10);
                tilePosition.z -= Vector3.forward.magnitude * -1 * j * (tilePrefab.transform.localScale.y * 10);
                

                GameObject instance = Instantiate(tilePrefab, tilePosition,Quaternion.identity);
                Tile tile = new Tile(i, j, int.MaxValue, 0, 0,model.GetPixel(i,j) != Color.white,instance);
                _grid[i,j] = tile;
                
                if (tile.IsObstacle)
                {
                    instance.GetComponent<MeshRenderer>().material.color = Color.black;
                }
                
                
                instance.name = "Tile [" + (i) + "," + (j) + "]";
                instance.transform.parent = debug.transform;

                ManageGridUI(instance,tile);
            }
        }
    }

    private void ManageGridUI(GameObject instance,Tile tile)
    {
        instance.transform.GetChild(0).GetChild(0).position = Camera.main.WorldToScreenPoint(instance.transform.position);
        instance.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = "[ " + tile.X + "," + tile.Y + "]"; //tile.FCost.ToString();
        instance.transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
                
        instance.transform.GetChild(0).GetChild(1).position = Camera.main.WorldToScreenPoint(instance.transform.position + new Vector3(0,3,0));
        instance.transform.GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>().text = tile.GCost.ToString();

        instance.transform.GetChild(0).GetChild(2).position = Camera.main.WorldToScreenPoint(instance.transform.position + new Vector3(0,-3.5f,0));
        instance.transform.GetChild(0).GetChild(2).GetComponent<TextMeshProUGUI>().text = tile.HCost.ToString();
    }

    public List<Tile> GeneratePath(Tile startTile, Tile endTile)
    { 
        
        foreach (Tile tile in _grid)
        {
            tile.HCost = 0;
            tile.GCost = int.MaxValue;
            tile.CalculateFCost();
            tile.CameFromTile = null;
        }

        startTile.GCost = 0;
        startTile.HCost = CalculateDistance(startTile, endTile);
        startTile.CalculateFCost();
        
        List<Tile> searchList = new List<Tile>() { startTile };
        List<Tile> finalPath = new List<Tile>();

        while (searchList.Count > 0)
        {
            Tile currentTile = GetLowestFTile(searchList);

           if (currentTile == endTile)
           {
               return CalculatePath(endTile);
           }
           
           finalPath.Add(currentTile);
           searchList.Remove(currentTile);
           
           foreach (Tile neighbourTile in GetNeighboursTile(currentTile))
           {
               if (finalPath.Contains(neighbourTile))
               {
                   continue;
               }

               int newGCost = currentTile.GCost + CalculateDistance(currentTile, neighbourTile);
               
               if (newGCost < neighbourTile.GCost)
               {
                   neighbourTile.GCost = newGCost;
                   neighbourTile.HCost = CalculateDistance(neighbourTile, endTile);
                   neighbourTile.CalculateFCost();
                   neighbourTile.CameFromTile = currentTile;

                   if (!searchList.Contains(neighbourTile))
                   {
                       searchList.Add(neighbourTile);
                   }    
               }
               

           }
        }
       
//        Debug.Log("path is null " + finalPath.Count);
        return null;
    }

    private List<Tile> CalculatePath(Tile endTile)
    {
        List<Tile> path = new List<Tile>();
        path.Add(endTile);
        Tile currentTile = endTile;

        while (currentTile.CameFromTile != null)
        {
            if (!path.Contains(currentTile.CameFromTile))
            {
                path.Add(currentTile.CameFromTile);
                currentTile = currentTile.CameFromTile;   
            }
        }

        path.Reverse();
        return path;
    }

    private Tile GetLowestFTile(List<Tile> tiles)
    {
        Tile lowest = tiles[0];

        foreach (Tile tile in tiles)
        {
            if (tile.FCost < lowest.FCost) 
            {
                lowest = tile;
            }
        }

        return lowest;
    }

    private int CalculateDistance(Tile a,Tile b)
    {
        int xDistance = Mathf.Abs(a.X - b.X);
        int yDistance = Mathf.Abs(a.Y - b.Y);
        int remaining = Mathf.Abs(xDistance - yDistance);

        return DIAGONAL_COST * Mathf.Min(xDistance, yDistance) + STRAIGHT_COST * remaining;
    }

    private List<Tile> GetNeighboursTile(Tile tile)
    {
        List<Tile> neighboursTile = new List<Tile>();

        if (tile.X + 1 < model.width)
        {
            AddTileToNeighbours(_grid[tile.X + 1,tile.Y],neighboursTile);

            if (tile.Y + 1 < model.height)
            {
                AddTileToNeighbours(_grid[tile.X + 1,tile.Y + 1],neighboursTile);
            }

            if (tile.Y - 1 >= 0)
            {
                AddTileToNeighbours(_grid[tile.X + 1,tile.Y - 1],neighboursTile);
            }
        }

        if (tile.X - 1 >= 0)
        {
            AddTileToNeighbours(_grid[tile.X - 1,tile.Y],neighboursTile);

            if (tile.Y + 1 < model.height)
            {
                AddTileToNeighbours(_grid[tile.X - 1,tile.Y + 1],neighboursTile);
            }

            if (tile.Y - 1 >= 0)
            {
                AddTileToNeighbours(_grid[tile.X - 1,tile.Y - 1],neighboursTile);
            }
        }

        if (tile.Y + 1 < model.height)
        {
            AddTileToNeighbours(_grid[tile.X,tile.Y + 1],neighboursTile);
        }

        if (tile.Y - 1 >= 0)
        {
            AddTileToNeighbours(_grid[tile.X,tile.Y - 1],neighboursTile);
        }
        
        return neighboursTile;


    }

    private void AddTileToNeighbours(Tile tile,List<Tile> neighbours)
    {
        if (!tile.IsObstacle && !neighbours.Contains(tile))
        {
            neighbours.Add(tile);
        }
    }

    public Vector3 GetWorldPosition(Tile tile)
    {
        Vector3 worldPosition = startPosition;

        worldPosition.x -= Vector3.right.magnitude * tile.X * (tilePrefab.transform.localScale.x * 10);
        worldPosition.z -= Vector3.forward.magnitude * -1 * tile.Y * (tilePrefab.transform.localScale.y * 10);


        return worldPosition;
    }

    public Tile FromCoords(Vector2Int coords)
    {
        return _grid[coords.x, coords.y];
    }
}
