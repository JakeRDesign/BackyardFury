using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuInput : MonoBehaviour
{

    public RectTransform cursorImage;

    void Awake()
    {
        var keyInput = gameObject.AddComponent<KeyboardInputController>();
        var ctrInput = gameObject.AddComponent<ControllerInputController>();

        keyInput.CursorPosFunc = GetCursorPos;
        ctrInput.CursorPosFunc = GetCursorPos;
        keyInput.SetCursorPosFunc = SetCursorPos;
        ctrInput.SetCursorPosFunc = SetCursorPos;
    }

    public Vector3 GetCursorPos()
    {
        return cursorImage.position;
    }

    public void SetCursorPos(Vector3 newPos)
    {
        cursorImage.position = newPos;
    }

}
