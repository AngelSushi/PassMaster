using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SimpleDialog", menuName = "Dialogs/SimpleDialog", order = 1)]
public class DialogsSO : ScriptableObject {

    public string name;
    public string author;
    public string[] contents;
    public bool needAnswer;
    public string[] answers;
    public bool repeatable;

    // Ajouter des actions pour déclencher le dialogs, des actions après le dialogue
}
