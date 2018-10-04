﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

// Attach this to an empty GameObject and make sure the object is 
// tagged 'GameController'

public class GameController : MonoBehaviour
{
    // drag both of the objects with attached PlayerControllers onto this
    public List<PlayerController> players;
    // transforms that the camera should copy when on a certain player's turn
    public List<Transform> cameraTransforms;

    public int currentTurn = 0;
    private float turnTimer = 0.0f;

    private Camera mainCamera;
    private UIController uiController;
    private GameObject followingProjectile;
    private Vector3 followingOffset;
    private bool buildPhase = true;

    void Awake()
    {
        // grab the camera so we can control it like moving between positions and stuff
        GameObject camObject = GameObject.FindGameObjectWithTag("MainCamera");
        mainCamera = camObject.GetComponent<Camera>();
        // grab the UI controller for things like showing winner text
        GameObject uiObject = GameObject.FindGameObjectWithTag("UIController");
        uiController = uiObject.GetComponent<UIController>();

        // disable all playercontrollers so the turn management can handle it
        foreach (PlayerController p in players)
        {
            p.onShoot += PlayerShot;
            p.Disable();
        }

        currentTurn = -1;
        StartNextTeam();
    }

    void Start()
    {
    }

    void Update()
    {
        turnTimer -= Time.deltaTime;
        uiController.SetTimer(turnTimer);
        if (turnTimer <= 0.0f)
            StartNextTeam();

        if (followingProjectile != null)
        {
            Vector3 destPosition =
                followingProjectile.transform.position + followingOffset;

            const float smoothAmount = 3.0f;

            mainCamera.transform.position -=
                (mainCamera.transform.position - destPosition) *
                smoothAmount * Time.deltaTime;

            return;
        }
    }

    void StartNextTeam()
    {
        turnTimer = 30.0f;

        // make sure we're not following a projectile
        followingProjectile = null;

        if (currentTurn >= 0 && currentTurn < players.Count)
            players[currentTurn].Disable();

        currentTurn++;
        if (currentTurn >= players.Count)
        {
            currentTurn = 0;
            // if we're wrapping around back to the first player, then build
            // phase is over!
            buildPhase = false;
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
    }

    public bool IsBuildPhase()
    {
        return buildPhase;
    }

}
