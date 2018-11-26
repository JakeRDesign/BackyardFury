using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour
{
    public delegate void ProjectileHitEvent();
    public ProjectileHitEvent onLand;

    public string instantDestroyTag = "Collider";

    public System.Action<string> soundFunction;

    public GameObject sparklePrefab;
    public float timeBeforeDestroying = 2.0f;
    public bool isRemoving = false;
    public bool wasShot = false;

    private Rigidbody body;
    private GameObject ourSparkle;
    private float originalMass;
    private float shootTime = 0.0f;

    void Awake()
    {
        body = GetComponent<Rigidbody>();
        originalMass = body.mass;

        // we make the sparkle in code instead of having it in the prefab by
        // default because nested prefabs aren't a thing yet so it would be
        // super difficult to change the particle if we wanted to
        if (sparklePrefab != null)
        {
            // keep a note of our particles so we can hide/show them whenever
            ourSparkle = Instantiate(sparklePrefab, transform);
            // make sure the root of the particle is in the center of projectile
            ourSparkle.transform.localPosition = Vector3.zero;

            var shape = ourSparkle.GetComponent<ParticleSystem>().shape;
            shape.shapeType = ParticleSystemShapeType.MeshRenderer;
            shape.meshRenderer = GetComponent<MeshRenderer>();
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        float impact = collision.relativeVelocity.magnitude/15.0f;
        if (collision.gameObject.tag == "Ground")
            SoundManager.instance.Play("GrassImpact", impact);
        if(collision.gameObject.tag == "Fence")
            SoundManager.instance.Play("FenceHit", impact);

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

        string soundType = "Miss";
        if(collision.collider.tag == "BuildingBox")
            soundType = "Hit";
        StartCoroutine(PlayVoice(soundType));

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

        if (ourSparkle != null)
            ourSparkle.SetActive(true);
    }

    IEnumerator PlayVoice(string type) {
        yield return new WaitForSeconds(0.2f);
        if(soundFunction != null)
            soundFunction(type);
    }

    public void Shot(float power = 0.0f)
    {
        shootTime = Time.timeSinceLevelLoad;
        wasShot = true;
        isRemoving = false;

        body.mass = originalMass + (originalMass * power);
    }

    public void PickedUp()
    {
        if (ourSparkle != null)
            ourSparkle.SetActive(false);
    }

    public void Dropped()
    {
        if (ourSparkle != null)
            ourSparkle.SetActive(true);
        EnablePhysics();

        Vector3 pos = transform.position;
        transform.parent = null;
        transform.position = pos;
    }

    public void DisablePhysics()
    {
        GetComponent<Rigidbody>().isKinematic = true;
        GetComponent<Collider>().isTrigger = true;
    }

    public void EnablePhysics()
    {
        GetComponent<Rigidbody>().isKinematic = false;
        GetComponent<Collider>().isTrigger = false;
    }
}
