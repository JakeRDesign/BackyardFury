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
    private float shootTime = 0.0f;

    void Awake()
    {
        body = GetComponent<Rigidbody>();
        originalMass = body.mass;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.isTrigger)
            return;
        if (collision.collider.gameObject.tag == "Launcher")
            return;
        if ((Time.timeSinceLevelLoad - shootTime) < 0.1f)
            return;

        if (!wasShot)
            return;
        if (isRemoving)
            return;

        Debug.Log("Colliding with " + collision.gameObject.name);

        onLand();
        StartCoroutine(IncreaseDrag());
        if (collision.gameObject.tag == instantDestroyTag)
            Destroy(gameObject);
        isRemoving = true;
        wasShot = false;
    }

    IEnumerator IncreaseDrag()
    {
        yield return new WaitForSeconds(3.0f);
        body.mass = 0.0000001f;
    }

    public void Shot(float power = 0.0f)
    {
        shootTime = Time.timeSinceLevelLoad;
        wasShot = true;
        isRemoving = false;

        body.mass = originalMass + (originalMass * power);
    }
}
