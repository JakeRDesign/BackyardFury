using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bird : MonoBehaviour
{

    private bool flyingAway = false;
    public float flySpeed = 5.0f;
    public float flapSpeed = 5.0f;
    public float flapAmount = 5.0f;
    private Vector3 flyingFrom = Vector3.zero;

    private void Update()
    {
        if (!flyingAway)
            return;

        Vector3 dist = (transform.position - flyingFrom).normalized;
        dist *= Time.deltaTime;
        transform.position += dist * flySpeed;

        dist.y += Mathf.Sin(Time.timeSinceLevelLoad * flapSpeed) * flapAmount;

        transform.LookAt(transform.position + dist);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (flyingAway)
            return;

        flyingAway = true;
        flyingFrom = other.transform.position;
        flyingFrom.y -= 1.0f;
    }

}
