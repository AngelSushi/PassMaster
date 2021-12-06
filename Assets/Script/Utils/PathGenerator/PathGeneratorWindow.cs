
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PathGenerator))]
public class PathGeneratorWindow : Editor {

    public override void OnInspectorGUI() {
        DrawDefaultInspector();

        PathGenerator instance = (PathGenerator)target;

        if(GUILayout.Button("Generate Path")) 
            instance.GeneratePath();
        if(GUILayout.Button("Generate Chests"))
            instance.GenerateChest();
        
    }

}
