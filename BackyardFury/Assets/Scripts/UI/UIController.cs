using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using XInputDotNetPure;

public class UIController : MonoBehaviour
{
    public Text timerText;
    public Text winnerText;
    public Text buildBaseText;
    public Text nextBuildText;

    public RectTransform meterTransform;
    public RectTransform buildPresetsGroup;

    [Header("Game Overs")]
    public GameObject gameOver;
    public GameObject player1Won;
    public GameObject player2Won;

    [Header("Pause Menu")]
    public GameObject pauseGroup;
    public RectTransform pauseWindow;
    public Image pauseBackground;
    [Tooltip("How long it takes for the pause menu to scroll in/out of view\n" + 
        "Should probably change this to using an actual Animation")]
    public float scrollTime = 0.3f;
    private bool windowLeaving = false;
    private bool wasMouseVisible = false;
    private bool wasBuildBaseVisible = false;

    [Header("Controller Cursor")]
    [Tooltip("Sensitivity for the controller's movement of the cursor")]
    public float cursorSensitivity = 100.0f;
    private Vector3 lastMousePosition;

    [Header("Preset Menu")]
    [Tooltip("Percentage of the screen that the menu will be from its side!\n" + 
        "e.g. 0.2 means the buttons will be 20% of the screen width away from the side of the screen")]
    public float distanceFromSide = 0.2f;
    public RectTransform tetrisContainer;
    public List<GameObject> tetrisIcons;

    [Header("Help Menu")]
    public GameObject helpGroup;

    [Header("COOLCRATES")]
    [TextArea]
    public string coolCrateString = "u have {0} coolcrates left";
    public GameObject coolCrateText;

    [Header("Particles")]
    public GameObject blueConfetti;
    public GameObject redConfetti;

    [Header("Audio")]
    public AudioMixer mixer;

    private RectTransform cursorImage;
    private GameController gameController;
    private BaseInput pauseOwner;
    private float closeHelpTime = 0.0f;

    private void Awake()
    {
        // start by showing build base text
        buildBaseText.gameObject.SetActive(true);

        GameObject controllerObject = GameObject.FindGameObjectWithTag("GameController");
        gameController = controllerObject.GetComponent<GameController>();

        GameObject cursor = GameObject.FindGameObjectWithTag("UICursor");
        cursorImage = cursor.GetComponent<RectTransform>();
    }

    private void Update()
    {
        if (pauseOwner != null && pauseOwner.PausePressed() && !windowLeaving && pauseGroup.activeSelf)
            ClosePauseMenu();

        if (pauseOwner != null && pauseOwner.HelpPressed() && helpGroup.activeSelf)
            HideHelpMenu();
    }

    #region Displayed Cursor

    void UpdateCursorPosition()
    {
        if (gameController.GetCurrentPlayer() == null)
            return;

        GamePadState state = GamePad.GetState((PlayerIndex)gameController.GetCurrentPlayer().playerIndex);
        Vector3 cursorPos = cursorImage.position;

        if (Vector3.Distance(Input.mousePosition, lastMousePosition) > 1.0f)
        {
            cursorPos = Input.mousePosition;
            lastMousePosition = Input.mousePosition;
        }

        cursorPos.x += state.ThumbSticks.Left.X * Time.deltaTime * cursorSensitivity;
        cursorPos.y += state.ThumbSticks.Left.Y * Time.deltaTime * cursorSensitivity;
        cursorImage.position = cursorPos;
    }

    public Vector3 GetCursorPos()
    {
        return cursorImage.position;
    }

    public void SetCursorPos(Vector3 newPos)
    {
        cursorImage.position = newPos;
    }

    public void SetCursorVisible(bool e)
    {
        if (cursorImage == null)
            return;
        cursorImage.gameObject.SetActive(e);
    }

    #endregion

    #region Show/Hide Elements

    public void ShowWinnerText(int player)
    {
        gameOver.SetActive(true);
        switch(player)
        {
            case 1:
                player1Won.SetActive(true);
                blueConfetti.SetActive(true);
                break;
            case 2:
                player2Won.SetActive(true);
                redConfetti.SetActive(true);
                break;
        }
    }

    public void ShowHelpMenu(BaseInput opener)
    {
        if (Time.timeSinceLevelLoad - closeHelpTime < 0.2f)
            return;
        closeHelpTime = Time.timeSinceLevelLoad;
        wasBuildBaseVisible = buildBaseText.gameObject.activeSelf;
        pauseOwner = opener;
        helpGroup.SetActive(true);
        ShowPause(true, false);
    }

    public void HideHelpMenu()
    {
        if (Time.timeSinceLevelLoad - closeHelpTime < 0.2f)
            return;
        closeHelpTime = Time.timeSinceLevelLoad;
        helpGroup.SetActive(false);
        ShowPause(false, false);
    }

    public void BuildPhaseOver()
    {
        buildBaseText.gameObject.SetActive(false);
    }

    public void SetCoolCrateText(int count)
    {
        coolCrateText.SetActive(count > 0);
        coolCrateText.GetComponent<Text>().text = string.Format(coolCrateString, count);
    }

