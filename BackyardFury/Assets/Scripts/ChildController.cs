using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChildController : MonoBehaviour
{

    // grab reference to the shoot mode so we can call back to it for
    // setting the projectile when we reach the launcher
    public ShootPlayerMode parentMode;

    public GameObject holdPoint;
    public float moveSpeed = 10.0f;
    public float forwardDirection = 1.0f;

    private GameObject holding = null;

    private void FixedUpdate()
    {
        float xMov = Input.GetAxis("Horizontal") * forwardDirection;
        float yMov = Input.GetAxis("Vertical") * forwardDirection;

        Vector3 pos = transform.position;
        pos -= new Vector3(xMov, 0.0f, yMov) * moveSpeed * Time.fixedDeltaTime;
        transform.position = pos;
    }

    private void OnTriggerEnter(Collider other)
    {
        // check if this component is enabled here, because apparently
        // OnTriggerEnter is still called if the component is disabled >:(
        if (!enabled)
            return;

        if(holding == null)
        {
            if(other.gameObject.tag == "Projectile")
            {
                holding = other.gameObject;
                holding.transform.parent = holdPoint.transform;
                holding.transform.localPosition = Vector3.zero;

                Rigidbody holdingBody = holding.GetComponent<Rigidbody>();
                if (holdingBody != null)
                    holdingBody.isKinematic = true;
            }
        }
        else
        {
            if(other.gameObject.tag == "Launcher")
            {
                holding.transform.parent = null;
                holding.gameObject.SetActive(false);
                parentMode.StoreProjectile(holding);
                holding = null;
            }
        }
    }

}
