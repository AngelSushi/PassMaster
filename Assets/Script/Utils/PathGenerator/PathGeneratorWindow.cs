
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
        if (GUILayout.Button("Generate Stack Positions"))
            instance.GenerateStackPositions();
        if (GUILayout.Button("Clear")) {
            for (int i = 0; i < instance.target.transform.childCount; i++) {
                for (int j = 0; j < instance.target.transform.GetChild(i).childCount; j++) {
                    Debug.Log(instance.target.transform.GetChild(i).GetChild(j).gameObject + " j " + j);
                    if(j == 2 ||j == 3) 
                      DestroyImmediate(instance.target.transform.GetChild(i).GetChild(j).gameObject);
                }
            }
        }
        if(GUILayout.Button("Affect Button Parameters"))
            instance.AffectParameters();
        
    }

}
