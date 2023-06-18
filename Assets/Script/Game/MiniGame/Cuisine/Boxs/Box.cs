using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Grid;

public abstract class Box : MonoBehaviour {

    [HideInInspector] public ChefController currentController;
    protected CookController _cookController;


    [SerializeField] private Vector2Int _tileCoords;
    

    private Tile _tile;
    
    public Tile Tile
    {
        get
        {
            _tile = GridManager.Instance.Grid[_tileCoords.x, _tileCoords.y];
            return _tile;
        }
    }

    protected virtual void Start() {
       _cookController = (CookController) CookController.instance;
    }
    
    public virtual void BoxInteract(GameObject current,ChefController controller) {
        currentController = controller;
        
        if (current != null)
            Put();
        else
            Take();
    }

    

    protected abstract void Put(); // Called when the player put something on the box
    protected abstract void Take(); // Called when the player take something on the box


}
