using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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

    private void Awake()
    {
        // start by showing build base text
        buildBaseText.gameObject.SetActive(true);
    }

    private void Update()
    {
        if (GameController.IsStartDown() && !windowLeaving && IsInPauseMenu())
            ClosePauseMenu();
    }

    public void ShowWinnerText(int player)
    {
        winnerText.text = "Player " + player + " won";
        winnerText.gameObject.SetActive(true);
    }

    public void BuildPhaseOver()
    {
        buildBaseText.gameObject.SetActive(false);
    }

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

    public void OpenPauseMenu()
    {
        pauseWindow.anchoredPosition = new Vector3(0.0f, 999.9f);
        pauseGroup.SetActive(true);
        windowLeaving = true;
        StartCoroutine(MoveWindowTo(999.9f, 0.0f, false));
    }

    public void ClosePauseMenu()
    {
        StartCoroutine(MoveWindowTo(0.0f, 999.9f, true));
    }

    public void QuitToMenu()
    {
        SceneManager.LoadScene(0);
    }

    public bool IsInPauseMenu()
    {
        return pauseGroup.activeSelf;
    }

    private IEnumerator MoveWindowTo(float starty, float endy, bool closeAfter = false)
    {
        windowLeaving = true;
        Vector2 startPos = new Vector2(0.0f, starty);
        Vector2 endPos = new Vector2(0.0f, endy);
        float moveTime = 0.0f;
        while (moveTime < scrollTime)
        {
            pauseWindow.anchoredPosition = startPos + (endPos - startPos) * EaseInOutQuad(moveTime / scrollTime);
            moveTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        windowLeaving = false;

        if (closeAfter)
            pauseGroup.SetActive(false);
    }

    private float EaseInOutQuad(float t)
    {
        return t < 0.5f ? 2 * t * t : -1 + (4 - 2 * t) * t;
    }

}
