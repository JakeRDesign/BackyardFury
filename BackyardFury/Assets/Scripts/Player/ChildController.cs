using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XInputDotNetPure;

public class ChildController : MonoBehaviour
{

    // grab reference to the shoot mode so we can call back to it for
    // setting the projectile when we reach the launcher
    public ShootPlayerMode parentMode;

    [Header("Animation")]
    public Animator animator;
    public string movingBoolName = "isMoving";
    public float animatingSpeed = 1.0f;

    public GameObject holdPoint;
    public float moveSpeed = 6.0f;
    public float forwardDirection = 1.0f;

    private GameObject holding = null;
    private Rigidbody body;
    private Camera cam;
    private Vector3 homePosition;

    private void Awake()
    {
        homePosition = transform.position;

        body = GetComponent<Rigidbody>();
        cam = Camera.main;
    }

    private void FixedUpdate()
    {
        // get camera's forward and right directions so we move in the proper
        // direction :)
        Vector3 right = cam.transform.right;
        Vector3 forward = cam.transform.forward;
        // set Y values to 0 - we don't want any verticality
        right.y = 0.0f;
        forward.y = 0.0f;
        // get back to a unit vector
        right.Normalize();
        forward.Normalize();

        BaseInput ourInput = parentMode.GetComponent<BaseInput>();
        float yRawInput = ourInput.VerticalAxis();
        float xRawInput = ourInput.HorizontalAxis();

        // get movement vectors based on camera directions
        Vector3 rightMovement = xRawInput * right;
        Vector3 forwardMovement = yRawInput * forward;

        Vector3 dif = (rightMovement + forwardMovement) * moveSpeed * Time.fixedDeltaTime * 1000.0f;
        body.AddForce(dif);

        Vector3 vel = body.velocity;
        if (animator != null)
            animator.SetBool(movingBoolName, vel.magnitude > 1.0f);
        if (vel.sqrMagnitude > 1.0f)
        {
            float ang = (Mathf.Atan2(vel.x, vel.z) * Mathf.Rad2Deg) - 90.0f;
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0.0f, ang, 0.0f), Time.deltaTime * 10.0f);
        }

        if (holding != null)
        {
            holding.transform.localPosition = Vector3.zero;

            if (parentMode.GetComponent<BaseInput>().AltPressed())
            {
                holding.GetComponent<Projectile>().Dropped();
                holding = null;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // check if this component is enabled here, because apparently
        // OnTriggerEnter is still called if the component is disabled >:(
        if (!enabled)
            return;

        if (holding == null)
        {
            if (other.gameObject.tag == "Projectile")
            {
                holding = other.gameObject;
                holding.transform.parent = holdPoint.transform;
                holding.transform.localPosition = Vector3.zero;

                Projectile holdingCmp = holding.GetComponent<Projectile>();
                if (holdingCmp != null)
                {
                    holdingCmp.PickedUp();
                    holdingCmp.DisablePhysics();
                }
            }
        }
        else
        {
            if (other.gameObject.tag == "Launcher")
            {
                holding.transform.parent = null;
                holding.gameObject.SetActive(false);
                parentMode.StoreProjectile(holding);
                transform.position = homePosition;
                holding = null;
            }
        }
    }

}
