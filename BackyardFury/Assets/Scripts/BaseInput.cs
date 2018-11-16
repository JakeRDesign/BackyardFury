using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Base class for input
 * Will be inherited from for Controller and PC controls
 */

public abstract class BaseInput : MonoBehaviour {

    protected UIController uiController;

    private void Awake()
    {
        GameObject uiControllerObject = GameObject.FindGameObjectWithTag("UIController");
        uiController = uiControllerObject.GetComponent<UIController>();
    }

    public abstract bool AltPressed();
    public abstract bool FirePressed();
    public abstract bool FireHeld();
    public abstract bool HelpPressed();
    public abstract bool PausePressed();
    public abstract float VerticalAxis();
    public abstract float HorizontalAxis();

    public void SetInputEnabled(bool b)
    {
        enabled = b;
    }

}
