using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(NoteCreator))]
public class NoteCreatorEditor : CustomFieldInspector<NoteCreator> {
    
    private SerializedProperty notesAsset,notesArray;

    protected override void OnEnable() {
        base.OnEnable();

        notesAsset = serializedClass.FindProperty("notesAsset");
        notesArray = serializedClass.FindProperty("notesArray");
    }

    public override void OnInspectorGUI() {
        serializedClass.Update();
        EditorGUI.BeginChangeCheck();
        
        base.OnInspectorGUI();

        if (GUILayout.Button("Generate Notes")) {
            if (classTarget.notesAsset == null) {
                Debug.LogError("Can't generate notes without text asset");
                return;
            }
            
            if (classTarget.controller == null) {
                Debug.LogError("Can't generate notes without controller");
                return;
            }
            
       //     classTarget.GenerateAllNotes();
        }
        
        if(EditorGUI.EndChangeCheck()) 
            serializedClass.ApplyModifiedProperties();
    }
}
