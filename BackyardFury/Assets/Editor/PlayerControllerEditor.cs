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
    ShootPlayerMode shootMode;

    void OnEnable()
    {
        controller = (PlayerController)target;
        shootMode = controller.GetComponent<ShootPlayerMode>();
    }

    void OnSceneGUI()
    {
        Handles.zTest = UnityEngine.Rendering.CompareFunction.Less;

        // build zone handle
        Handles.color = Color.cyan;
        controller.buildZone.center = Handles.PositionHandle(controller.buildZone.center, Quaternion.identity);
        Handles.color = Color.cyan;
        Handles.DrawWireCube(controller.buildZone.center, controller.buildZone.extents*2.0f);
        Handles.Label(controller.buildZone.center - Vector3.up*0.5f, "Build Zone");

        controller.buildZone.extents = 
            Handles.ScaleHandle(controller.buildZone.extents, 
            controller.buildZone.center - Vector3.up * 1.3f, 
            Quaternion.identity, 1.0f);

        // play zone handle
        Handles.color = Color.red;
        controller.playZone.center = Handles.PositionHandle(controller.playZone.center, Quaternion.identity);
        Handles.color = Color.red;
        Handles.DrawWireCube(controller.playZone.center, controller.playZone.extents*2.0f);
        Handles.Label(controller.playZone.center - Vector3.up*0.5f, "Play Zone");

        controller.playZone.extents = 
            Handles.ScaleHandle(controller.playZone.extents, 
            controller.playZone.center - Vector3.up * 1.3f, 
            Quaternion.identity, 1.0f);

        // shot destination handle
        if (shootMode == null)
            return;
        shootMode.shotDestination = Handles.PositionHandle(shootMode.shotDestination, Quaternion.identity);
        Handles.Label(shootMode.shotDestination, "Shot Destination Start");
    }
}
