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
    
    // General
    private SerializedProperty firstStep,actualPlayer,players,part,posBegin,stackPos,stackSize,smallSprites,classedColors,turn,difficulty,freeze,diceMaterials,nightIndex,hasChangeState,blackScreenAnim,shellPrefab,mainPath;
    
    // Controllers
    private SerializedProperty day, dialog, mg, order, shop, chest, item, endAnimation, debug,loadingScene;
    
    // Files
    private SerializedProperty dialogsFile, stepFile, excelArray;
    
    // Chest&Code
    private SerializedProperty chestParent, actualChest,secretCode,hasGenChest,stepChest;
    
    private void OnEnable() {
        classTarget = (GameController)target;
        serializedClass = new SerializedObject(classTarget);

        // General
        
        firstStep = serializedClass.FindProperty("firstStep");
        actualPlayer = serializedClass.FindProperty("actualPlayer");
        players = serializedClass.FindProperty("players");
        part = serializedClass.FindProperty("part");
        posBegin = serializedClass.FindProperty("posBegin");
        stackPos = serializedClass.FindProperty("stackPos");
        stackSize = serializedClass.FindProperty("stackSize");
        smallSprites = serializedClass.FindProperty("smallSprites");
        classedColors = serializedClass.FindProperty("classedColors");
        turn = serializedClass.FindProperty("turn");
        difficulty = serializedClass.FindProperty("difficulty");
        freeze = serializedClass.FindProperty("freeze");
        diceMaterials = serializedClass.FindProperty("diceMaterials");
        nightIndex = serializedClass.FindProperty("nightIndex");
        hasChangeState = serializedClass.FindProperty("hasChangeState");
        blackScreenAnim = serializedClass.FindProperty("blackScreenAnim");
        shellPrefab = serializedClass.FindProperty("shellPrefab");
        mainPath = serializedClass.FindProperty("mainPath");
        
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
        loadingScene = serializedClass.FindProperty("loadingScene");
        
        // Files
        dialogsFile = serializedClass.FindProperty("dialogsFile");
        stepFile = serializedClass.FindProperty("stepFile");
        excelArray = serializedClass.FindProperty("excelArray");
        
        // Chest&Code
        chestParent = serializedClass.FindProperty("chestParent");
        actualChest = serializedClass.FindProperty("actualChest");
        secretCode = serializedClass.FindProperty("secretCode");
        hasGenChest = serializedClass.FindProperty("hasGenChest");
        stepChest = serializedClass.FindProperty("stepChest");
    }

    public override void OnInspectorGUI() {
        serializedClass.Update();
        EditorGUI.BeginChangeCheck();

        classTarget.currentTabIndex = GUILayout.Toolbar(classTarget.currentTabIndex,new string[] {"General","Controller","Files","Chest&Code"});
        //DrawDefaultInspector();
        
        switch (classTarget.currentTabIndex) {
            case 0:
                EditorGUILayout.PropertyField(firstStep);
                EditorGUILayout.PropertyField(actualPlayer);
                EditorGUILayout.PropertyField(players);
                EditorGUILayout.PropertyField(part);
                EditorGUILayout.PropertyField(posBegin);
                EditorGUILayout.PropertyField(stackPos);
                EditorGUILayout.PropertyField(stackSize);
                EditorGUILayout.PropertyField(smallSprites);
                EditorGUILayout.PropertyField(classedColors);
                EditorGUILayout.PropertyField(turn);
                EditorGUILayout.PropertyField(difficulty);
                EditorGUILayout.PropertyField(freeze);
                EditorGUILayout.PropertyField(diceMaterials);
                EditorGUILayout.PropertyField(nightIndex);
                EditorGUILayout.PropertyField(hasChangeState);
                EditorGUILayout.PropertyField(blackScreenAnim);
                EditorGUILayout.PropertyField(shellPrefab);
                EditorGUILayout.PropertyField(mainPath);
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
                EditorGUILayout.PropertyField(loadingScene);
                break;
            
            case 2:
                EditorGUILayout.PropertyField(dialogsFile);
                EditorGUILayout.PropertyField(stepFile);
                EditorGUILayout.PropertyField(excelArray);
                break;
            
            case 3:
                EditorGUILayout.PropertyField(chestParent);
                EditorGUILayout.PropertyField(actualChest);
                EditorGUILayout.PropertyField(secretCode);
                EditorGUILayout.PropertyField(hasGenChest);
                EditorGUILayout.PropertyField(stepChest);
                break;
        }
        
        if(EditorGUI.EndChangeCheck()) {
            serializedClass.ApplyModifiedProperties();
            //  GUI.FocusControl(null); 
        }
    }
}
