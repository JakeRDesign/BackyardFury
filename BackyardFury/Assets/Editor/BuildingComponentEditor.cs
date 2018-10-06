using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BuildingComponent))]
public class BuildingComponentEditor : Editor
{

    // serialized properties corresponding to variables
    // these allow the settings to be saved in the prefab
    SerializedProperty removeOnSpringBreak;
    SerializedProperty distanceToRemove;

    SerializedProperty connectUnder;
    SerializedProperty connectAdjacent;
    SerializedProperty connectDistance;
    SerializedProperty forceToBreak;

    SerializedProperty fallTime;
    SerializedProperty fallHeight;
    SerializedProperty stretchAmount;
    SerializedProperty squashAmount;
    SerializedProperty inflateSpeed;

    void OnEnable()
    {
        removeOnSpringBreak = 
            serializedObject.FindProperty("removeOnSpringBreak");
        distanceToRemove = 
            serializedObject.FindProperty("distanceToRemove");

        connectUnder = 
            serializedObject.FindProperty("connectUnder");
        connectAdjacent = 
            serializedObject.FindProperty("connectAdjacent");
        connectDistance = 
            serializedObject.FindProperty("connectDistance");
        forceToBreak = 
            serializedObject.FindProperty("forceToBreak");

        fallTime = 
            serializedObject.FindProperty("fallTime");
        fallHeight = 
            serializedObject.FindProperty("fallHeight");
        stretchAmount = 
            serializedObject.FindProperty("stretchAmount");
        squashAmount = 
            serializedObject.FindProperty("squashAmount");
        inflateSpeed = 
            serializedObject.FindProperty("inflateSpeed");

        Debug.Log("enabled!");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        BuildingComponent cmp = (BuildingComponent)target;

        EditorGUILayout.Separator();
        EditorGUILayout.LabelField("Building Removal", EditorStyles.boldLabel);

        removeOnSpringBreak.boolValue =
            EditorGUILayout.ToggleLeft("Remove On Spring Break",
            removeOnSpringBreak.boolValue);

        // explain what the above toggle actually means
        if (removeOnSpringBreak.boolValue)
        {
            EditorGUILayout.HelpBox(
                "Buildings will disappear once all connected springs are " +
                "broken\nIf no springs were created (placed by itself " +
                "on the ground) then it falls back to disappearing once " +
                "they have moved a distance", MessageType.Info);
        }
        else
        {
            EditorGUILayout.HelpBox(
                "Buildings will disappear if they've moved far enough away",
                MessageType.Info);
        }

        distanceToRemove.floatValue =
            EditorGUILayout.FloatField(new GUIContent("Distance To Remove",
            "How far the building needs to move from its placed position " +
            "in order to disappear\nThis is only used if 'Remove On Spring " +
            "Break' is disabled or the building has no springs"),
            distanceToRemove.floatValue);

        EditorGUILayout.Separator();

        EditorGUILayout.LabelField("Spring Connections (very broken)", 
            EditorStyles.boldLabel);

        EditorGUILayout.LabelField(
            new GUIContent("Directions To Attach Springs",
            "The directions that buildings will be attached\n" +
            "'Under' attaches to the building underneath\n" +
            "'Adjacent' attaches to buildings next to it horizontally"),
            EditorStyles.centeredGreyMiniLabel);

        EditorGUILayout.BeginHorizontal();

        connectUnder.boolValue =
            GUILayout.Toggle(connectUnder.boolValue, "Under", "Button");
        connectAdjacent.boolValue =
            GUILayout.Toggle(connectAdjacent.boolValue, "Adjacent", "Button");

        EditorGUILayout.EndHorizontal();

        connectDistance.floatValue =
            EditorGUILayout.FloatField(new GUIContent("Spring Attach Dist.",
            "How close another box needs to be to create a spring\n" +
            "Should be at least half of the size of the building!!!"),
            connectDistance.floatValue);

        forceToBreak.floatValue =
            EditorGUILayout.FloatField(new GUIContent("Spring Break Force",
            "Force required to break springs\n" +
            "Seems to be dependent on mass, so tweaking might be needed"),
            forceToBreak.floatValue);

        EditorGUILayout.Separator();
        EditorGUILayout.LabelField("Animation", EditorStyles.boldLabel);

        fallTime.floatValue = EditorGUILayout.FloatField("Fall Time", 
            fallTime.floatValue);
        fallHeight.floatValue = EditorGUILayout.FloatField("Fall Height", 
            fallHeight.floatValue);

        stretchAmount.floatValue =
            EditorGUILayout.FloatField(new GUIContent("Stretch Amount", 
            "The factor to stretch by at peak stretch levels"),
            stretchAmount.floatValue);
        squashAmount.floatValue =
            EditorGUILayout.FloatField(new GUIContent("Squash Amount", 
            "The factor to squash by upon landing"),
            squashAmount.floatValue);
        inflateSpeed.floatValue =
            EditorGUILayout.FloatField(new GUIContent("Inflate Speed", 
            "How fast the box should un-squash back to its original size"),
            inflateSpeed.floatValue);

        serializedObject.ApplyModifiedProperties();
    }

}
