using System.Collections;
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

    public float cameraFollowStrength = 10.0f;

    public int currentTurn = 0;
    public float turnLength = 30.0f;
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
    }

    void Start()
    {
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.J))
            StartCoroutine(PlaceObstacles());

        if (currentTurn < 0)
            return;

        turnTimer -= Time.deltaTime;
        // debug key to add time to a turn
        if (Input.GetKeyDown(KeyCode.O))
            turnTimer += 10.0f;

        uiController.SetTimer(turnTimer);

        // TODO: make a proper binding for skipping turn
        if (turnTimer <= 0.0f || Input.GetKeyDown(KeyCode.P))
            StartNextTeam();

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
        turnTimer = turnLength;

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
