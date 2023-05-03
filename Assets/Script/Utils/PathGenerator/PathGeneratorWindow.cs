
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
        if (GUILayout.Button("Clear Path Mesh")) {
            foreach (GameObject path in FindObjectsOfType<GameObject>()) {
                if (path != null && path.GetComponentInChildren<MeshRenderer>() != null && path.layer == 31) {
                    path.GetComponentInChildren<MeshRenderer>().enabled = true;
                    path.GetComponentInChildren<MeshRenderer>().material = instance.pathMat;
                }
            }
        }
        if(GUILayout.Button("Affect Button Parameters"))
            instance.AffectParameters();
        
    }

}
