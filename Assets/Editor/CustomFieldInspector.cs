using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public abstract class CustomFieldInspector<T>  : Editor where T : MonoBehaviour {

    protected T classTarget;
    protected SerializedObject serializedClass;

    protected virtual void OnEnable() {
        classTarget = (T)target;
        serializedClass = new SerializedObject(classTarget);
    }
}
