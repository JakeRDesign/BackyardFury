using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Attach this to an empty GameObject and make sure the object is 
// tagged 'GameController'

public class GameController : MonoBehaviour
{
    // drag both of the objects with attached PlayerControllers onto this
    public List<PlayerController> players;
    // transforms that the camera should copy when on a certain player's turn
    public List<Transform> cameraTransforms;
    public int currentTurn = 0;
    
    private Camera mainCamera;

    void Awake()
    {
        // grab the camera so we can control it like moving between positions and stuff
        GameObject camObject = GameObject.FindGameObjectWithTag("MainCamera");
        mainCamera = camObject.GetComponent<Camera>();

        // disable all playercontrollers so the turn management can handle it
        foreach (PlayerController p in players)
            p.Disable();

        currentTurn--;
        StartNextTeam();
    }

    void Start()
    {
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            StartNextTeam();
    }

    void StartNextTeam()
    {
        if (currentTurn >= 0 && currentTurn < players.Count)
            players[currentTurn].Disable();

        currentTurn++;
        if (currentTurn >= players.Count)
            currentTurn = 0;

        // move camera
        if (currentTurn >= 0 && currentTurn < cameraTransforms.Count)
        {
            Transform destTransform = cameraTransforms[currentTurn];
            mainCamera.transform.SetPositionAndRotation(
                destTransform.position,
                destTransform.rotation
            );
        }

        players[currentTurn].Enable();
    }
}
