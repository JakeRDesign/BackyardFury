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
        SceneManager.LoadScene(4);
    }

    public void BoxingMatchMode()
    {
        SceneManager.LoadScene(5);
    }
}
