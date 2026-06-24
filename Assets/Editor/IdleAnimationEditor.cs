using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(IdleAnimation))]
public class IdleAnimationEditor : Editor
{
    private SerializedProperty _enableTilt;
    private SerializedProperty _tiltAngle;
    private SerializedProperty _tiltDuration;
    private SerializedProperty _enableScalePulse;
    private SerializedProperty _scalePulseAmount;
    private SerializedProperty _scalePulseDuration;
    private SerializedProperty _enableSquashStretch;
    private SerializedProperty _squashStretchAmount;
    private SerializedProperty _squashStretchDuration;
    private SerializedProperty _squashStretchPhase;

    private void OnEnable()
    {
        _enableTilt = serializedObject.FindProperty("_enableTilt");
        _tiltAngle = serializedObject.FindProperty("_tiltAngle");
        _tiltDuration = serializedObject.FindProperty("_tiltDuration");
        _enableScalePulse = serializedObject.FindProperty("_enableScalePulse");
        _scalePulseAmount = serializedObject.FindProperty("_scalePulseAmount");
        _scalePulseDuration = serializedObject.FindProperty("_scalePulseDuration");
        _enableSquashStretch = serializedObject.FindProperty("_enableSquashStretch");
        _squashStretchAmount = serializedObject.FindProperty("_squashStretchAmount");
        _squashStretchDuration = serializedObject.FindProperty("_squashStretchDuration");
        _squashStretchPhase = serializedObject.FindProperty("_squashStretchPhase");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.LabelField("Tilt", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(_enableTilt, new GUIContent("Enable Tilt"));
        if (_enableTilt.boolValue)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(_tiltAngle, new GUIContent("Angle (degrees)"));
            EditorGUILayout.PropertyField(_tiltDuration, new GUIContent("Half-swing Duration (s)"));
            EditorGUI.indentLevel--;
        }

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Scale Pulse", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(_enableScalePulse, new GUIContent("Enable Scale Pulse"));
        if (_enableScalePulse.boolValue)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(_scalePulseAmount, new GUIContent("Pulse Scale"));
            EditorGUILayout.PropertyField(_scalePulseDuration, new GUIContent("Half-pulse Duration (s)"));
            EditorGUI.indentLevel--;
        }

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Squash & Stretch", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(_enableSquashStretch, new GUIContent("Enable Squash & Stretch"));
        if (_enableSquashStretch.boolValue)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(_squashStretchAmount, new GUIContent("Amount"));
            EditorGUILayout.PropertyField(_squashStretchDuration, new GUIContent("Half-cycle Duration (s)"));
            EditorGUILayout.PropertyField(_squashStretchPhase, new GUIContent("Phase Delay (s)"));
            EditorGUI.indentLevel--;
        }

        serializedObject.ApplyModifiedProperties();
    }
}
