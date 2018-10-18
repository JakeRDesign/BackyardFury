﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using XInputDotNetPure;

// Attach this to an empty GameObject and make sure the object is 
// tagged 'GameController'

public class GameController : MonoBehaviour
{
    // drag both of the objects with attached PlayerControllers onto this
    public List<PlayerController> players;
    // transforms that the camera should copy when on a certain player's turn
    public List<Transform> cameraTransforms;

    public float cameraFollowStrength = 10.0f;

    public int currentTurn = 0;
    public float buildPhaseLength = 30.0f;
    public float turnLength = 30.0f;
    private float turnTimer = 0.0f;
    [Header("Allow building every X turns:")]
    public int buildInterval = 3;
    private int turnCount = 0;

    private Camera mainCamera;
    private UIController uiController;
    private GameObject followingProjectile;
    private Vector3 followingOffset;

    private bool gameOver = false;

    void Awake()
    {
        // grab the camera so we can control it like moving between positions and stuff
        GameObject camObject = GameObject.FindGameObjectWithTag("MainCamera");
        mainCamera = camObject.GetComponent<Camera>();
        // grab the UI controller for things like showing winner text
        GameObject uiObject = GameObject.FindGameObjectWithTag("UIController");
        uiController = uiObject.GetComponent<UIController>();

        // disable all playercontrollers so the turn management can handle it
        int butts = 0;
        foreach (PlayerController p in players)
        {
            p.onShoot += PlayerShot;
            p.Disable();
            p.playerIndex = butts;
            butts++;
        }

        currentTurn = -1;
        StartCoroutine(PlaceObstacles());
    }

    void Start()
    {
    }


    ButtonState previousYState = ButtonState.Released;
    void Update()
    {
       

        if (currentTurn < 0 || gameOver)
            return;

        GamePadState state = GamePad.GetState((PlayerIndex)GetCurrentPlayer().playerIndex);

        turnTimer -= Time.deltaTime;
        // debug key to add time to a turn
        if (Input.GetKeyDown(KeyCode.O))
            turnTimer += 10.0f;

        uiController.SetTimer(turnTimer);

        // TODO: make a proper binding for skipping turn
        if (turnTimer <= 0.0f || (Input.GetKeyDown(KeyCode.P) || state.Buttons.Y == ButtonState.Pressed))
            StartNextTeam();

        previousYState = state.Buttons.Y;

        if (followingProjectile != null)
        {
            Vector3 destPosition =
                followingProjectile.transform.position + followingOffset;

            mainCamera.transform.position -=
                (mainCamera.transform.position - destPosition) *
                cameraFollowStrength * Time.deltaTime;

            return;
        }
    }

    IEnumerator PlaceObstacles()
    {
        currentTurn = -1;
        ObstaclePlacer placer = GetComponent<ObstaclePlacer>();
        for (int i = 0; i < players.Count; ++i)
        {
            // move camera to see obstacles being dropped
            Transform destTransform = cameraTransforms[i];
            mainCamera.transform.SetPositionAndRotation(
                destTransform.position,
                destTransform.rotation
            );

            PlayerController p = players[i];
            placer.isPlacing = true;
            StartCoroutine(placer.PlaceObstacles(p.buildZone));
            while (placer.isPlacing)
                yield return new WaitForSeconds(0.1f);

            yield return new WaitForSeconds(1.0f);
        }

        currentTurn = -1;
        StartNextTeam();
    }

    void StartNextTeam()
    {
        if (gameOver)
            return;

        // make sure we're not following a projectile
        followingProjectile = null;

        if (currentTurn >= 0 && currentTurn < players.Count)
            players[currentTurn].Disable();

        currentTurn++;
        if (currentTurn >= players.Count)
        {
            currentTurn = 0;
            turnCount++;
            uiController.BuildPhaseOver();
        }

        // move camera
        if (currentTurn >= 0 && currentTurn < cameraTransforms.Count)
        {
            Transform destTransform = cameraTransforms[currentTurn];
            mainCamera.transform.SetPositionAndRotation(
                destTransform.position,
                destTransform.rotation
            );
        }

        if (currentTurn >= 0 && currentTurn < players.Count)
            players[currentTurn].Enable();

        // set turn timer AFTER changing turns so we know if it's build phase
        turnTimer = IsBuildPhase() ? buildPhaseLength : turnLength;

        uiController.UpdateNextBuildTurn(turnCount, buildInterval);
    }

    void PlayerShot(GameObject projectile)
    {
        // disable the current player so they can't shoot anymore
        GetCurrentPlayer().Disable();
        // start following the shot projectile
        followingProjectile = projectile;
        // get the offset that the camera should be from the projectile
        // while following
        followingOffset = mainCamera.transform.position -
            projectile.transform.position;

        // hook up projectile's onLand event to ProjectileLanded
        Projectile component = projectile.GetComponent<Projectile>();
        if (component)
            component.onLand += ProjectileLanded;
    }

    void ProjectileLanded()
    {
        StartCoroutine(WaitToStartNextTeam(2.0f));
    }

    IEnumerator WaitToStartNextTeam(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        StartNextTeam();
    }

    public PlayerController GetCurrentPlayer()
    {
        if (currentTurn >= 0 && currentTurn < players.Count)
            return players[currentTurn];
        return null;
    }

    public void PlayerLost(PlayerController loser)
    {
        // grab index of winning player
        int index = -1;
        for (int i = 0; i < players.Count; ++i)
        {
            if (players[i] != loser)
            {
                index = i;
                break;
            }
        }

        Assert.AreNotEqual(-1, index, "Winning player couldn't be found in players list!!");
        uiController.ShowWinnerText(index + 1);

        // disable all player controllers :)
        foreach (PlayerController p in players)
            p.Disable();
        gameOver = true;
    }

    public bool IsBuildPhase()
    {
        return turnCount <= 0;
    }

    public bool CanBuildThisTurn()
    {
        return (turnCount % buildInterval) == 0;
    }

}
