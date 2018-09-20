using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

// This should work as long as it's in the Assets/Editor folder
// just shows the build zone in the scene view

[CustomEditor(typeof(PlayerController))]
public class PlayerControllerEditor : Editor
{

    PlayerController controller;

    void OnEnable()
    {
        controller = (PlayerController)target;
    }

    void OnSceneGUI()
    {
        Handles.zTest = UnityEngine.Rendering.CompareFunction.Less;
        Handles.color = Color.cyan;
        controller.zoneCenter =
            Handles.PositionHandle(controller.zoneCenter, Quaternion.identity);
        Handles.color = Color.cyan;
        Handles.DrawWireCube(controller.zoneCenter, controller.zoneSize);
        Handles.Label(controller.zoneCenter - Vector3.up*0.5f, "Build Zone");

        controller.zoneSize = 
            Handles.ScaleHandle(controller.zoneSize, 
            controller.zoneCenter - Vector3.up * 1.3f, 
            Quaternion.identity, 1.0f);
    }
}
