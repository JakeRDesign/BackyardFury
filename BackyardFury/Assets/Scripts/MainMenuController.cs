using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using XInputDotNetPure;

public class MainMenuController : MonoBehaviour
{

    public Text player1Text;
    public Text player2Text;


    [Header ("Input Devices")]
    public GameObject P1ControllerIcon;
    public GameObject P1KeyboardIcon;
    public GameObject P2ControllerIcon;
    public GameObject P2KeyboardIcon;

    private void Start()
    {
        // make sure settings show what the current controls are
        UpdateControlTexts();
    }

    private void Update()
    {
        InputDisplay();
    }

    // Loads game
    public void PlayGame()
    {
        SceneManager.LoadScene(1);
    }

    // Loads Settings
    public void Settings()
    {
        SceneManager.LoadScene(2);
    }

    // Volume control Slider
    public Slider Volume;
    public AudioSource Sound;
    public void SoundSlider()
    {
        Sound.volume = Volume.value;
    }

    // Load the Credits
    public void Credits()
    {
        SceneManager.LoadScene(3);
    }

    // Adding mute
    public void Mute()
    {
        AudioListener.pause = !AudioListener.pause;
    }

    // Back to main menu
    public void Back()
    {
        SceneManager.LoadScene(0);
    }

    // Quits Game
    public void QuitGame()
    {
        Application.Quit();
    }

    // Control togglies

    public void TogglePlayer1Controls()
    {
        GlobalSettings settings = GlobalSettings.Instance();
        TogglePlayerControl(ref settings.player1Control);
        UpdateControlTexts();
    }

    public void TogglePlayer2Controls()
    {
        GlobalSettings settings = GlobalSettings.Instance();
        TogglePlayerControl(ref settings.player2Control);
        UpdateControlTexts();
    }

    void TogglePlayerControl(ref ControlTypes butts)
    {
        ControlTypes newType = butts + 1;
        if (newType >= ControlTypes.Count || !IsControllerConnected(newType))
            newType = 0;

        butts = newType;
    }

    bool IsControllerConnected(ControlTypes type)
    {
        PlayerIndex controllerIndex = PlayerIndex.One;
        switch (type)
        {
            case ControlTypes.Controller1:
                controllerIndex = PlayerIndex.One;
                break;
            case ControlTypes.Controller2:
                controllerIndex = PlayerIndex.Two;
                break;
            case ControlTypes.Controller3:
                controllerIndex = PlayerIndex.Three;
                break;
            case ControlTypes.Controller4:
                controllerIndex = PlayerIndex.Four;
                break;
            default:
                return true;
        }

        return GamePad.GetState(controllerIndex).IsConnected;
    }

    void UpdateControlTexts()
    {
        GlobalSettings settings = GlobalSettings.Instance();

        if (player1Text != null)
        {
            player1Text.text = settings.player1Control.ToString();
            player2Text.text = settings.player2Control.ToString();
        }
    }

    #region User Input
    public void InputDisplay()
    {
        GlobalSettings settings = GlobalSettings.Instance();

        if (P1ControllerIcon == null)
            return;
        if (P1KeyboardIcon == null)
            return;
        if (P2ControllerIcon == null)
            return;
        if (P2KeyboardIcon == null)
            return;


        // Checkin Player 1 Inputs Devices
        bool isPlayer1Keyboard = settings.player1Control == ControlTypes.KeyboardMouse;
        if(isPlayer1Keyboard)
        {
            P1ControllerIcon.SetActive(false);
            P1KeyboardIcon.SetActive(true);
        }
        else
        {
            P1KeyboardIcon.SetActive(false);
            P1ControllerIcon.SetActive(true);
        }

        // Checkin Player 2 Inputs Devices
        bool isPlayer2Keyboard = settings.player2Control == ControlTypes.KeyboardMouse;
        if (isPlayer2Keyboard)
        {
            P2ControllerIcon.SetActive(false);
            P2KeyboardIcon.SetActive(true);
        }
        else
        {
            P2KeyboardIcon.SetActive(false);
            P2ControllerIcon.SetActive(true);
        }
    }

    #endregion

}
