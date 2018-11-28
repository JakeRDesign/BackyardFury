using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeHit : MonoBehaviour {

    public GameObject particle;

    private void OnTriggerEnter(Collider other) {
        if (other.tag == "Projectile") {
            PlayParticle();
        }
    }

    void PlayParticle() {
        particle.SetActive(true);
        StartCoroutine(waitThreeSeconds());
    }

    IEnumerator waitThreeSeconds() {
        yield return new WaitForSeconds(3.0f);
        particle.SetActive(false);
    }

}
