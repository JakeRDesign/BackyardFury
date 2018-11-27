using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioTriggers : MonoBehaviour {

	public AudioSource npc;
	public bool playedonce;

	void Start()
	{
		playedonce = false;
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Projectile" && playedonce == false)
			{
				npc.Play();
				playedonce = true;
				Destroy(other, 1f);
				Invoke ("Reset", 3f);
			}
			
}

	void Reset()
	{
		playedonce = false;
	}
}