using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plate : MonoBehaviour {

    private CookController _cookController;
    public List<Ingredient> ingredientsInPlate;
    public List<RecipeController.Recipe> availableRecipes;

    private GameObject _plateUIParent;
    public Vector2 plateOffset;

    private void Start() {
        _cookController = (CookController)CookController.instance;
        SetupPlateUI();
    }

    public void SetupPlateUI() {
        GameObject plateCanvasObj = new GameObject("Plate Canvas");
        Canvas plateCanvas = plateCanvasObj.AddComponent<Canvas>();
        plateCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        plateCanvas.transform.parent = transform;
        
        _plateUIParent = Instantiate(_cookController.platePrefab,plateCanvasObj.transform);
        _plateUIParent.SetActive(false);

    }

    public void Update() {
        Vector3 plateUIPosition = _cookController.instances[transform.parent.GetComponent<ZoneSwapper>().areaIndex].instanceCamera.WorldToScreenPoint(transform.position);
        plateUIPosition += (Vector3)plateOffset;
        _plateUIParent.transform.position = plateUIPosition;
    }
    
    public void AddIngredient(Ingredient newIngredient,Box box) {
        if(!_plateUIParent.activeSelf)
            _plateUIParent.SetActive(true);
    }
}
