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
    // what happened this frame
    bool altPressed = false;
    bool firePressed = false;
    bool helpPressed = false;
    bool pausePressed = false;
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

        // determine whether or not it was pressed this frame using the last values
        altPressed = thisAlt == ButtonState.Pressed && lastAlt == ButtonState.Released;
        firePressed = thisFire == ButtonState.Pressed && lastFire == ButtonState.Released;
        helpPressed = thisHelp == ButtonState.Pressed && lastHelp == ButtonState.Released;
        pausePressed = thisPause == ButtonState.Pressed && lastPause == ButtonState.Released;

        // update last values
        lastAlt = thisAlt;
        lastFire = thisFire;
        lastHelp = thisHelp;
        lastPause = thisPause;

        // update cursor position
        //Vector3 cursorPos = uiController.GetCursorPos();
        Vector3 cursorPos = CursorPosFunc();//uiController.GetCursorPos();
        cursorPos.x += state.ThumbSticks.Left.X * Time.deltaTime * cursorSensitivity;
        cursorPos.y += state.ThumbSticks.Left.Y * Time.deltaTime * cursorSensitivity;
        SetCursorPosFunc(cursorPos);
        //uiController.SetCursorPos(cursorPos);

        // check for UI clicks
        // check if a UI button was pressed with controller
        if (state.Buttons.A == ButtonState.Pressed)
        {
            // make pointer data to pass into raycasting event
            PointerEventData pointerData = new PointerEventData(EventSystem.current)
            {
                pointerId = -1,
                position = cursorPos
            };

            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, results);

            foreach (var cast in results)
            {
                Button b = cast.gameObject.GetComponent<Button>();
                if (b)
                    b.OnPointerClick(pointerData);
            }
        }
    }

    public override bool AltPressed() { return altPressed; }
    public override bool FirePressed() { return firePressed; }
    public override bool FireHeld()
    {
        return GetState().Buttons.A == ButtonState.Pressed;
    }
    public override bool HelpPressed() { return helpPressed; }
    public override bool PausePressed() { return pausePressed; }
    public override float VerticalAxis() { return GetState().ThumbSticks.Left.Y; }
    public override float HorizontalAxis() { return GetState().ThumbSticks.Left.X; }

    private GamePadState GetState()
    {
        return GamePad.GetState(player);
    }

}
