using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using XInputDotNetPure;

public class ControllerInputController : BaseInput
{

    public PlayerIndex player = PlayerIndex.One;
    public float cursorSensitivity = 300.0f;

    #region Ugly Button States
    // previous frame's state
    ButtonState lastAlt = ButtonState.Released;
    ButtonState lastFire = ButtonState.Released;
    ButtonState lastHelp = ButtonState.Released;
    ButtonState lastPause = ButtonState.Released;
    ButtonState lastNextTurn = ButtonState.Released;
    // what happened this frame
    bool altPressed = false;
    bool firePressed = false;
    bool helpPressed = false;
    bool pausePressed = false;
    bool nextTurnPressed = false;
    #endregion

    void Update()
    {
        // update button states
        GamePadState state = GetState();

        // get button states this frame
        ButtonState thisAlt = state.Buttons.B;
        ButtonState thisFire = state.Buttons.A;
        ButtonState thisHelp = state.Buttons.Back;
        ButtonState thisPause = state.Buttons.Start;
        ButtonState thisNextTurn = state.Buttons.Y;

        // determine whether or not it was pressed this frame using the last values
        altPressed = thisAlt == ButtonState.Pressed && lastAlt == ButtonState.Released;
        firePressed = thisFire == ButtonState.Pressed && lastFire == ButtonState.Released;
        helpPressed = thisHelp == ButtonState.Pressed && lastHelp == ButtonState.Released;
        pausePressed = thisPause == ButtonState.Pressed && lastPause == ButtonState.Released;
        nextTurnPressed = thisNextTurn == ButtonState.Pressed && lastNextTurn == ButtonState.Released;

        // update cursor position
        Vector3 cursorPos = CursorPosFunc();
        cursorPos.x += state.ThumbSticks.Left.X * Time.deltaTime * cursorSensitivity;
        cursorPos.y += state.ThumbSticks.Left.Y * Time.deltaTime * cursorSensitivity;

        // limit cursor position to stay on screen
        Resolution res = Screen.currentResolution;
        Vector3 screenSize = new Vector3(res.width, res.height, 0.0f);
        cursorPos = Vector3.Min(screenSize, cursorPos);
        cursorPos = Vector3.Max(Vector3.zero, cursorPos);
        SetCursorPosFunc(cursorPos);

        List<RaycastResult> results = new List<RaycastResult>();
        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            pointerId = -1,
            position = cursorPos
        };

        EventSystem.current.RaycastAll(pointerData, results);

        foreach (var cast in results)
        {
            Button b = cast.gameObject.GetComponent<Button>();
            if (b)
            {
                b.OnPointerEnter(pointerData);
                b.OnPointerExit(pointerData);

                if (state.Buttons.A == ButtonState.Pressed)
                {

                    b.OnPointerDown(pointerData);
                }
                else
                {
                    if (lastFire == ButtonState.Pressed)
                        b.OnPointerClick(pointerData);
                    else
                        b.OnPointerUp(pointerData);
                }

            }
        }

        // update last values
        lastAlt = thisAlt;
        lastFire = thisFire;
        lastHelp = thisHelp;
        lastPause = thisPause;
        lastNextTurn = thisNextTurn;
    }

    public override bool AltPressed() { return altPressed; }
    public override bool FirePressed() { return firePressed; }
    public override bool FireHeld()
    {
        return GetState().Buttons.A == ButtonState.Pressed;
    }
    public override bool HelpPressed() { return helpPressed; }
    public override bool PausePressed() { return pausePressed; }
    public override bool NextTurnPressed() { return nextTurnPressed; }
    public override float VerticalAxis() { return GetState().ThumbSticks.Left.Y; }
    public override float HorizontalAxis() { return GetState().ThumbSticks.Left.X; }

    private GamePadState GetState()
    {
        return GamePad.GetState(player);
    }
}
