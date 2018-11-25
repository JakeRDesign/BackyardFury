using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using XInputDotNetPure;
using XboxCtrlrInput;

// Attach this to an empty object and set up all exposed things in inspector

[System.Serializable]
public enum TurnMode
{
    BUILD,
    SHOOT
}

public class PlayerController : MonoBehaviour
{
    // whether we're shooting or building - default switch key is 
    // right click or alt
    public TurnMode currentMode = TurnMode.BUILD;

    public int playerIndex = -1;

    [Header("Build Settings")]
    public Bounds buildZone;

    [Header("Play Settings")]
    public Bounds playZone;

    // mode components
    [HideInInspector]
    public BuildPlayerMode buildMode;
    [HideInInspector]
    public ShootPlayerMode shootMode;
    [HideInInspector]
    public BaseInput ourInput;

    [Header("Sound Settings")]
    public string soundSet = "Boy";

    private UIController uiController;
    private GameController gameController;

    // shooting delegate/event for GameController to handle the end of the turn
    public delegate void ProjectileShotEvent(GameObject projectile);
    public ProjectileShotEvent onShoot;

    void Awake()
    {
        // grab references to objects
        GameObject controller = GameObject.FindGameObjectWithTag("GameController");
        gameController = controller.GetComponent<GameController>();

        GameObject uiControllerObject = GameObject.FindGameObjectWithTag("UIController");
        uiController = uiControllerObject.GetComponent<UIController>();

        //ourInput = GetComponent<BaseInput>();
        buildMode = GetComponent<BuildPlayerMode>();
        shootMode = GetComponent<ShootPlayerMode>();

        StartBuildMode();
    }

    void Update()
    {
        //if (ourInput.AltPressed())
        //{
        //    // don't allow switching modes if we're in the build phase
        //    if (gameController.IsBuildPhase())
        //        return;

        //    if (currentMode == TurnMode.BUILD)
        //        StartShootMode();
        //    else
        //        StartBuildMode();
        //}
    }

    void StartBuildMode()
    {
        if (!gameController.CanBuildThisTurn())
            return;

        currentMode = TurnMode.BUILD;

        buildMode.EnableMode();
        shootMode.DisableMode();
        uiController.ShowBuildPresets(true);
    }

    void StartShootMode()
    {
        currentMode = TurnMode.SHOOT;

        shootMode.EnableMode();
        buildMode.DisableMode();
        uiController.ShowBuildPresets(false);
    }

    public void Enable()
    {
        this.enabled = true;
        ourInput.SetInputEnabled(true);
        // start the last mode we were in 
        if (!gameController.CanBuildThisTurn())
            StartShootMode();
        else
            StartBuildMode();
    }

    public void Disable()
    {
        uiController.ShowBuildPresets(false);
        if (ourInput != null)
            ourInput.SetInputEnabled(false);
        buildMode.DisableMode();
        shootMode.DisableMode();

        enabled = false;
    }

}
