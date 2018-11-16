using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardInputController : BaseInput
{

    [Header("Keybinds")]
    public KeyCode helpButton = KeyCode.Tab;
    public KeyCode pauseButton = KeyCode.Escape;

    private void Update()
    {
        uiController.SetCursorPos(Input.mousePosition);
    }

    public override bool AltPressed() { return Input.GetMouseButtonDown(1); }
    public override bool FirePressed() { return Input.GetMouseButtonDown(0); }
    public override bool FireHeld() { return Input.GetMouseButton(0); }
    public override bool HelpPressed() { return Input.GetKeyDown(helpButton); }
    public override bool PausePressed() { return Input.GetKeyDown(pauseButton); }
    public override float VerticalAxis() { return Input.GetAxis("Vertical"); }
    public override float HorizontalAxis() { return Input.GetAxis("Horizontal"); }
}
