using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BuildingComponent : MonoBehaviour
{
    [Header("Building Removal Stuff")]
    // whether or not we count the springs breaking as the box being "dead"
    // opposed to just using its position to see if it has moved far enough
    // NOTE: buildings placed alone on the ground won't have any springs
    // attached, so the distance should still be set!
    public bool removeOnSpringBreak = true;
    // distance from the original placed position at which we remove the box
    public float distanceToRemove = 0.5f;
    // force required to break spring joints
    public float forceToBreak = 5.0f;

    // where to make connecting joints
    public bool connectUnder = true;
    public bool connectAdjacent = true;
    // how far to look to connect boxes
    public float connectDistance = 0.6f;

    // animation
    public float fallTime = 0.2f;
    public float fallHeight = 10.0f;
    public float stretchAmount = 5.0f;
    public float squashAmount = 0.6f;
    public float inflateSpeed = 20.0f;

    // is this box still existing
    // this gets set to false as soon as the box starts disappearing, so we
    // can count it as "dead" before it's fully animated out
    public bool isAlive = true;

    // variables used to know whether or not the building is broken
    private Vector3 placedPosition;
    private List<SpringJoint> connectedSprings;

    // timer used to lerp it to the ground
    private float dropTimer;
    // stuff used for stretching/squashing
    private Vector3 startScale;

    // delegate event things
    public delegate void BuildingCallback(GameObject obj);
    public BuildingCallback onDestroy;
    public BuildingCallback onFinishedPlacing;

    public Rigidbody body;

    // Called when building is placed
    private void Start()
    {
        body = GetComponent<Rigidbody>();
        // turn off physics until we land
        body.useGravity = false;
        body.detectCollisions = false;

        dropTimer = 0.0f;
        startScale = transform.localScale;
        placedPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (dropTimer < fallTime)
        {
            dropTimer += Time.deltaTime;

            // animate
            float animPercent = 1 - (dropTimer / fallTime);
            float yOffset = fallHeight * animPercent;
            transform.position = placedPosition + Vector3.up * yOffset;

            float yStretch = stretchAmount * animPercent;
            Vector3 destScale = startScale;
            destScale.y *= squashAmount;
            transform.localScale = destScale + (Vector3.up * yStretch * startScale.y);

            // check if the fall finished this frame
            if (dropTimer >= fallTime)
            {
                // re-enable physics
                body.useGravity = true;
                body.detectCollisions = true;

                transform.position = placedPosition;
                CreateSpringConnections();
                //onFinishedPlacing(this.gameObject);
            }
            return;
        }

        transform.localScale -= (transform.localScale - startScale) *
            Time.deltaTime * inflateSpeed;

        if (connectedSprings.Count == 0 || !removeOnSpringBreak)
        {
            if (Vector3.Distance(transform.position, placedPosition) >
                distanceToRemove)
                StartCoroutine(StartRemoving());
        }
        else if (connectedSprings.Count > 0 && removeOnSpringBreak)
        {
            // check if any springs are still connected
            bool hasConnectedSpring = false;
            foreach (SpringJoint joint in connectedSprings)
            {
                if (joint != null && joint.connectedBody != null)
                {
                    hasConnectedSpring = true;
                    break;
                }
            }

            // and remove if there's none left
            if (!hasConnectedSpring)
                StartCoroutine(StartRemoving());
        }
    }

    private IEnumerator StartRemoving()
    {
        if (!isAlive)
            yield break;

        isAlive = false;

        MeshRenderer mesh = GetComponent<MeshRenderer>();

        while (mesh.material.color.a > 0)
        {
            Color c = mesh.material.color;
            c.a -= Time.deltaTime * 1.0f;
            mesh.material.color = c;

            yield return new WaitForEndOfFrame();
        }

        // call onDestroy before Destroy so we can still access this object
        onDestroy(this.gameObject);
        Destroy(this.gameObject);
    }

    private void CreateSpringConnections()
    {
        connectedSprings = new List<SpringJoint>();
        // create underneath spring
        if (connectUnder)
        {
            // raycast downwards to see if we're ontop of a box
            GameObject downBox = GetAdjacentBox(Vector3.down);
            ConnectWithSpring(downBox);
        }

        // create adjacent springs
        if (!connectAdjacent)
            return; // (this is to reduce nesting)

        // little for loop to cut the lines in half
        for (int i = -1; i <= 1; i += 2)
        {
            GameObject xBox = GetAdjacentBox(Vector3.right * i);
            GameObject zBox = GetAdjacentBox(Vector3.forward * i);

            ConnectWithSpring(xBox);
            ConnectWithSpring(zBox);
        }
    }

    // Gets an adjacent box in a certain direction
    // this is its own function because it takes some effort to ignore
    // this box
    private GameObject GetAdjacentBox(Vector3 direction)
    {
        Ray r = new Ray(transform.position, direction);

        GameObject closest = null;
        float closestDist = Mathf.Infinity;
        foreach (RaycastHit hit in Physics.RaycastAll(r))
        {
            if (hit.collider.gameObject == this.gameObject)
                continue;
            if (hit.collider.gameObject.tag != "BuildingBox")
                continue;
            if (hit.distance < closestDist &&
                hit.distance <= connectDistance)
            {
                closestDist = hit.distance;
                closest = hit.collider.gameObject;
            }
        }

        return closest;
    }

    private void ConnectWithSpring(GameObject other)
    {
        if (other == null)
            return;

        // grab the rigidbody of the box to connect it to
        Rigidbody underBody =
            other.GetComponent<Rigidbody>();
        if (underBody == null)
        {
            Debug.LogError(other.name + " DOESN'T HAVE RIGIDBODY!");
            return;
        }

        // you can't have multiple spring joints on one object, so make a
        // new object for each new joint
        GameObject jointObject = new GameObject();
        jointObject.transform.parent = gameObject.transform;
        jointObject.transform.localPosition = Vector3.zero;
        SpringJoint downJoint = jointObject.AddComponent<SpringJoint>();
        downJoint.GetComponent<Rigidbody>().isKinematic = true;

        // make sure the boxes can still collide
        downJoint.enableCollision = true;
        downJoint.breakForce = forceToBreak;
        downJoint.connectedBody = underBody;

        // add to our list so we can keep track of broken springs
        connectedSprings.Add(downJoint);
    }

}
