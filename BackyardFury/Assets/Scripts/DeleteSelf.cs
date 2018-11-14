using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteSelf : MonoBehaviour {

    public float deleteAfter = 2.0f;

	void Start () {
        StartCoroutine(DelayDelete());
	}

    IEnumerator DelayDelete()
    {
        yield return new WaitForSeconds(deleteAfter);
        Destroy(this.gameObject);
    }

}
