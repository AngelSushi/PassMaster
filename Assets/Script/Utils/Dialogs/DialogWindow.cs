using UnityEngine;
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
        GUIStyle labelStyle = GUI.skin.GetStyle("Label");
       // int size = labelStyle.fontSize;
      //  labelStyle.fontSize = 15;
        EditorGUI.LabelField(new Rect((position.width / 3 - 200) / 2 - 50,(position.height / 2  - 50 ) / 2 - 30,100,20),"Any dialogs find",labelStyle);
       // labelStyle.fontSize = size;

        string[] assetNames = AssetDatabase.FindAssets("Your_Filter", new[] { "Assets/YourFolder" });
            
    }

}
