using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectDropper : MonoBehaviour
{

    // animation
    public float fallTime = 0.2f;
    public float fallHeight = 10.0f;
    public float stretchAmount = 5.0f;
    public float squashAmount = 0.6f;
    public float inflateSpeed = 20.0f;

    public GameObject dustParticles;

    // timer used to lerp it to the ground
    private float dropTimer;
    // timer used to wait for the object to finish squashing
    private float squashTimer;
    // used for stretching/squashing
    private Vector3 startScale;
    // starting position to drop to
    private Vector3 startPosition;

    public delegate void DropperCallback(GameObject obj);
    public DropperCallback onLanded;
    public bool hasLanded { get { return _hasLanded; } }

    private bool _hasLanded = false;

    // reference to this object's rigidbody if it has one
    // so we can disable physics while we're dropping
    private Rigidbody body = null;

    // Use this for initialization
    void Start()
    {
        // grab the rigidbody if it exists
        body = GetComponent<Rigidbody>();
        DisableRigidbody();

        dropTimer = 0.0f;
        squashTimer = 0.0f;
        startScale = transform.localScale;
        startPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (dropTimer < fallTime)
        {
            dropTimer += Time.deltaTime;

            // animate
            float animPercent = 1 - Mathf.Min(1.0f, dropTimer / fallTime);
            float yOffset = fallHeight * animPercent;
            transform.position = startPosition + Vector3.up * yOffset;

            float yStretch = stretchAmount * animPercent;
            Vector3 destScale = startScale;
            destScale.y *= squashAmount;
            transform.localScale = destScale + (Vector3.up * yStretch * startScale.y);

            // check if the fall finished this frame
            if (dropTimer >= fallTime)
            {
                transform.position = startPosition;
                if (dustParticles != null)
                    Instantiate(dustParticles, transform.position, Quaternion.identity);
            }
            return;
        }

        transform.localScale -= (transform.localScale - startScale) *
            Time.deltaTime * inflateSpeed;
        squashTimer += Time.deltaTime;

        // 1/inflateSpeed is how long inflating would take if it were linear
        // so double that to be safe
        if (squashTimer >= (1.0f / inflateSpeed) * 2.0f && !hasLanded)
        {
            // squash finished
            EnableRigidbody();
            if (onLanded != null)
                onLanded(this.gameObject);
            _hasLanded = true;
        }
    }

    void DisableRigidbody()
    {
        if (body == null)
            return;

        body.useGravity = false;
        body.detectCollisions = false;
    }

    void EnableRigidbody()
    {
        if (body == null)
            return;

        body.useGravity = true;
        body.detectCollisions = true;
    }

}
