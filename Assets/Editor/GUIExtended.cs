using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityToolbarExtender;

[InitializeOnLoad]
public class GUIExtended {

    static GUIExtended() {
        ToolbarExtender.RightToolbarGUI.Add(OnRightToolbarGUI);
    }

    private static void OnRightToolbarGUI() {
        GUILayout.FlexibleSpace();

        GUI.backgroundColor = GameController.Instance.showStepNames ? Color.green : Color.red;
        if (GUILayout.Button("Enable/Disable Step Names")) 
            GameController.Instance.showStepNames = !GameController.Instance.showStepNames;

        GUI.backgroundColor = GameController.Instance.showStepDirections ? Color.green : Color.red;
        if (GUILayout.Button("Enable/Disable Step Directions")) {
            GameController.Instance.showStepDirections = !GameController.Instance.showStepDirections;
            // Need To Recompile Script to Disable it 
        }
    }
}
