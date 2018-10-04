using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BuildingComponent : MonoBehaviour
{
    public bool isAlive = true;

    private Vector3 placedPosition;
    private SpringJoint connectedJoint;

    // destroy event
    public delegate void DestroyFunction(GameObject destroyed);
    public DestroyFunction onDestroy;

    private void Awake()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (connectedJoint != null)
        {
            if (connectedJoint.connectedBody == null)
                StartCoroutine(StartRemoving());
        }
        else if (Vector3.Distance(transform.position, placedPosition) > 0.6f)
        {
            StartCoroutine(StartRemoving());
        }
    }

    public void Placed(SpringJoint joint)
    {
        if (joint == null)
            placedPosition = transform.position;
        else
            connectedJoint = joint;
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

}
