using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * GHOSTBOX
 * 
 * used for checking if the box is intersecting anything so we can't 
 * build inside obstacles
 */

public class GhostBox : MonoBehaviour
{

    List<string> ignoredTags = new List<string>();

    List<Collider> colliding = new List<Collider>();

    // Use this for initialization
    void Start()
    {
        ignoredTags.Add("BuildingBox");
        ignoredTags.Add("Ground");
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (!colliding.Contains(other) && !ignoredTags.Contains(other.gameObject.tag)) 
            colliding.Add(other);
    }

    private void OnTriggerExit(Collider other)
    {
        if (colliding.Contains(other))
            colliding.Remove(other);
    }

    public bool IsIntersecting()
    {
        return colliding.Count > 0;
    }

}
