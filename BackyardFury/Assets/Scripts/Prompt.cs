using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Prompt : MonoBehaviour {

	public Text prompt;

	// Use this for initialization
	void Start () {
		prompt.canvasRenderer.SetAlpha(0f);
		Invoke ("StartUp", 0.1f);
		Invoke ("FadeOut", 6f);
	}

	void StartUp()
	{
		prompt.CrossFadeAlpha (1f, 1f, true);
	}

	void FadeOut()
	{
		prompt.CrossFadeAlpha (0f, 1f, true);
	}
}