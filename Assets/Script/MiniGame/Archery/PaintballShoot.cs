using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PaintballShoot : CoroutineSystem {
    
    public ArcheryController controller;
    public GameObject ammoParent;
    public GameObject ammoPrefab;
    public float speedAmmo;
    public bool check;

    private int reload = 35;
    private int ammo = 30;
    public bool shoot;
    private RaycastHit hit;
    private Vector3 direction;
    private GameObject ammoObj;

    void Update() {
        Debug.DrawLine(transform.position,transform.forward,Color.red);

        if(shoot) {
            ammoObj.transform.position = Vector3.MoveTowards(ammoObj.transform.position,direction,speedAmmo * Time.deltaTime);

            if(ammoObj.transform.position == direction && !check) {
                RunDelayed(0.05f,() => {
                    if(ammoObj != null) 
                        direction = new Vector3(direction.x,direction.y,direction.z - 50);
                });
                check = true;
            }
        }
    }

    void FixedUpdate() {
        Ray rayOrigin = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

        if(Physics.Raycast(rayOrigin,out hit) && hit.collider != null && !controller.begin && !controller.finish) {
            Vector3 direction = hit.point - transform.GetChild(1).position;
            transform.GetChild(1).LookAt(hit.point,Vector3.up);
        }
    }

    public void OnShoot(InputAction.CallbackContext e) {
        if(e.started && ammo > 0 && !shoot && !controller.begin && !controller.finish) {
            ammo--;
            shoot = true;
            ammoObj = Instantiate(ammoPrefab,transform.GetChild(1).GetChild(0).GetChild(0).position,Quaternion.Euler(0,0,0));
            ammoObj.GetComponent<Ammo>().player = transform.gameObject;
            ammoObj.GetComponent<Ammo>().controller = controller;
            
            direction = hit.point;
            //ammoObj.GetComponent<Rigidbody>().velocity = Camera.main.ScreenToWorldPoint(new Vector3(0,0,1)) * -1;


            ManageHudAmmo();
        }
    }

    public void OnReload(InputAction.CallbackContext e) {
        if(e.started && reload > 0 && !controller.begin) {

            if(reload >= 15) {
                ammo += 15;
                reload -= 15;
            }
            else {
                ammo += reload;
                reload = 0;
            }

            ManageHudAmmo();
        }
    }

    private void ManageHudAmmo() {
        if(ammo >= 10) {
            ammoParent.transform.GetChild(0).gameObject.GetComponent<Text>().text = "" + ammo;
            ammoParent.transform.GetChild(0).gameObject.GetComponent<Outline>().enabled = false;
        }
        else {
            ammoParent.transform.GetChild(0).gameObject.GetComponent<Text>().text = "0" + ammo;
            ammoParent.transform.GetChild(0).gameObject.GetComponent<Outline>().enabled = true;
        }

        if(reload >= 10) {
            ammoParent.transform.GetChild(2).gameObject.GetComponent<Text>().text = "" + reload;
            ammoParent.transform.GetChild(0).gameObject.GetComponent<Outline>().enabled = false;
        }
        else {
            ammoParent.transform.GetChild(2).gameObject.GetComponent<Text>().text = "0" + reload;
            ammoParent.transform.GetChild(2).gameObject.GetComponent<Outline>().enabled = true;
        }
        
    }

    // LookAt pour voir ou la souris est
}
