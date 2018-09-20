using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Attach this to an empty GameObject and make sure the object is 
// tagged 'GameController'

public class GameController : MonoBehaviour
{
    // drag both of the objects with attached PlayerControllers onto this
    public List<PlayerController> players;
    public int currentTurn = 0;

    void Awake()
    {
        // disable all playercontrollers so the turn management can handle it
        foreach (PlayerController p in players)
            p.enabled = false;

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
            players[currentTurn].enabled = false;

        currentTurn++;
        if (currentTurn >= players.Count)
            currentTurn = 0;

        players[currentTurn].enabled = true;
    }
}
