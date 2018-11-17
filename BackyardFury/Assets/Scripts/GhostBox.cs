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
        // shrink collider a little so we don't collide with already placed boxes
        BoxCollider col = GetComponent<BoxCollider>();
        if (col != null)
            col.size *= 0.9f;

        ignoredTags.Add("Ground");
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
