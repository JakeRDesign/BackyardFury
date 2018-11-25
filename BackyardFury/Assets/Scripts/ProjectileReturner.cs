using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileReturner : MonoBehaviour
{

    public Vector3 returnDirection;
    public float returnStrength = 20.0f;
    public float returnVerticalVelocity = 4.0f;
    public List<GameObject> prefabList;
    public bool throwBack = true;

    private GameController gameController;

    private void Awake()
    {
        gameController = GameObject.FindGameObjectWithTag("GameController")
            .GetComponent<GameController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!throwBack)
            return;

        if (other.tag != "Projectile")
            return;

        // ignore projectiles that are being held by a player
        if (other.transform.parent != null)
            return;

        Projectile proj = other.gameObject.GetComponent<Projectile>();
        gameController.StopFollowingProjectile();
        StartCoroutine(ThrowRandomObject(proj));
    }

    // this'll delete it after a while idk it doesn't matter
    IEnumerator DeleteObjectEventually(GameObject obj)
    {
        yield return new WaitForSeconds(Random.Range(2.0f, 5.0f));
        Destroy(obj);
    }

    IEnumerator ThrowRandomObject(Projectile otherProjectile)
    {
        if (otherProjectile != null && !otherProjectile.wasShot)
            yield break;

        // set isRemoving to true to disable the automatic turn ending when
        // colliding - we'll call this manually after a delay
        otherProjectile.isRemoving = true;

        // grab the position of the intersection so we can throw it through
        // a similar point
        Vector3 returnPos = otherProjectile.transform.position;

        yield return new WaitForSeconds(1.0f);

        if (otherProjectile != null && otherProjectile.onLand != null)
            otherProjectile.onLand();

        StartCoroutine(DeleteObjectEventually(otherProjectile.gameObject));

        // make new projectile
        GameObject newProj = Instantiate(prefabList[Random.Range(0, prefabList.Count)]);
        Rigidbody projBody = newProj.GetComponent<Rigidbody>();

        // stick it a little bit away from the fence
        newProj.transform.position = returnPos - 
            (returnDirection.normalized * 5.0f);
        // throw it back in
        Vector3 returnVelocity = returnDirection.normalized * returnStrength;
        returnVelocity.y += returnVerticalVelocity;
        projBody.velocity = returnVelocity;
    }

}
