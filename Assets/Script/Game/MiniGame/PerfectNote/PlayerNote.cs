using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerNote : MonoBehaviour {
   
   public GameObject[] notesButton;

   public void OnPressNoteOne(InputAction.CallbackContext e) {
       if(e.started) 
           Press(notesButton[0]);
        if(e.canceled) 
            Release(notesButton[0]);
   }

   public void OnPressNoteTwo(InputAction.CallbackContext e) {
       if(e.started) 
           Press(notesButton[1]);
        if(e.canceled) 
            Release(notesButton[1]);
   }

   public void OnPressNotThree(InputAction.CallbackContext e) {
       if(e.started) 
           Press(notesButton[2]);
        if(e.canceled) 
            Release(notesButton[2]);
   }

   public void OnPressNoteFour(InputAction.CallbackContext e) {
       if(e.started) 
           Press(notesButton[3]);
        if(e.canceled) 
            Release(notesButton[3]);
   }

   public void OnPressNoteFive(InputAction.CallbackContext e) {
       if(e.started) 
           Press(notesButton[4]);
        if(e.canceled) 
            Release(notesButton[4]);
   }

   public void OnPressNoteSix(InputAction.CallbackContext e) {
       if(e.started) 
           Press(notesButton[5]);
        if(e.canceled) 
            Release(notesButton[5]);
   }

   public void OnPressNoteSeven(InputAction.CallbackContext e) {
       if(e.started) 
           Press(notesButton[6]);
        if(e.canceled) 
            Release(notesButton[6]);
   }

   private void Press(GameObject button) {
       button.transform.localScale = new Vector3(button.transform.localScale.x,0.8f,button.transform.localScale.z);
   }

   private void Release(GameObject button) {
       button.transform.localScale = new Vector3(button.transform.localScale.x,1.15f,button.transform.localScale.z);
   }
}
