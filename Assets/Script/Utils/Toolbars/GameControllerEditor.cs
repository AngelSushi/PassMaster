using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Rendering;

[CustomEditor(typeof(GameController))]
public class GameControllerEditor : Editor {
    
    private GameController classTarget;
    private SerializedObject serializedClass;
    
    
    // Controllers
    private SerializedProperty day, dialog, mg, order, shop, chest, item, endAnimation, debug;
    private void OnEnable() {
        classTarget = (GameController)target;
        serializedClass = new SerializedObject(classTarget);

        
        // Controllers 
        day = serializedClass.FindProperty("dayController");
        dialog = serializedClass.FindProperty("dialog");
        mg = serializedClass.FindProperty("mgController");
        order = serializedClass.FindProperty("orderController");
        shop = serializedClass.FindProperty("shopController");
        chest = serializedClass.FindProperty("chestController");
        item = serializedClass.FindProperty("itemController");
        endAnimation = serializedClass.FindProperty("endAnimationController");
        debug = serializedClass.FindProperty("debugController");
    }

    public override void OnInspectorGUI() {
        serializedClass.Update();
        EditorGUI.BeginChangeCheck();

        classTarget.currentTabIndex = GUILayout.Toolbar(classTarget.currentTabIndex,new string[] {"General","Controller"});


        switch (classTarget.currentTabIndex) {
            case 0:
                break;
            
            case 1:
                EditorGUILayout.PropertyField(day);
                EditorGUILayout.PropertyField(dialog);
                EditorGUILayout.PropertyField(mg);
                EditorGUILayout.PropertyField(order);
                EditorGUILayout.PropertyField(shop);
                EditorGUILayout.PropertyField(chest);
                EditorGUILayout.PropertyField(item);
                EditorGUILayout.PropertyField(endAnimation);
                EditorGUILayout.PropertyField(debug);
                break;
        }
        
        if(EditorGUI.EndChangeCheck()) {
            serializedClass.ApplyModifiedProperties();
            //  GUI.FocusControl(null); 
        }
    }
}
