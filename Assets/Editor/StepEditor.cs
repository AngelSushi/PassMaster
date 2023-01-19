using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Step))]
public class StepEditor : CustomFieldInspector<Step> {

private SerializedProperty useVectors,positive,type,chest,shop,avoidPos;
    
    
    protected override void OnEnable() {
        base.OnEnable();

        useVectors = serializedClass.FindProperty("useVectors");
        positive = serializedClass.FindProperty("positive");
        type = serializedClass.FindProperty("type");

        chest = serializedClass.FindProperty("chest");
        shop = serializedClass.FindProperty("shop");

        avoidPos = serializedClass.FindProperty("avoidPos");
    }

    public override void OnInspectorGUI() {
        serializedClass.Update();
        EditorGUI.BeginChangeCheck();
        
        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("Step", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(useVectors);
        EditorGUILayout.PropertyField(positive);
        EditorGUILayout.PropertyField(type);
        
        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("Environment",EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(chest);
        EditorGUILayout.PropertyField(shop);
        
        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("AI",EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(avoidPos);
        

        if(EditorGUI.EndChangeCheck()) 
            serializedClass.ApplyModifiedProperties();
    }
    
}
