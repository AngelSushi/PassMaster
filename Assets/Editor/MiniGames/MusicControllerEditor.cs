using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Rendering;
using UnityEngine;

[CustomEditor(typeof(MusicController)),CanEditMultipleObjects]
public class MusicControllerEditor : CustomFieldInspector<MusicController> {
    private SerializedProperty _begin,_finish,_runSinceMenu,_isTraining,_mainAudio,_inputs,_players,_mainCamera,_actualState;

    private SerializedProperty _startCinematic,_startCinematicCameras,_endCinematic,_endCinematicCameras;

    private SerializedProperty _useTimer,_gameTime,_timer,_endChrono;

    private SerializedProperty _useChrono,_startChrono,_runChrono,_chronoText;

    private SerializedProperty _win, _classementPanels, _confetti;

    private SerializedProperty _songDelay;

    private SerializedProperty _notePrefab,_longNotePrefab,_noteTime, _noteSpawnY, _noteTapY,_patterns;

    private SerializedProperty _detection,_goodMarginError, _goodReward, _perfectMarginError, _perfectReward,_lanes;

    private SerializedProperty _canvas,_start,_playersUI,_noteState,_uiKeyParent,_classementParent,_circleTransition;
    
    protected override void OnEnable() {
        base.OnEnable();

        MiniGameInit();

        _songDelay = serializedClass.FindProperty("songDelay");

        _notePrefab = serializedClass.FindProperty("notePrefab");
        _longNotePrefab = serializedClass.FindProperty("longNotePrefab");
        _noteTime = serializedClass.FindProperty("noteTime");
        _noteSpawnY = serializedClass.FindProperty("noteSpawnY");
        _noteTapY = serializedClass.FindProperty("noteTapY");
        _patterns = serializedClass.FindProperty("patternsRef");

        _detection = serializedClass.FindProperty("detection");
        _goodMarginError = serializedClass.FindProperty("goodMarginError");
        _goodReward = serializedClass.FindProperty("goodReward");
        _perfectMarginError = serializedClass.FindProperty("perfectMarginError");
        _perfectReward = serializedClass.FindProperty("perfectReward");

        _canvas = serializedClass.FindProperty("canvas");
        _start = serializedClass.FindProperty("start");
        _lanes = serializedClass.FindProperty("lanes");
        _playersUI = serializedClass.FindProperty("playersUI");
        _noteState = serializedClass.FindProperty("noteStatePrefab");
        _uiKeyParent = serializedClass.FindProperty("uiKeyParent");
        _classementParent = serializedClass.FindProperty("classementParent");
        _circleTransition = serializedClass.FindProperty("circleTransition");
    }

    public override void OnInspectorGUI() {
        EditorGUI.BeginChangeCheck();
        
        EditorGUILayout.Space(10);


        GUIStyle textStyle = GUI.skin.label;
        textStyle.alignment = TextAnchor.MiddleCenter;
        textStyle.fontStyle = FontStyle.Bold;
        
        MiniGameBody(textStyle);

        EditorGUILayout.Space(10);
        
        EditorGUILayout.BeginVertical("box");
        EditorGUILayout.TextField("Note", textStyle);
        EditorGUILayout.PropertyField(_notePrefab);
        EditorGUILayout.PropertyField(_longNotePrefab);
        EditorGUILayout.PropertyField(_noteTime);
        EditorGUILayout.PropertyField(_noteSpawnY);
        EditorGUILayout.PropertyField(_noteTapY);
        EditorGUILayout.PropertyField(_patterns);
        EditorGUILayout.EndVertical();

        EditorGUILayout.Space(10);
        
        EditorGUILayout.BeginVertical("box");
        EditorGUILayout.TextField("Detection", textStyle);
        EditorGUILayout.PropertyField(_detection);
        EditorGUILayout.PropertyField(_goodMarginError);
        EditorGUILayout.PropertyField(_goodReward);
        EditorGUILayout.PropertyField(_perfectMarginError);
        EditorGUILayout.PropertyField(_perfectReward);
        EditorGUILayout.PropertyField(_lanes);
        EditorGUILayout.EndVertical();
        
        
        EditorGUILayout.Space(10);
        
        EditorGUILayout.BeginVertical("box");
        EditorGUILayout.TextField("UI", textStyle);
        EditorGUILayout.PropertyField(_canvas);
        EditorGUILayout.PropertyField(_start);
        EditorGUILayout.PropertyField(_playersUI);
        EditorGUILayout.PropertyField(_noteState);
        EditorGUILayout.PropertyField(_uiKeyParent);
        EditorGUILayout.PropertyField(_classementParent);
        EditorGUILayout.PropertyField(_circleTransition);
        EditorGUILayout.EndVertical();
        

        if(EditorGUI.EndChangeCheck()) 
            serializedClass.ApplyModifiedProperties();
        
    }

 /*   public override bool RequiresConstantRepaint() {
        return true;
    }
*/
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
        EditorGUILayout.PropertyField(_songDelay);
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
