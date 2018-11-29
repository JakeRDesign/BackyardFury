using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quit : MonoBehaviour {

	// Use this for initialization
	void Start() {
		Invoke ("GameQuit", 4f);
	}

	void GameQuit()
	{
		Application.Quit();
	}
}
