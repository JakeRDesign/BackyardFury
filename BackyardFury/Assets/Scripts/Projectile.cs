using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public delegate void ProjectileHitEvent();
    public ProjectileHitEvent onLand;

    public string instantDestroyTag = "Collider";

    public float timeBeforeDestroying = 2.0f;
    public bool isRemoving = false;
    public bool wasShot = false;

    void OnCollisionEnter(Collision collision)
    {
        if (!wasShot)
            return;
        if (isRemoving)
            return;

        onLand();
        if (collision.gameObject.tag == instantDestroyTag)
            Destroy(gameObject);
        isRemoving = true;
        wasShot = false;
    }

    public void Shot()
    {
        wasShot = true;
        isRemoving = false;
    }
}
