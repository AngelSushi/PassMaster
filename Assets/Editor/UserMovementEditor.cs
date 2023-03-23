using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(UserMovement))]
public class UserMovementEditor : CustomFieldInspector<UserMovement> {


    // Controller Tab
    private SerializedProperty movement,ui,inventory,audio,gameController,userType,userAction,userCam;

    // Player tab
    private SerializedProperty id,isTurn,isPlayer,agent,rb,canMoove,isMooving,canJump,isJumping,jumpSpeed,animatorController;

    // Movement tab
    private SerializedProperty waitDiceResult,finishMovement,finishTurn,left,front,right,stop,bypassDirection,waitChest,reverseCount,bypassReverse,diceResult,actualStep,beginStep,nextStep,beginResult,stepPaths,stepBack;

    // Object tab;
    private SerializedProperty doubleDice,tripleDice,reverseDice,useHourglass,useLightning,isElectrocuted,useShell,targetLightningStep,checkObjectToUse;
    // UI Tab
    private SerializedProperty giveUI,changeUI,stepMaterial;
    

    protected override void OnEnable() {
        base.OnEnable();
        
        // Controller tab
        movement = serializedClass.FindProperty("movement");
        ui = serializedClass.FindProperty("ui");
        inventory = serializedClass.FindProperty("inventory");
        audio = serializedClass.FindProperty("audio");
        gameController = serializedClass.FindProperty("gameController");
        userType = serializedClass.FindProperty("userType");
        userAction = serializedClass.FindProperty("currentAction");
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
        animatorController = serializedClass.FindProperty("animatorController");

        // Movement tab
        waitDiceResult = serializedClass.FindProperty("waitDiceResult");
        finishMovement = serializedClass.FindProperty("finishMovement");
        finishTurn = serializedClass.FindProperty("finishTurn");
        left = serializedClass.FindProperty("left");
        front = serializedClass.FindProperty("front");
        right = serializedClass.FindProperty("right");
        stop = serializedClass.FindProperty("stop");
        bypassDirection = serializedClass.FindProperty("bypassDirection");
        waitChest = serializedClass.FindProperty("waitChest");
        reverseCount = serializedClass.FindProperty("reverseCount");
        bypassReverse = serializedClass.FindProperty("bypassReverse");
        diceResult = serializedClass.FindProperty("diceResult");
        actualStep = serializedClass.FindProperty("actualStep");
        beginStep = serializedClass.FindProperty("beginStep");
        nextStep = serializedClass.FindProperty("nextStep");
        beginResult = serializedClass.FindProperty("beginResult");
        stepPaths = serializedClass.FindProperty("stepPaths");
        stepBack = serializedClass.FindProperty("stepBack");

        // Object tab
        doubleDice = serializedClass.FindProperty("doubleDice");
        tripleDice = serializedClass.FindProperty("tripleDice");
        reverseDice = serializedClass.FindProperty("reverseDice");
        useHourglass = serializedClass.FindProperty("useHourglass");
        useLightning = serializedClass.FindProperty("useLightning");
        isElectrocuted = serializedClass.FindProperty("isElectrocuted");
        targetLightningStep = serializedClass.FindProperty("targetLightningStep");
        checkObjectToUse = serializedClass.FindProperty("checkObjectToUse");
        useShell = serializedClass.FindProperty("useShell");

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
                EditorGUILayout.PropertyField(userType);
                EditorGUILayout.PropertyField(userAction);
                EditorGUILayout.PropertyField(userCam);
                break;
            case 1: // Player
         //       EditorGUILayout.PropertyField(id);
                EditorGUILayout.PropertyField(isTurn);
                EditorGUILayout.PropertyField(isPlayer);
                EditorGUILayout.PropertyField(agent);
                EditorGUILayout.PropertyField(rb);
                EditorGUILayout.PropertyField(canMoove);
                EditorGUILayout.PropertyField(isMooving);
                EditorGUILayout.PropertyField(canJump);
                EditorGUILayout.PropertyField(isJumping);
                EditorGUILayout.PropertyField(jumpSpeed);
                EditorGUILayout.PropertyField(animatorController);
                break;
            case 2:
                EditorGUILayout.PropertyField(waitDiceResult);
                EditorGUILayout.PropertyField(finishMovement);
                EditorGUILayout.PropertyField(finishTurn);
                EditorGUILayout.PropertyField(left);
                EditorGUILayout.PropertyField(front);
                EditorGUILayout.PropertyField(right);
                EditorGUILayout.PropertyField(stop);
                EditorGUILayout.PropertyField(bypassDirection);
                EditorGUILayout.PropertyField(waitChest);
                EditorGUILayout.PropertyField(reverseCount);
                EditorGUILayout.PropertyField(bypassReverse);
                EditorGUILayout.PropertyField(diceResult);
                EditorGUILayout.PropertyField(actualStep);
                EditorGUILayout.PropertyField(beginStep);
                EditorGUILayout.PropertyField(nextStep);
                EditorGUILayout.PropertyField(beginResult);
                EditorGUILayout.PropertyField(stepPaths);
                EditorGUILayout.PropertyField(stepBack);
                break;
            case 3:
                EditorGUILayout.PropertyField(doubleDice);
                EditorGUILayout.PropertyField(tripleDice);
                EditorGUILayout.PropertyField(reverseDice);
                EditorGUILayout.PropertyField(useHourglass);
                EditorGUILayout.PropertyField(useLightning);
                EditorGUILayout.PropertyField(isElectrocuted);
                EditorGUILayout.PropertyField(targetLightningStep);
                if(!isPlayer.boolValue)
                    EditorGUILayout.PropertyField(checkObjectToUse);
                EditorGUILayout.PropertyField(useShell);
                break;
            case 4:
                EditorGUILayout.PropertyField(giveUI);
                EditorGUILayout.PropertyField(changeUI);
                break;
        }

        if(EditorGUI.EndChangeCheck()) {
            serializedClass.ApplyModifiedProperties();
          //  GUI.FocusControl(null); 
        }
    }
}
