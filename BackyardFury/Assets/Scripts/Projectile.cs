using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public delegate void ProjectileHitEvent();
    public ProjectileHitEvent onLand;

    public string instantDestroyTag = "Collider";

    public float timeBeforeDestroying = 2.0f;
    private bool isRemoving = false;

    void OnCollisionEnter(Collision collision)
    {
        if (isRemoving)
            return;

        onLand();
        if (collision.gameObject.tag == instantDestroyTag)
            Destroy(gameObject);
        else
            StartCoroutine(StartRemoving());
        isRemoving = true;
    }

    IEnumerator StartRemoving()
    {
        yield return new WaitForSecondsRealtime(timeBeforeDestroying);
        Destroy(this.gameObject);
    }
}