    #endregion

    #region Setting Element Values

    public void SetTimer(float time)
    {
        if (time < 0)
            timerText.text = "";
        timerText.text = Mathf.CeilToInt(time).ToString();
    }

    public void SetShotMeter(float percentage)
    {
        if (meterTransform == null)
            return;

        const float maxWidth = 200.0f;
        meterTransform.sizeDelta = new Vector2(maxWidth * percentage, meterTransform.sizeDelta.y);
    }

    public void UpdateNextBuildTurn(int turnCount, int interval)
    {
        // disable the text if the interval is super high, meaning we're 
        // probably testing the idea of only having one initial build phase
        if(interval > 50)
        {
            nextBuildText.gameObject.SetActive(false);
            return;
        }

        int next = turnCount % interval;
        nextBuildText.gameObject.SetActive(next > 0);

        int turnsToNext = interval - next;

        string turnText = "turns!";
        if (turnsToNext == 1)
            turnText = "turn!";

        nextBuildText.text = string.Format("Next build phase in {0} {1}", interval - next, turnText);
    }

    #endregion

    #region Build Preset Menu

    public void ShowBuildPresets(bool show)
    {
        buildPresetsGroup.gameObject.SetActive(show);
        tetrisContainer.gameObject.SetActive(show);
        tetrisContainer.transform.parent.gameObject.SetActive(show);
    }

    public void SetPresetPosition(bool left)
    {
        float anchorX = left ? (1.0f - distanceFromSide) : distanceFromSide;

        Vector2 anchorPos = new Vector2(anchorX, 0.5f);

        buildPresetsGroup.anchorMin = anchorPos;
        buildPresetsGroup.anchorMax = anchorPos;

        anchorPos = new Vector2(anchorX, 0.05f);
        tetrisContainer.anchorMin = anchorPos;
        tetrisContainer.anchorMax = anchorPos;

        buildPresetsGroup.anchoredPosition = Vector2.zero;
        tetrisContainer.anchoredPosition = Vector2.zero;
    }

    public void SelectPreset(int index)
    {
        gameController.GetCurrentPlayer().buildMode.SelectBuildPreset(index);
    }

    public void ShowPreset(string name)
    {
        foreach(GameObject o in tetrisIcons)
            o.SetActive(o.name == name);
    }

    #endregion

    #region Pause Menu

    // starts opening the pause menu
    // happens when start is pressed
    public void OpenPauseMenu(BaseInput opener)
    {
        pauseOwner = opener;
        // save current status of ui elements to assign when pause is closed
        wasMouseVisible = cursorImage.gameObject.activeSelf;
        wasBuildBaseVisible = buildBaseText.gameObject.activeSelf;
        // start window off the screen
        pauseWindow.anchoredPosition = new Vector3(0.0f, 999.9f);
        SetCursorVisible(true);
        ShowPause(true);
        windowLeaving = true;
        StartCoroutine(MoveWindowTo(999.9f, 0.0f, false));
    }


    // starts tweening the window's position between positions and optionally
    // closes all pause-related stuff afterwards
    private IEnumerator MoveWindowTo(float starty, float endy, bool closeAfter = false)
    {
        windowLeaving = true;
        // menu is in the middle so both X positions are 0
        Vector2 startPos = new Vector2(0.0f, starty);
        Vector2 endPos = new Vector2(0.0f, endy);

        // time elapsed when moving
        float moveTime = 0.0f;

        while (moveTime < scrollTime)
        {
            pauseWindow.anchoredPosition = startPos + (endPos - startPos) * EaseInOutQuad(moveTime / scrollTime);
            moveTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        windowLeaving = false;

        if (closeAfter)
            ShowPause(false);
    }

    // sets the active state of all relevant objects to show or hide pause
    private void ShowPause(bool show, bool isPause = true)
    {
        if (isPause)
            pauseGroup.SetActive(show);
        else
            helpGroup.SetActive(show);
        timerText.gameObject.SetActive(!show);
        buildPresetsGroup.gameObject.SetActive(!show);

        bool showBuildBase = !show;
        if (!show)
            showBuildBase = wasBuildBaseVisible;
        buildBaseText.gameObject.SetActive(showBuildBase);
    }

    // starts closing pause menu
    // happens when start is pressed OR resume is clicked
    public void ClosePauseMenu()
    {
        SetCursorVisible(wasMouseVisible);
        StartCoroutine(MoveWindowTo(0.0f, 999.9f, true));
    }

    // called when Quit button is clicked
    public void QuitToMenu()
    {
        SceneManager.LoadScene(0);
    }

    public bool IsInPauseMenu()
    {
        return pauseGroup.activeSelf || helpGroup.activeSelf;
    }

    public void VolumeChanged(float newVol)
    {
        mixer.SetFloat("MasterVolume", newVol);
    }

    #endregion

    // easing used when moving the pause menu
    private float EaseInOutQuad(float t) { return t < 0.5f ? 2 * t * t : -1 + (4 - 2 * t) * t; }

}
