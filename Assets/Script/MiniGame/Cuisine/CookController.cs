using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CookController : MonoBehaviour {

    [System.Serializable]
    public class Recipes {
        public List<string> ingredients;
    }
    
    public static CookController instance;
    public List<Recipes> recipes;

    void Awake() {
        instance = this;
    }
    
}
