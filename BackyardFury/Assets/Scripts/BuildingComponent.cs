﻿using System.Collections;
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

    // is this box still existing
    // this gets set to false as soon as the box starts disappearing, so we
    // can count it as "dead" before it's fully animated out
    public bool isAlive = true;

    // variables used to know whether or not the building is broken
    private Vector3 placedPosition;
    private List<SpringJoint> connectedSprings;

    // delegate event things
    public delegate void BuildingCallback(GameObject destroyed);
    public BuildingCallback onDestroy;
    public BuildingCallback onFinishedPlacing;

    // Called when building is placed
    private void Start()
    {
        placedPosition = transform.position;
        CreateSpringConnections();
    }

    // Update is called once per frame
    void Update()
    {
        if (connectedSprings.Count == 0)
        {
            if (Vector3.Distance(transform.position, placedPosition) > 
                distanceToRemove)
                StartCoroutine(StartRemoving());

        }
    }

    private IEnumerator StartRemoving()
    {
        isAlive = false;

        MeshRenderer mesh = GetComponent<MeshRenderer>();

        while (mesh.material.color.a > 0)
        {
            Color c = mesh.material.color;
            c.a -= Time.deltaTime * 0.1f;
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

        // make sure the boxes can still collide
        downJoint.enableCollision = true;
        downJoint.breakForce = forceToBreak;
        downJoint.connectedBody = underBody;

        // add to our list so we can keep track of broken springs
        connectedSprings.Add(downJoint);
    }

}
