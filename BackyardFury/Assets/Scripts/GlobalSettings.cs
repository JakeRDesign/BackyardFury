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
    Controller4,

    Count
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

        // if it's still null, create a new instance
        if (instance == null)
        {
            Debug.Log("Making new settings manager");
            GameObject newObj = new GameObject("SettingsManager");
            instance = newObj.AddComponent<GlobalSettings>();

            // set default controls based on connected controllers
            List<ControlTypes> connectedControls = new List<ControlTypes>();
            for (int i = 0; i <= (int)PlayerIndex.Four; ++i)
            {
                PlayerIndex index = (PlayerIndex)i;
                if (GamePad.GetState(index).IsConnected)
                    connectedControls.Add((ControlTypes)(i + 1));
            }

            // no controllers connected - both players use keyboard
            if (connectedControls.Count == 0)
            {
                instance.player1Control = ControlTypes.KeyboardMouse;
                instance.player2Control = ControlTypes.KeyboardMouse;
            }
            // one controller connected - pass it back and forth
            else if (connectedControls.Count == 1)
            {
                instance.player1Control = connectedControls[0];
                instance.player2Control = connectedControls[0];
            }
            // more than 1 connected - one controller each
            else
            {
                instance.player1Control = connectedControls[0];
                instance.player2Control = connectedControls[1];
            }
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

