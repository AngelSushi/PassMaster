using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(UserMovement))]
public class UserMovementEditor : Editor {
    private UserMovement classTarget;
    private SerializedObject serializedClass;

    // Controller Tab
    private SerializedProperty movement,ui,inventory,audio,gameController,userCam;

    // Player tab
    private SerializedProperty id,isTurn,isPlayer,agent,rb,canMoove,isMooving,canJump,isJumping,jumpSpeed;

    // Movement tab
    private SerializedProperty waitDiceResult,finishMovement,finishTurn,left,front,right,stop,lastStepIsArrow,waitChest,goToChest,returnStepBack,goToShop,
    canMooveToShop,reverseCount,diceResult,actualStep,beginStep,nextStep,stack,beginResult,stepPaths,stepBack;

    // Object tab;
    private SerializedProperty doubleDice,reverseDice,hasBotBuyItem,isParachuting;
    // UI Tab
    private SerializedProperty giveUI,changeUI,stepMaterial;
    

    private void OnEnable() {
        classTarget = (UserMovement)target;
        serializedClass = new SerializedObject(classTarget);

        // Controller tab
        movement = serializedClass.FindProperty("movement");
        ui = serializedClass.FindProperty("ui");
        inventory = serializedClass.FindProperty("inventory");
        audio = serializedClass.FindProperty("audio");
        gameController = serializedClass.FindProperty("gameController");
        userCam = serializedClass.FindProperty("userCam");

        // Player tab
        id = serializedClass.FindProperty("id");
        isTurn = serializedClass.FindProperty("isTurn");
        isPlayer = serializedClass.FindProperty("isPlayer");
        agent = serializedClass.FindProperty("agent");
        rb = serializedClass.FindProperty("rb");
        canMoove = serializedClass.FindProperty("canMoove");
        isMooving = serializedClass.FindProperty("isMooving");
        canJump = serializedClass.FindProperty("canJump");
        isJumping = serializedClass.FindProperty("isJumping");
        jumpSpeed = serializedClass.FindProperty("jumpSpeed");

        // Movement tab
        waitDiceResult = serializedClass.FindProperty("waitDiceResult");
        finishMovement = serializedClass.FindProperty("finishMovement");
        finishTurn = serializedClass.FindProperty("finishTurn");
        left = serializedClass.FindProperty("left");
        front = serializedClass.FindProperty("front");
        right = serializedClass.FindProperty("right");
        stop = serializedClass.FindProperty("stop");
        lastStepIsArrow = serializedClass.FindProperty("lastStepIsArrow");
        waitChest = serializedClass.FindProperty("waitChest");
        goToChest = serializedClass.FindProperty("goToChest");
        returnStepBack = serializedClass.FindProperty("returnStepBack");
        goToShop = serializedClass.FindProperty("goToShop");
        canMooveToShop = serializedClass.FindProperty("canMooveToShop");
        reverseCount = serializedClass.FindProperty("reverseCount");
        diceResult = serializedClass.FindProperty("diceResult");
        actualStep = serializedClass.FindProperty("actualStep");
        beginStep = serializedClass.FindProperty("beginStep");
        nextStep = serializedClass.FindProperty("nextStep");
        stack = serializedClass.FindProperty("stack");
        beginResult = serializedClass.FindProperty("beginResult");
        stepPaths = serializedClass.FindProperty("stepPaths");
        stepBack = serializedClass.FindProperty("stepBack");

        // Object tab
        doubleDice = serializedClass.FindProperty("doubleDice");
        reverseDice = serializedClass.FindProperty("reverseDice");
        hasBotBuyItem = serializedClass.FindProperty("hasBotBuyItem");
        isParachuting = serializedClass.FindProperty("isParachuting");

        // UI Tab
        giveUI = serializedClass.FindProperty("giveUI");
        changeUI = serializedClass.FindProperty("changeUI");
        stepMaterial = serializedClass.FindProperty("stepMaterial");
    }
    public override void OnInspectorGUI() {
        serializedClass.Update();
        EditorGUI.BeginChangeCheck();

        classTarget.currentTabIndex = GUILayout.Toolbar(classTarget.currentTabIndex,new string[] {"Controller","Player","Movement","Object","UI"});

        switch(classTarget.currentTabIndex) {
            case 0: // Controller
                EditorGUILayout.PropertyField(movement);
                EditorGUILayout.PropertyField(ui);
                EditorGUILayout.PropertyField(inventory);
                EditorGUILayout.PropertyField(audio);
                EditorGUILayout.PropertyField(gameController);
                EditorGUILayout.PropertyField(userCam);
                break;
            case 1: // Player
                EditorGUILayout.PropertyField(id);
                EditorGUILayout.PropertyField(isTurn);
                EditorGUILayout.PropertyField(isPlayer);
                EditorGUILayout.PropertyField(agent);
                EditorGUILayout.PropertyField(rb);
                EditorGUILayout.PropertyField(canMoove);
                EditorGUILayout.PropertyField(isMooving);
                EditorGUILayout.PropertyField(canJump);
                EditorGUILayout.PropertyField(isJumping);
                EditorGUILayout.PropertyField(jumpSpeed);
                break;
            case 2:
                EditorGUILayout.PropertyField(waitDiceResult);
                EditorGUILayout.PropertyField(finishMovement);
                EditorGUILayout.PropertyField(finishTurn);
                EditorGUILayout.PropertyField(left);
                EditorGUILayout.PropertyField(front);
                EditorGUILayout.PropertyField(right);
                EditorGUILayout.PropertyField(stop);
                EditorGUILayout.PropertyField(lastStepIsArrow);
                EditorGUILayout.PropertyField(waitChest);
                EditorGUILayout.PropertyField(goToChest);
                EditorGUILayout.PropertyField(returnStepBack);
                EditorGUILayout.PropertyField(goToShop);
                EditorGUILayout.PropertyField(canMooveToShop);
                EditorGUILayout.PropertyField(reverseCount);
                EditorGUILayout.PropertyField(diceResult);
                EditorGUILayout.PropertyField(actualStep);
                EditorGUILayout.PropertyField(beginStep);
                EditorGUILayout.PropertyField(nextStep);
                EditorGUILayout.PropertyField(stack);
                EditorGUILayout.PropertyField(beginResult);
                EditorGUILayout.PropertyField(stepPaths);
                EditorGUILayout.PropertyField(stepBack);
                break;
            case 3:
                EditorGUILayout.PropertyField(doubleDice);
                EditorGUILayout.PropertyField(reverseDice);
                EditorGUILayout.PropertyField(hasBotBuyItem);
                EditorGUILayout.PropertyField(isParachuting);
                break;
            case 4:
                EditorGUILayout.PropertyField(giveUI);
                EditorGUILayout.PropertyField(changeUI);
                EditorGUILayout.PropertyField(stepMaterial);
                break;
        }

        if(EditorGUI.EndChangeCheck()) {
            serializedClass.ApplyModifiedProperties();
          //  GUI.FocusControl(null); 
        }
    }
}
