using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(CookController)),CanEditMultipleObjects]
public class CookControllerEditor : CustomFieldInspector<CookController> {

    private SerializedProperty _begin,_finish,_runSinceMenu,_isTraining,_mainAudio,_inputs,_players,_mainCamera,_actualState;

    private SerializedProperty _startCinematic,_startCinematicCameras,_endCinematic,_endCinematicCameras;

    private SerializedProperty _useTimer,_gameTime,_timer,_endChrono;

    private SerializedProperty _useChrono,_startChrono,_runChrono,_chronoText;

    private SerializedProperty _win, _classementPanels, _confetti;

    private SerializedProperty _sliderPrefab, _slotPrefab, _teams, _recipePrefab, _recipeParent,_instances,_platePrefab;
    
    protected override void OnEnable() {
        base.OnEnable();
        MiniGameInit();

        _sliderPrefab = serializedClass.FindProperty("sliderPrefab");
        _slotPrefab = serializedClass.FindProperty("slotPrefab");
        _teams = serializedClass.FindProperty("teams");
        _recipeParent = serializedClass.FindProperty("recipeParent");
        _recipePrefab = serializedClass.FindProperty("recipePrefab");
        _platePrefab = serializedClass.FindProperty("platePrefab");
        _instances = serializedClass.FindProperty("instances");
    }

    public override void OnInspectorGUI() {
        EditorGUI.BeginChangeCheck();

        EditorGUILayout.Space(10);


        GUIStyle textStyle = GUI.skin.label;
        textStyle.alignment = TextAnchor.MiddleCenter;
        textStyle.fontStyle = FontStyle.Bold;

        MiniGameBody(textStyle);
        
        EditorGUILayout.Space(10);

        EditorGUILayout.PropertyField(_sliderPrefab);
        EditorGUILayout.PropertyField(_slotPrefab);
        EditorGUILayout.PropertyField(_instances);
        EditorGUILayout.PropertyField(_teams);
        EditorGUILayout.PropertyField(_recipeParent);
        EditorGUILayout.PropertyField(_recipePrefab);
        EditorGUILayout.PropertyField(_platePrefab);
        
        if(EditorGUI.EndChangeCheck()) 
            serializedClass.ApplyModifiedProperties();
    }

    private void MiniGameInit() {
        _begin = serializedClass.FindProperty("begin");
        _finish = serializedClass.FindProperty("finish");
        _runSinceMenu = serializedClass.FindProperty("runSinceMenu");
        _isTraining = serializedClass.FindProperty("isTraining");
        _mainAudio = serializedClass.FindProperty("mainAudio");
        _inputs = serializedClass.FindProperty("inputs");
        _players = serializedClass.FindProperty("players");
        _mainCamera = serializedClass.FindProperty("mainCamera");
        _actualState = serializedClass.FindProperty("actualState");
        
        _startCinematic = serializedClass.FindProperty("startCinematic");
        _startCinematicCameras = serializedClass.FindProperty("startCinematicCameras");
        _endCinematic = serializedClass.FindProperty("endCinematic");
        _endCinematicCameras = serializedClass.FindProperty("endCinematicCameras");

        _useTimer = serializedClass.FindProperty("useTimer");
        _gameTime = serializedClass.FindProperty("gameTime");
        _timer = serializedClass.FindProperty("timer");
        _endChrono = serializedClass.FindProperty("endChrono");

        _useChrono = serializedClass.FindProperty("useChrono");
        _startChrono = serializedClass.FindProperty("startChrono");
        _runChrono = serializedClass.FindProperty("runChrono");
        _chronoText = serializedClass.FindProperty("chronoText");

        _win = serializedClass.FindProperty("win");
        _classementPanels = serializedClass.FindProperty("classementPanels");
        _confetti = serializedClass.FindProperty("confetti");
    }
    
    private void MiniGameBody(GUIStyle textStyle) {
        EditorGUILayout.BeginVertical("box");
        EditorGUILayout.TextField("Game State",textStyle);

        EditorGUILayout.PropertyField(_begin);
        EditorGUILayout.PropertyField(_finish);
        EditorGUILayout.PropertyField(_runSinceMenu);
        EditorGUILayout.PropertyField(_isTraining);
        EditorGUILayout.PropertyField(_mainAudio);
        EditorGUILayout.PropertyField(_players);
        EditorGUILayout.PropertyField(_inputs);
        EditorGUILayout.PropertyField(_mainCamera);
        EditorGUILayout.PropertyField(_actualState);
        EditorGUILayout.EndVertical();
        
        EditorGUILayout.Space(10);
        
        
        EditorGUILayout.BeginVertical("box");
        EditorGUILayout.TextField("Cinematics", textStyle);
        EditorGUILayout.PropertyField(_startCinematic);
        EditorGUILayout.PropertyField(_startCinematicCameras);
        EditorGUILayout.PropertyField(_endCinematic);
        EditorGUILayout.PropertyField(_endCinematicCameras);
        EditorGUILayout.EndVertical();
        
        EditorGUILayout.Space(10);
        
        
        EditorGUILayout.BeginVertical("box");
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.TextField("Timer", textStyle);
        EditorGUILayout.PropertyField(_useTimer);
        EditorGUILayout.EndHorizontal();

        if (classTarget.useTimer) {
            EditorGUILayout.PropertyField(_gameTime);
            EditorGUILayout.PropertyField(_timer);
            EditorGUILayout.PropertyField(_endChrono);
        }
        
        EditorGUILayout.EndVertical();


        EditorGUILayout.Space(10);
        
        EditorGUILayout.BeginVertical("box");
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.TextField("Chrono", textStyle);
        EditorGUILayout.PropertyField(_useChrono);
        EditorGUILayout.EndHorizontal();

        if (classTarget.useChrono) {
            EditorGUILayout.PropertyField(_startChrono);
            EditorGUILayout.PropertyField(_runChrono);
            EditorGUILayout.PropertyField(_chronoText);
        }        
        
        EditorGUILayout.EndVertical();


        EditorGUILayout.Space(10);
        
        EditorGUILayout.BeginVertical("box");
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Win",textStyle);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.PropertyField(_win);
        EditorGUILayout.PropertyField(_confetti);
        EditorGUILayout.PropertyField(_classementPanels);
        EditorGUILayout.EndVertical();
    }
}
