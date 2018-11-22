using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuInput : MonoBehaviour
{

    public RectTransform cursorImage;
    static Vector3 lastPos = Vector3.zero;

    void Awake()
    {
        var keyInput = gameObject.AddComponent<KeyboardInputController>();
        var ctrInput = gameObject.AddComponent<ControllerInputController>();

        keyInput.CursorPosFunc = GetCursorPos;
        ctrInput.CursorPosFunc = GetCursorPos;
        keyInput.SetCursorPosFunc = SetCursorPos;
        ctrInput.SetCursorPosFunc = SetCursorPos;

        if (lastPos == Vector3.zero)
            lastPos = new Vector2(Screen.currentResolution.width, Screen.currentResolution.height) / 2.0f;

        SetCursorPos(lastPos);
    }

    public Vector3 GetCursorPos()
    {
        return cursorImage.position;
    }

    public void SetCursorPos(Vector3 newPos)
    {
        lastPos = newPos;
        cursorImage.position = newPos;
    }

}
