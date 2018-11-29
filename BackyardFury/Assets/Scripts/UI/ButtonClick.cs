using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonClick : MonoBehaviour {

    public void PlayClick()
    {
        SoundManager mgr = SoundManager.instance;
        if (mgr)
            mgr.Play("Click");
    }

}
