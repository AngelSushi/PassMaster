using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
public class ShopEntity : MonoBehaviour {

    public Animator controller;
    
    public Vector3 idlePosition;
    public Vector3 idleRotation;
    
    public Vector3 sleepPosition;
    public Vector3 sleepRotation;

    private DayController dayController;

    private void Awake() => dayController = FindObjectOfType<DayController>();

    private void Start() {
        dayController.OnChangeStateOfDay += OnChangeStateOfDay;
        transform.position = idlePosition;
        transform.eulerAngles = idleRotation;
    }

    private void OnCollisionEnter(Collision collision) {
        
        if (collision.gameObject.CompareTag("Player")) 
            GameController.Instance.shopController.EnterShop();
        
    }


    private void OnChangeStateOfDay(object sender, DayController.OnChangeStateOfDayArgs e) {

        transform.position = e.newPeriod == DayController.DayPeriod.DAY || e.newPeriod == DayController.DayPeriod.DUSK
            ? idlePosition
            : sleepPosition;

        transform.eulerAngles =
            e.newPeriod == DayController.DayPeriod.DAY || e.newPeriod == DayController.DayPeriod.DUSK
                ? idleRotation
                : sleepRotation;
        
        controller.SetBool("IsSleeping",e.newPeriod == DayController.DayPeriod.NIGHT);
    }
}
