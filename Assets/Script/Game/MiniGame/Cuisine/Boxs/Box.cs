using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Grid;

public abstract class Box : MonoBehaviour {

    [HideInInspector] public ChiefController currentController;
    protected CookController _cookController;


    [SerializeField] private Vector2Int _tileCoords;

    public Vector2Int TileCoords
    {
        get => _tileCoords;
        private set => _tileCoords = value;
    }
    

    private Tile _tile;
    
    public Tile Tile
    {
        get
        {
            _tile = GridManager.Instance.Grid[_tileCoords.x, _tileCoords.y];
            return _tile;
        }
    }
    
    
    protected AudioSource audioSource;

    protected virtual void Start() {
       _cookController = (CookController) CookController.instance;

       audioSource = GetComponent<AudioSource>();
    }
    
    public virtual void BoxInteract(GameObject current,ChiefController controller) {
        currentController = controller;
        
        if (current != null)
            Put();
        else
            Take();
    }

    

    protected abstract void Put(); // Called when the player put something on the box
    protected abstract void Take(); // Called when the player take something on the box


}
