using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Box : MonoBehaviour {

    protected ChefController currentController;
    protected CookController _cookController;

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
