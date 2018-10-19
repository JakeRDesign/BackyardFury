using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerModeBase : MonoBehaviour {

    // buncha references to objects
    protected Camera mainCamera;
    protected UIController uiController;
    protected GameController gameController;
    protected PlayerController parentController;

    public virtual void Awake()
    {
        // grab all the references we need
        // gamecontroller
        GameObject controller = GameObject.FindGameObjectWithTag("GameController");
        gameController = controller.GetComponent<GameController>();
        // camera - for raycasting
        GameObject camera = GameObject.FindGameObjectWithTag("MainCamera");
        mainCamera = camera.GetComponent<Camera>();
        // uicontroller
        GameObject uiObject = GameObject.FindGameObjectWithTag("UIController");
        uiController = uiObject.GetComponent<UIController>();
        // the playercontroller on this object
        parentController = GetComponent<PlayerController>();
    }
}
