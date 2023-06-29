using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using UnityEngine;

namespace Grid
{
    [Serializable]
    public class Tile 
    {
        [SerializeField]
        private int _x;
               
       public int X
       {
           get => _x;
           set => _x = value;
       }

       [SerializeField]
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
           set => _fCost = value;
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

       private Box _attachedBox;

       public Box AttachedBox
       {
           get
           {
               Box box = GameObject.FindObjectsOfType<Box>().FirstOrDefault(box => box.TileCoords == Coords);

               if (box != null)
               {
                   _attachedBox = box;
                   return _attachedBox;
               }
                
               Debug.Log("Error when getting attached box to tile " + Coords);
               return null;
           }
           
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


