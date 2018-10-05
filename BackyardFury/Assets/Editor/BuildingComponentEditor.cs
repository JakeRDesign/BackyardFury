using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BuildingComponent))]
public class BuildingComponentEditor : Editor
{

    public override void OnInspectorGUI()
    {
        BuildingComponent cmp = (BuildingComponent)target;

        EditorGUILayout.Separator();
        EditorGUILayout.LabelField("Building Removal", EditorStyles.boldLabel);

        cmp.removeOnSpringBreak =
            EditorGUILayout.ToggleLeft("Remove On Spring Break",
            cmp.removeOnSpringBreak);

        // explain what the above toggle actually means
        if (cmp.removeOnSpringBreak)
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

        cmp.distanceToRemove =
            EditorGUILayout.FloatField(new GUIContent("Distance To Remove",
            "How far the building needs to move from its placed position " +
            "in order to disappear\nThis is only used if 'Remove On Spring " +
            "Break' is disabled or the building has no springs"),
            cmp.distanceToRemove);

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

        cmp.connectUnder =
            GUILayout.Toggle(cmp.connectUnder, "Under", "Button");
        cmp.connectAdjacent =
            GUILayout.Toggle(cmp.connectAdjacent, "Adjacent", "Button");

        EditorGUILayout.EndHorizontal();

        cmp.connectDistance =
            EditorGUILayout.FloatField(new GUIContent("Spring Attach Dist.",
            "How close another box needs to be to create a spring\n" +
            "Should be at least half of the size of the building!!!"),
            cmp.connectDistance);

        cmp.forceToBreak =
            EditorGUILayout.FloatField(new GUIContent("Spring Break Force",
            "Force required to break springs\n" +
            "Seems to be dependent on mass, so tweaking might be needed"),
            cmp.forceToBreak);

        EditorGUILayout.Separator();
        EditorGUILayout.LabelField("Animation", EditorStyles.boldLabel);

        cmp.fallTime = EditorGUILayout.FloatField("Fall Time", cmp.fallTime);
        cmp.fallHeight = EditorGUILayout.FloatField("Fall Height", 
            cmp.fallHeight);

        cmp.stretchAmount =
            EditorGUILayout.FloatField(new GUIContent("Stretch Amount", 
            "The factor to stretch by at peak stretch levels"),
            cmp.stretchAmount);
        cmp.squashAmount =
            EditorGUILayout.FloatField(new GUIContent("Squash Amount", 
            "The factor to squash by upon landing"),
            cmp.squashAmount);
        cmp.inflateSpeed =
            EditorGUILayout.FloatField(new GUIContent("Inflate Speed", 
            "How fast the box should un-squash back to its original size"),
            cmp.inflateSpeed);
    }

}
