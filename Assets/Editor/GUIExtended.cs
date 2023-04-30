using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityToolbarExtender;

[InitializeOnLoad]
public class GUIExtended {

    static GUIExtended() {
        ToolbarExtender.RightToolbarGUI.Add(OnRightToolbarGUI);
        ToolbarExtender.LeftToolbarGUI.Add(OnLeftToolbarGUI);
    }

    private static void OnRightToolbarGUI() {
        
        if( SceneManager.GetActiveScene().name != "NewMain")
            return;
        
        GUILayout.FlexibleSpace();

/*        GUI.backgroundColor = GameController.Instance.showStepNames ? Color.green : Color.red;
        if (GUILayout.Button("Enable/Disable Step Names")) 
            GameController.Instance.showStepNames = !GameController.Instance.showStepNames;

        GUI.backgroundColor = GameController.Instance.showStepDirections ? Color.green : Color.red;
        if (GUILayout.Button("Enable/Disable Step Directions") ) {
            GameController.Instance.showStepDirections = !GameController.Instance.showStepDirections; // Need To recompile
        }
      
  */    
    }

    private static void OnLeftToolbarGUI() {
        if (GUILayout.Button("Isle")) {
            EditorSceneManager.SaveOpenScenes(); 
            EditorSceneManager.OpenScene("Assets/Scenes/Mains/Isle/NewMain/NewMain.unity");
        }

        if (GUILayout.Button("Keyball")) {
            EditorSceneManager.SaveOpenScenes();
            EditorSceneManager.OpenScene("Assets/Scenes/MiniGames/Keyball/Keyball.unity");
        }

        if (GUILayout.Button("DrawMe")) {
            EditorSceneManager.SaveOpenScenes();
            EditorSceneManager.OpenScene("Assets/Scenes/MiniGames/DrawMe/DrawMe.unity");
        }

        if (GUILayout.Button("Surf")) {
            EditorSceneManager.SaveOpenScenes();
            EditorSceneManager.OpenScene("Assets/Scenes/MiniGames/Surf/Surf.unity");
        }
        
        if (GUILayout.Button("PerfectNote")) {
            EditorSceneManager.SaveOpenScenes();
            EditorSceneManager.OpenScene("Assets/Scenes/MiniGames/PerfectNote/PerfectNote.unity");
        }
        
        if (GUILayout.Button("Cuisine")) {
            EditorSceneManager.SaveOpenScenes();
            EditorSceneManager.OpenScene("Assets/Scenes/MiniGames/Cuisine/Cuisine.unity");
        }
    }
}
