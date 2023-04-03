using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Rendering;
using UnityEngine;

[CustomEditor(typeof(KB_PlayerMovement))]
public class KB_PlayerMovementEditor : CustomFieldInspector<KB_PlayerMovement> {


    private SerializedProperty _controller, _cameraTransform,_gravity;
    private SerializedProperty _dead, _freeze;
    private SerializedProperty _speed, _isMoving;
    private SerializedProperty _canJump, _maxJumpTime, _isJumping,_maxJumpHeight,_isJumpPressed,_fallMultiplier;
    private SerializedProperty _slidingAmplifier;
    protected override void OnEnable() {
        base.OnEnable();

        _controller = serializedClass.FindProperty("controller");
        _cameraTransform = serializedClass.FindProperty("cameraTransform");
        _gravity = serializedClass.FindProperty("gravity");
        
        _dead = serializedClass.FindProperty("dead");
        _freeze = serializedClass.FindProperty("freeze");

        _speed = serializedClass.FindProperty("speed");
        _isMoving = serializedClass.FindProperty("isMoving");

        _canJump = serializedClass.FindProperty("canJump");
        _isJumping = serializedClass.FindProperty("isJumping");
        _isJumpPressed = serializedClass.FindProperty("isJumpPressed");
        _maxJumpHeight = serializedClass.FindProperty("maxJumpHeight");
        _maxJumpTime = serializedClass.FindProperty("maxJumpTime");
        _fallMultiplier = serializedClass.FindProperty("fallMultiplier");

        _slidingAmplifier = serializedClass.FindProperty("slidingAmplifier");
    }

    public override void OnInspectorGUI() {
        
        EditorGUI.BeginChangeCheck();
        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("Controllers",EditorStyles.boldLabel);
        
        EditorGUILayout.PropertyField(_controller);
        EditorGUILayout.PropertyField(_cameraTransform);
        EditorGUILayout.PropertyField(_gravity);

        EditorGUILayout.Space(20);
        EditorGUILayout.LabelField("Player State",EditorStyles.boldLabel);
        
        EditorGUILayout.PropertyField(_dead);
        EditorGUILayout.PropertyField(_freeze);
        
        EditorGUILayout.Space(20);
        EditorGUILayout.LabelField("Movement",EditorStyles.boldLabel);
        
        EditorGUILayout.PropertyField(_speed);
        EditorGUILayout.PropertyField(_isMoving);
        
        EditorGUILayout.Space(20);
        EditorGUILayout.LabelField("Jump",EditorStyles.boldLabel);
        
        EditorGUILayout.PropertyField(_canJump);
        EditorGUILayout.PropertyField(_isJumpPressed);
        EditorGUILayout.PropertyField(_isJumping);
        EditorGUILayout.PropertyField(_maxJumpHeight);
        EditorGUILayout.PropertyField(_maxJumpTime);
        EditorGUILayout.PropertyField(_fallMultiplier);
        
        
        EditorGUILayout.Space(20);
        EditorGUILayout.LabelField("Additional Force",EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(_slidingAmplifier);
        
        if(EditorGUI.EndChangeCheck()) {
            serializedClass.ApplyModifiedProperties();
            //  GUI.FocusControl(null); 
        }
    }
}
