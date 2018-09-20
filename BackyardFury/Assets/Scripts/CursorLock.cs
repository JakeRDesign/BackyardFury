using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorLock : MonoBehaviour {

    //--------------------------------------------------------------------------------------
    //	    Start()
    //          Runs during initialisation.
    //
    //      Param:
    //		       None
    //      Return:
    //		       Void
    //--------------------------------------------------------------------------------------
    void Start () {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = false;
    }
}
