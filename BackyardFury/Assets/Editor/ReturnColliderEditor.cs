using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

[CustomEditor(typeof(ProjectileReturner))]
public class ReturnColliderEditor : Editor
{

    ProjectileReturner returner;
    BoxCollider collider;

    void OnEnable()
    {
        returner = (ProjectileReturner)target;
        collider = returner.GetComponent<BoxCollider>();
    }

    void OnSceneGUI()
    {
        Handles.DrawLine(collider.bounds.center, collider.bounds.center + (returner.returnDirection.normalized * 5.0f));
    }
}
