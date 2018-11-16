using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XInputDotNetPure;

/*
 * This object should be created by the main menu, and will stick around
 * for the duration of the game
 */

// enum for all valid control types
[System.Serializable]
public enum ControlTypes
{
    KeyboardMouse,
    Controller1,
    Controller2,
    Controller3,
    Controller4
}

// enum for game modes
[System.Serializable]
public enum GameModes
{
    BoxingMatch,
    SpecialBoxes
}

public class GlobalSettings : MonoBehaviour
{

    static GlobalSettings instance = null;

    public ControlTypes player1Control = ControlTypes.Controller1;
    public ControlTypes player2Control = ControlTypes.Controller2;
    public GameModes selectedMode = GameModes.BoxingMatch;

    public static GlobalSettings Instance()
    {
        if (instance == null)
        {
            instance = FindObjectOfType(typeof(GlobalSettings)) as GlobalSettings;
        }

        // If it is still null, create a new instance
        if (instance == null)
        {
            Debug.Log("Making new settings manager");
            GameObject newObj = new GameObject("SettingsManager");
            instance = newObj.AddComponent<GlobalSettings>();
        }

        DontDestroyOnLoad(instance.gameObject);

        return instance;
    }

    private void Awake()
    {
        // make sure there's only 1 settings
        var realInstance = Instance();
        if (realInstance.gameObject != gameObject)
            Destroy(gameObject);
    }

    private void OnApplicationQuit()
    {
        instance = null;
    }

}

