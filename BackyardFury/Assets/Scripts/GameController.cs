using System.Collections;
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
    public int minimumProjectilesOnSide = 2;
    public float initialBuildPhaseLength = 30.0f;
    public float otherBuildTurnLength = 30.0f;
    public float shootTurnLength = 15.0f;
    private float turnTimer = 0.0f;
    [Header("Allow building every X turns:")]
    public int buildInterval = 3;
    public bool noOtherBuildPhases = false;
    private int turnCount = 0;

    [Header("Tetris Stuff")]
    public List<GameObject> tetrisPieces;

    [Header("Sounds")]
    public AudioSource audioSource;
    public AudioClip cantPlaceSound;

    [Header("(TEST) Decreasing Build Time")]
    public bool decreaseEachBuildTime = false;
    [Tooltip("How many seconds each build phase should decrease by")]
    public float decreaseBy = 5.0f;
    [Tooltip("Set this to 0 to stop having build phases once the decreased time reaches 0")]
    public float minimumBuildTime = 5.0f;

    [Header("(TEST) Defend Special Boxes")]
    public bool defendingBoxes = false;
    public int boxesToDefend = 1;

    private Camera mainCamera;
    private UIController uiController;
    private GameObject followingProjectile;
    private List<GameObject> activeProjectiles = new List<GameObject>();
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

        if (noOtherBuildPhases)
            buildInterval = 9999;

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

    void Start() { }

    void Update()
    {
        if (uiController.IsInPauseMenu())
            return;

        if (IsStartDown())
            uiController.OpenPauseMenu();

        if (currentTurn < 0 || gameOver)
            return;

        GamePadState state = GamePad.GetState((PlayerIndex)GetCurrentPlayer().playerIndex);

        if (Input.GetKeyDown(KeyCode.Tab) || state.Buttons.Back == ButtonState.Pressed)
            uiController.ShowHelpMenu();

        if (GetCurrentPlayer().enabled)
            turnTimer -= Time.deltaTime;
        // debug key to add time to a turn
        if (Input.GetKeyDown(KeyCode.O))
            turnTimer += 10.0f;

        uiController.SetTimer(turnTimer);

        // TODO: make a proper binding for skipping turn
        if (turnTimer <= 0.0f || (Input.GetKeyDown(KeyCode.P) || state.Buttons.Y == ButtonState.Pressed))
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
        placer.PlaceProjectiles();
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

        if (GetCurrentPlayer() != null && GetCurrentPlayer().shootMode.CanShoot())
        {
            // set turn timer a little over 0 so we're not continuously calling
            // this function
            turnTimer = 0.1f;
            // and shoot!
            GetCurrentPlayer().shootMode.Shoot();
            return;
        }

        if (GetCurrentPlayer() != null && GetCurrentPlayer().buildMode.GetBuildingCount() == 0)
        {
            PlayerLost(null);
            return;
        }

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
        if (turnCount <= 0)
        {
            turnTimer = initialBuildPhaseLength;
        }
        else if (CanBuildThisTurn())
        {
            turnTimer = otherBuildTurnLength;

            if (decreaseEachBuildTime)
            {
                // only want to decrease the build time before player 1's turn
                // (so decrease it when the current player is player 2, since
                //  this is happening after we set the turn timer)
                if (currentTurn == 1)
                    otherBuildTurnLength -= decreaseBy;

                if (otherBuildTurnLength < minimumBuildTime)
                    otherBuildTurnLength = minimumBuildTime;

                // when/if the turn length reaches 0 (meaning no more build phases),
                // set the interval to something huge so the UI knows not to
                // show the "next build phase in X turns"
                if (otherBuildTurnLength <= 0.0f)
                    buildInterval = 9999;
            }
        }
        else
        {
            turnTimer = shootTurnLength;
        }

        CheckProjectileCount();

        // reset preset used flag
        GetCurrentPlayer().buildMode.hasUsedPreset = false;

        uiController.SetPresetPosition(currentTurn < 1);
        uiController.UpdateNextBuildTurn(turnCount, buildInterval);
    }

    void CheckProjectileCount()
    {
        PlayerController plr = GetCurrentPlayer();

        // loop through all projectiles and check if they're inside the zone
        int insideZone = 0;
        foreach (GameObject p in activeProjectiles)
        {
            if (p == null)
                continue;
            if (plr.playZone.Contains(p.transform.position))
                insideZone++;
            if (insideZone >= minimumProjectilesOnSide)
                return;
        }

        // spawn new projectiles if there's not enough
        ObstaclePlacer placer = GetComponent<ObstaclePlacer>();
        for (int i = 0; i < (minimumProjectilesOnSide - insideZone); ++i)
            placer.GiveTeamProjectile(currentTurn);
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
            component.onLand = ProjectileLanded;
    }

    public void StopFollowingProjectile()
    {
        followingProjectile = null;
    }

    void ProjectileLanded()
    {
        StartCoroutine(WaitToStartNextTeam(2.0f));
    }

    bool waitingForNextTeam = false;
    IEnumerator WaitToStartNextTeam(float seconds)
    {
        if (waitingForNextTeam)
            yield break;
        waitingForNextTeam = true;
        yield return new WaitForSeconds(seconds);
        StartNextTeam();
        waitingForNextTeam = false;
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

        if (loser == null)
            uiController.ShowWinnerText(-1);
        else
            uiController.ShowWinnerText(index + 1);

        // disable all player controllers :)
        foreach (PlayerController p in players)
            p.Disable();
        gameOver = true;
    }

    public void ProjectileAdded(GameObject newProjectile)
    {
        activeProjectiles.Add(newProjectile);
    }

    public bool IsBuildPhase()
    {
        return turnCount <= 0;
    }

    public bool CanBuildThisTurn()
    {
        // once build time gets to 0, we never want to build again
        if (otherBuildTurnLength <= 0.0f)
            return false;

        return (turnCount % buildInterval) == 0;
    }

    public static bool IsStartDown()
    {
        for (int i = 0; i < 4; ++i)
            if (GamePad.GetState((PlayerIndex)i).Buttons.Start == ButtonState.Pressed)
                return true;
        if (Input.GetKey(KeyCode.Escape))
            return true;
        return false;
    }
    public bool IsPaused()
    {
        return uiController.IsInPauseMenu();
    }

}
