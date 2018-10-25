using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

    [Header("Pause Menu")]
    public GameObject pauseGroup;
    public RectTransform pauseWindow;
    public Image pauseBackground;
    public float scrollTime = 0.3f;
    public float backgroundAlpha = 0.8f;
    private bool windowLeaving = false;
    private bool wasMouseVisible = false;
    private bool wasBuildBaseVisible = false;

    [Header("Controller Cursor")]
    public float cursorSensitivity = 100.0f;
    private Vector3 lastMousePosition;

    private RectTransform cursorImage;
    private GameController gameController;

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
        if (GameController.IsStartDown() && !windowLeaving && IsInPauseMenu())
            ClosePauseMenu();

        if (cursorImage.gameObject.activeSelf)
            UpdateCursorPosition();
    }

    #region Displayed Cursor

    void UpdateCursorPosition()
    {
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
        winnerText.text = "Player " + player + " won";
        winnerText.gameObject.SetActive(true);
    }

    public void BuildPhaseOver()
    {
        buildBaseText.gameObject.SetActive(false);
    }

    #endregion

    #region Setting Element Values

    public void SetTimer(float time)
    {
        if (time < 0)
            timerText.text = "";
        timerText.text = Mathf.Floor(time).ToString();
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
    }

    public void SetPresetPosition(bool left)
    {
        buildPresetsGroup.anchorMin = new Vector2(!left ? 0.0f : 1.0f, 0.5f);
        buildPresetsGroup.anchorMax = new Vector2(!left ? 0.0f : 1.0f, 0.5f);

        buildPresetsGroup.anchoredPosition = new Vector2(1920.0f * 0.1f * (!left ? 1.0f : -1.0f), 0.0f);
    }

    #endregion

    #region Pause Menu

    // starts opening the pause menu
    // happens when start is pressed
    public void OpenPauseMenu()
    {
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
    private void ShowPause(bool show)
    {
        pauseGroup.SetActive(show);
        timerText.gameObject.SetActive(!show);
        buildPresetsGroup.gameObject.SetActive(!show);
        buildBaseText.gameObject.SetActive(wasBuildBaseVisible);
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
        return pauseGroup.activeSelf;
    }

    #endregion

    // easing used when moving the pause menu
    private float EaseInOutQuad(float t) { return t < 0.5f ? 2 * t * t : -1 + (4 - 2 * t) * t; }

}
