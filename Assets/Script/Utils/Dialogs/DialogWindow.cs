using UnityEngine;
using System.Collections.Generic;
using UnityEditor;

public class DialogWindow : EditorWindow {

    [MenuItem("Window/Dialogs")]
    public static void DisplayWindow() {
        GetWindow<DialogWindow>("Dialogs");
    }

    void OnGUI() {
        // Parts Draw 
        EditorGUI.DrawRect(new Rect(position.width / 3 - 200,0,1,position.height / 2  - 50),Color.gray);
        EditorGUI.DrawRect(new Rect(position.width / 3 * 2 + 140,0,1,position.height / 2  - 50),Color.gray);
        EditorGUI.DrawRect(new Rect(0,position.height / 2 - 50,position.width,1),Color.gray);

        // Title Draw
        
        
        List<DialogsSO> allDialogs = new List<DialogsSO>();

        string[] assetNames = AssetDatabase.FindAssets("", new[] { "Assets/Dialogs" });
        foreach (string name in assetNames)  {
            DialogsSO dialog = AssetDatabase.LoadAssetAtPath<DialogsSO>(AssetDatabase.GUIDToAssetPath(name));
            
            if(dialog != null && !allDialogs.Contains(dialog)) 
                allDialogs.Add(dialog);    
        }

        if(allDialogs.Count == 0) {
            EditorGUI.LabelField(new Rect((position.width / 3 - 200) / 2 - 50,(position.height / 2  - 50 ) / 2 - 30,100,20),"Any dialogs find");
            return;
        }

        GUIStyle btnStyle = GUI.skin.button;
        
        foreach(DialogsSO dialog in allDialogs) {
            Color c = GUI.backgroundColor;
            RectOffset border = GUI.skin.button.border;

            GUI.backgroundColor = Color.clear;
            GUI.skin.button.alignment = TextAnchor.MiddleLeft;
            GUI.skin.button.border = new RectOffset(0,0,0,0);
            GUILayout.Button(dialog.name,btnStyle,GUILayout.Width(position.width / 3 - 200)); 
         //   GUILayout.Button("-",btnStyle,GUILayout.Width(20));    

            GUI.backgroundColor = c;
            GUI.skin.button.border = border;
        }
    }
}
