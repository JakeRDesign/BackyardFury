using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour
{
    public delegate void ProjectileHitEvent();
    public ProjectileHitEvent onLand;

    public string instantDestroyTag = "Collider";

    public float timeBeforeDestroying = 2.0f;
    public bool isRemoving = false;
    public bool wasShot = false;

    private Rigidbody body;
    private float originalMass;

    void Awake()
    {
        body = GetComponent<Rigidbody>();
        originalMass = body.mass;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!wasShot)
            return;
        if (isRemoving)
            return;

        onLand();
        StartCoroutine(IncreaseDrag());
        if (collision.gameObject.tag == instantDestroyTag)
            Destroy(gameObject);
        isRemoving = true;
        wasShot = false;
    }

    IEnumerator IncreaseDrag()
    {
        yield return new WaitForSeconds(1.0f);
        body.mass = 0.0000001f;
    }

    public void Shot()
    {
        wasShot = true;
        isRemoving = false;

        body.mass = originalMass;
    }
}
