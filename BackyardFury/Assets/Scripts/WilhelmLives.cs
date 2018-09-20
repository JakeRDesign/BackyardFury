using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WilhelmLives : MonoBehaviour {

    public AudioSource wilhelm;
	
	// Update is called once per frame
	void Update () {
        if (Input.GetMouseButton(0))
        {
            wilhelm.Play();
        }
    }
}
