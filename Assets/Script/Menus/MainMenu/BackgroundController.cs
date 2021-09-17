using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackgroundController : MonoBehaviour {
    
    public float speed;
    public Sprite backgroundSprite;
    private GameObject parent;
    public float y;

    public bool hasGenImage;

    void Start(){
        parent = transform.parent.gameObject;
    }

    void Update() {

        float directionX = speed * Time.deltaTime;

        transform.Translate(-directionX,0,0);

        if(transform.localPosition.x <= 0 && !hasGenImage) {
            hasGenImage= true;
            CreateNewImage();
        }

        if(transform.localPosition.x  <= -2000) 
            Destroy(transform.gameObject);
        

    }

    private void CreateNewImage() {
        GameObject obj = new GameObject();
        Image image = obj.AddComponent<Image>(); 
        image.sprite = backgroundSprite;
        obj.GetComponent<RectTransform>().SetParent(parent.transform); 
        obj.transform.localScale = new Vector3(19.35519f,11.23341f,1f);
        obj.transform.localPosition = new Vector3(1565f,y,0f);
        obj.AddComponent<BackgroundController>();
        obj.GetComponent<BackgroundController>().speed = speed;
        obj.GetComponent<BackgroundController>().backgroundSprite = backgroundSprite;
        obj.GetComponent<BackgroundController>().parent = parent;
        obj.GetComponent<BackgroundController>().y = y;

        obj.name = "Image";
        obj.SetActive(true);
    }
}
