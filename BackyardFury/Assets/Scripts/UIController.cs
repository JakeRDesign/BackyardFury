﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public Text timerText;
    public Text winnerText;
    public Text buildBaseText;
    public Text nextBuildText;

    public RectTransform meterTransform;

    private void Awake()
    {
        // start by showing build base text
        buildBaseText.gameObject.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {

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

}
