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

    [Header("Build Settings")]
    public Bounds buildZone;

    // mode components
    BuildPlayerMode buildMode;
    ShootPlayerMode shootMode;

    private GameController gameController;

    // shooting delegate/event for GameController to handle the end of the turn
    public delegate void ProjectileShotEvent(GameObject projectile);
    public ProjectileShotEvent onShoot;

    void Awake()
    {

        // grab references to objects
        GameObject controller = GameObject.FindGameObjectWithTag("GameController");
        gameController = controller.GetComponent<GameController>();

        buildMode = GetComponent<BuildPlayerMode>();
        shootMode = GetComponent<ShootPlayerMode>();

        StartBuildMode();
        Disable();
    }

    void Update()
    {
        if (Input.GetButtonDown("Fire2"))
        {
            // don't allow switching modes if we're in the build phase
            if (gameController.IsBuildPhase())
                return;

            if (currentMode == TurnMode.BUILD)
                StartShootMode();
            else
                StartBuildMode();
        }
    }

    void StartBuildMode()
    {
        currentMode = TurnMode.BUILD;

        buildMode.EnableMode();
        shootMode.DisableMode();
    }

    void StartShootMode()
    {
        currentMode = TurnMode.SHOOT;

        shootMode.EnableMode();
        buildMode.DisableMode();
    }

    public void Enable()
    {
        this.enabled = true;
        // start the last mode we were in 
        if (currentMode == TurnMode.SHOOT)
            StartShootMode();
        else
            StartBuildMode();
    }

    public void Disable()
    {
        this.enabled = false;
        buildMode.DisableMode();
        shootMode.DisableMode();
    }

}
