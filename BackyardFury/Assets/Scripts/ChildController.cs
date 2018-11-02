using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XInputDotNetPure;

public class ChildController : MonoBehaviour
{

    // grab reference to the shoot mode so we can call back to it for
    // setting the projectile when we reach the launcher
    public ShootPlayerMode parentMode;

    public GameObject holdPoint;
    public float moveSpeed = 6.0f;
    public float forwardDirection = 1.0f;

    private GameObject holding = null;
    private Rigidbody body;

    private void Awake()
    {
        body = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        GamePadState state = GamePad.GetState((PlayerIndex)parentMode.GetParentController().playerIndex);

        float yRawInput = state.ThumbSticks.Left.Y;
        float xRawInput = state.ThumbSticks.Left.X;

        // get keyboard input if available
        if (Mathf.Abs(Input.GetAxis("Horizontal")) > 0)
            xRawInput = Input.GetAxis("Horizontal");
        if (Mathf.Abs(Input.GetAxis("Vertical")) > 0)
            yRawInput = Input.GetAxis("Vertical");

        float xMov = xRawInput * forwardDirection;
        float yMov = yRawInput * forwardDirection;

        //Vector3 pos = transform.position;
        Vector3 dif = new Vector3(xMov, 0.0f, yMov) * -moveSpeed * Time.fixedDeltaTime * 1000.0f;
        //pos -= new Vector3(xMov, 0.0f, yMov) * moveSpeed * Time.fixedDeltaTime;
        //transform.position = pos;
        body.AddForce(dif);

        Vector3 vel = body.velocity;
        if (vel.sqrMagnitude > 1.0f)
        {
            float ang = Mathf.Atan2(vel.x, vel.z) * Mathf.Rad2Deg - 90.0f;
            transform.rotation = Quaternion.Euler(0.0f, ang, 0.0f);
        }

        if (holding != null)
            holding.transform.localPosition = Vector3.zero;
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

                Rigidbody holdingBody = holding.GetComponent<Rigidbody>();
                if (holdingBody != null)
                    holdingBody.isKinematic = true;
            }
        }
        else
        {
            if (other.gameObject.tag == "Launcher")
            {
                holding.transform.parent = null;
                holding.gameObject.SetActive(false);
                parentMode.StoreProjectile(holding);
                holding = null;
            }
        }
    }

}
