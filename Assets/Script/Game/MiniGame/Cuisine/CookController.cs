using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CookController : MonoBehaviour {

    public static CookController instance { get; private set; }

    public RecipeController recipeController;

    void Start() {
        instance = this;

        recipeController = GetComponent<RecipeController>();
    }

}
