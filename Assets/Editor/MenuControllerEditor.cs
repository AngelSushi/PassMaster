using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cinemachine.Editor;
using UnityEditor;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;
using UnityEngine.UI;

[CustomEditor(typeof(MenuController))]
[CanEditMultipleObjects]
public class MenuControllerEditor : CustomFieldInspector<MenuController>
{

    private GUIStyle textStyle;
    public override void OnInspectorGUI() {
        InitStyles();
        SerializedProperty actionAsset = serializedClass.FindProperty("inputs");
        SerializedProperty buttons = serializedClass.FindProperty("buttons");
        SerializedProperty subMenus = serializedClass.FindProperty("subMenus");

        EditorGUILayout.PropertyField(actionAsset);
        
        EditorGUILayout.Space(30);
        DrawStartMenu();
        EditorGUILayout.Space(30);
        EditorGUILayout.PropertyField(subMenus);
        EditorGUILayout.Space(30);
        DrawButtons();

        if(EditorGUI.EndChangeCheck()) 
            serializedClass.ApplyModifiedProperties();
        

        void DrawStartMenu() {
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("Start Menu",textStyle);
            EditorGUILayout.PropertyField(serializedClass.FindProperty("actionName"));
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(serializedClass.FindProperty("startAction"));
            EditorGUILayout.EndVertical();
        }

        void DrawButtons() {

            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Buttons Menu ",textStyle);
            if (GUILayout.Button(new GUIContent("X", "Clear all buttons"), GUILayout.Width(30))) {
                if (EditorUtility.DisplayDialog("Delete all buttons", "Do you want delete all buttons ?", "Yes", "No")) {
                    classTarget.buttons.Clear();
                    buttons.ClearArray();
                }
            }
            EditorGUILayout.EndHorizontal();
            
            for (int i = 0; i < buttons.arraySize; i++) {
                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.PropertyField(buttons.GetArrayElementAtIndex(i));
                if (GUILayout.Button(new GUIContent("X", "Clear button"), GUILayout.Width(30))) {
                    if (EditorUtility.DisplayDialog("Delete button", "Do you want delete button ?", "Yes","No")) {
                        classTarget.buttons.RemoveAt(i);
                        buttons.DeleteArrayElementAtIndex(i);
                    }
                }
                
                EditorGUILayout.EndHorizontal();
            }
            
            if (GUILayout.Button(new GUIContent("Add button", ""))) {
                classTarget.buttons.Add(new MenuController.ButtonItem(null, null, new UnityEvent()));
                buttons.InsertArrayElementAtIndex(buttons.arraySize);
            }
            
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(30);
            
        }
    }

    void InitStyles() {
        textStyle = new GUIStyle();
        textStyle = GUI.skin.label;
        textStyle.alignment = TextAnchor.MiddleCenter;
        textStyle.fontStyle = FontStyle.Bold;
    }
}

