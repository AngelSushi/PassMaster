using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using UnityEngine;

namespace Grid
{
    public class Tile 
    {
       private int _x;
               
       public int X
       {
           get => _x;
           set => _x = value;
       }

       private int _y;
       
       public int Y
       {
           get => _y;
           set => _y = value;
       }

       public Vector2 Coords
       {
           get => new Vector2(X, Y);
       }
       
       private int _gCost; // Walking cost from the start node
       
       public int GCost
       {
           get => _gCost;
           set => _gCost = value;
       }
       
       private int _hCost; // Heuristic cost to each end node 
       
       public int HCost
       {
           get => _hCost;
           set => _hCost = value;
       }

       private int _fCost; // G + H
       
       public int FCost
       {
           get => _fCost;
       }

       private bool _isObstacle;

       public bool IsObstacle
       {
           get => _isObstacle;
           private set => _isObstacle = value;
       }

       private GameObject _tileObj;

       public GameObject TileObj
       {
           get => _tileObj;
           private set => _tileObj = value;

       }

       private Tile _cameFromTile;

       public Tile CameFromTile
       {
           get => _cameFromTile;
           set => _cameFromTile = value;
       }

       public Tile(int x, int y, int gCost, int hCost, int fCost,bool isObstacle,GameObject tileObj)
       {
           _x = x;
           _y = y;
           _gCost = gCost;
           _hCost = hCost;
           _fCost = fCost;
           _isObstacle = isObstacle;
           _tileObj = tileObj;
       }

       public void CalculateFCost()
       {
           _fCost = _gCost + _hCost;
       }
    }
}


