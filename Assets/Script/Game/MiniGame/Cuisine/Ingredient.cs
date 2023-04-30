using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ingredient : MonoBehaviour {

    public int id;
    public bool isCuttable;
    public bool isCut;
    public bool isCookable;
    public bool isCook;
    public Sprite sprite;
    
    [Range(-1,1)] [Tooltip("-1 : Ignore ; 0 : Stove ; 1 : Pan")] public int cookIndex; 


    [HideInInspector] public GameObject basic;
    [HideInInspector] public GameObject modified;

    private void Start() {
        basic = transform.GetChild(0).gameObject;
        modified = transform.GetChild(1).gameObject;
    }

    private void Update() {
        Debug.DrawLine(transform.position,transform.position + transform.right * -1 * 100,Color.black);
        Debug.DrawLine(transform.position, transform.position + Vector3.forward  * 100, Color.red);
        Debug.DrawLine(transform.position,transform.position + Vector3.up * -1 * 100,Color.blue);

        if (transform.parent.gameObject.TryGetComponent <ChefController>(out ChefController chefController)) {
           /* if (basic.activeSelf)
                basic.transform.eulerAngles = holdRotationBasic;
            if (cut.activeSelf)
                cut.transform.eulerAngles = holdRotationCut;
            if (cook.activeSelf)
                cook.transform.eulerAngles = holdRotationCook;          
*/
           
           //transform.LookAt(Vector3.up);
           //Quaternion rotation = Quaternion.LookRotation(Vector3.up);
            // rotation.x -= 90;
            // basic.transform.rotation = rotation;
        }
    }
}
