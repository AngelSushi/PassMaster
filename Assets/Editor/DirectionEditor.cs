﻿using System;
using System.Collections;
using System.Collections.Generic;
using InspectorGadgets.Editor;
using UnityEditor;
using UnityEditor.Rendering;
using UnityEngine;

[CustomEditor(typeof(Direction))]
public class DirectionEditor : CustomFieldInspector<Direction> {

    private SerializedProperty directions, step, directionsStep,aiPaths,reversePath;
    
    protected override void OnEnable() {
        base.OnEnable();
        
        directions = serializedClass.FindProperty("directions");
        step = serializedClass.FindProperty("type");
        directionsStep = serializedClass.FindProperty("directionsStep");
        aiPaths = serializedClass.FindProperty("aiPaths");
        reversePath = serializedClass.FindProperty("reversePaths");
    }

    public override void OnInspectorGUI() { 
        serializedClass.Update();
        EditorGUI.BeginChangeCheck();
        
        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("Step", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(step);
        
        EditorGUILayout.Space(10);

        EditorGUILayout.LabelField("Directions Info", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(directions);
        EditorGUILayout.PropertyField(directionsStep);
        
        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("AI", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(aiPaths);
        EditorGUILayout.PropertyField(reversePath);
            
        if(EditorGUI.EndChangeCheck()) 
            serializedClass.ApplyModifiedProperties();
        
    }
}
