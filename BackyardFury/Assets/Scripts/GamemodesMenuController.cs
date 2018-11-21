using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GamemodesMenuController : MonoBehaviour {

	void Start () {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void LastBoxMode()
    {
        GlobalSettings.Instance().selectedMode = GameModes.SpecialBoxes;
        SceneManager.LoadScene(4);
    }

    public void BoxingMatchMode()
    {
        GlobalSettings.Instance().selectedMode = GameModes.BoxingMatch;
        SceneManager.LoadScene(4);
    }
}
