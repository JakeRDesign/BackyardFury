using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstaclePlacer : MonoBehaviour
{

    public int obstaclesPerSide = 2;
    public bool rotateObstacles = true;
    public List<GameObject> obstacles;

    public void PlaceObstacles(Bounds inBounds)
    {
        int placedObstacles = 0;
        for (int i = 0; i < obstaclesPerSide; ++i)
            if (PlaceObstacle(inBounds))
                placedObstacles++;

        Debug.Log("Placed " + placedObstacles + " obstacles");
    }

    private bool PlaceObstacle(Bounds inBounds)
    {
        Vector3 point;
        if (!GetRandomPoint(inBounds, out point))
        {
            Debug.LogError("Couldn't find a position for an obstacle!");
            return false;
        }

        Quaternion randomRotation = Quaternion.Euler(0, Random.Range(0.0f, 360.0f), 0);

        GameObject newObstacle = 
            Instantiate(obstacles[Random.Range(0, obstacles.Count)], point, 
            randomRotation);

        ObjectDropper dropper = newObstacle.GetComponent<ObjectDropper>();
        if (dropper)
            dropper.fallTime += Random.Range(-0.1f, 0.1f);

        return true;
    }

    private bool GetRandomPoint(Bounds inBounds, out Vector3 point)
    {
        point = Vector3.zero;

        Vector3 min = inBounds.min;
        Vector3 max = inBounds.max;

        int attemptCount = 0;
        const int maxAttempts = 100;
        const float raycastHeight = 10.0f;

        Ray r;
        RaycastHit hit;
        GameObject collided = null;
        do
        {
            if (attemptCount >= maxAttempts)
                return false;

            Vector3 rayPos = Vector3.up * raycastHeight;
            rayPos.x = Random.Range(min.x, max.x);
            rayPos.z = Random.Range(min.z, max.z);

            r = new Ray(rayPos, Vector3.down);
            if (Physics.Raycast(r, out hit))
                collided = hit.collider.gameObject;

            attemptCount++;
        } while (collided == null || collided.tag != "Ground");

        point = hit.point;
        return true;
    }
}
