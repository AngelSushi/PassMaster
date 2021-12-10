using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(UserMovement))]
public class UserMovementEditor : Editor {
    
    private SerializedProperty id;
    private SerializedProperty diceResult;
    
    private UserMovement classTarget;
    private SerializedObject serializedClass;

    private void OnEnable() {
        classTarget = (UserMovement)target;
        serializedClass = new SerializedObject(classTarget);

        id = serializedClass.FindProperty("id");
        diceResult = serializedClass.FindProperty("diceResult");
    }
    public override void OnInspectorGUI() {
       // DrawDefaultInspector();

        serializedClass.Update();
        EditorGUI.BeginChangeCheck();

        classTarget.currentTabIndex = GUILayout.Toolbar(classTarget.currentTabIndex,new string[] {"Controller","Player","Movement","Object","UI"});

        switch(classTarget.currentTabIndex) {
            case 0:
                classTarget.currentTabName = "Controller";
                EditorGUILayout.PropertyField(diceResult);
                break;
            case 1:
                classTarget.currentTabName = "Player";
                EditorGUILayout.PropertyField(id);
                break;
            case 2:
                classTarget.currentTabName = "Movement";
                break;
            case 3:
                classTarget.currentTabName = "Object";
                break;
            case 4:
                classTarget.currentTabName = "UI";
                break;
        }

        if(EditorGUI.EndChangeCheck()) {
            serializedClass.ApplyModifiedProperties();
          //  GUI.FocusControl(null); 
        }
    }
}
