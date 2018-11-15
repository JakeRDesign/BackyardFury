    using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour {

    private void Start()
    {
        // make cursor free and visible - just undoing what CursorLock.cs 
        // does in game
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
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
    public void SoundSlider ()
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
}
