using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour
{
    public delegate void ProjectileHitEvent();
    public ProjectileHitEvent onLand;

    private Rigidbody body;

    void Awake()
    {
        body = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnCollisionEnter(Collision collision)
    {
        Destroy(this.gameObject);
        onLand();
    }
}
