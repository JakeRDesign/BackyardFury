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
    SerializedProperty springTolerance;
    SerializedProperty springMaxDistance;

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
        springTolerance = 
            serializedObject.FindProperty("springTolerance");
        springMaxDistance = 
            serializedObject.FindProperty("springMaxDistance");
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

        springTolerance.floatValue =
            EditorGUILayout.FloatField(new GUIContent("Spring Tolerance",
            "Tolerance of spring joints\n" +
            "higher = more distance allowed between connected objects"),
            springTolerance.floatValue);

        springMaxDistance.floatValue =
            EditorGUILayout.FloatField(new GUIContent("Spring Max Distance",
            "Max distance allowed between connected objects"),
            springMaxDistance.floatValue);

        EditorGUILayout.Separator();
        EditorGUILayout.LabelField("Animation", EditorStyles.boldLabel);

        serializedObject.ApplyModifiedProperties();
    }

}
