using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstaclePlacer : MonoBehaviour
{

    [Header("Build Zone Obstacles")]
    public int obstaclesPerSide = 2;
    public bool rotateObstacles = true;
    public bool isPlacing = false;
    public List<GameObject> obstacles;

    [Header("Projectiles")]
    public List<Transform> team1SpawnPoints;
    public List<Transform> team2SpawnPoints;
    public List<GameObject> projectileList;

    public void PlaceProjectiles()
    {
        foreach (Transform t in team1SpawnPoints)
            SpawnAt(t);
        foreach (Transform t in team2SpawnPoints)
            SpawnAt(t);
    }

    int lastIndex = -1;
    public void GiveTeamProjectile(int teamIndex)
    {
        List<Transform> points = team1SpawnPoints;
        if (teamIndex >= 1)
            points = team2SpawnPoints;

        // make sure we never place projectiles at the same point twice in
        // a row
        int newIndex = -1;
        do
        {
            newIndex = Random.Range(0, points.Count);
        } while (newIndex == lastIndex);

        lastIndex = newIndex;

        Transform t = points[newIndex];
        SpawnAt(t);
    }

    private void SpawnAt(Transform t)
    {
        GameObject newProj = Instantiate(projectileList[Random.Range(0, projectileList.Count)]);
        newProj.transform.position = t.position;

        GameController gc = GetComponent<GameController>();
        gc.ProjectileAdded(newProj);
    }

    public IEnumerator PlaceObstacles(Bounds inBounds)
    {
        isPlacing = true;
        int placedObstacles = 0;
        for (int i = 0; i < obstaclesPerSide; ++i)
        {
            ObjectDropper obs = PlaceObstacle(inBounds);
            if (obs == null)
                continue;

            while (!obs.hasLanded)
                yield return new WaitForEndOfFrame();

            placedObstacles++;
        }
        isPlacing = false;
    }

    private ObjectDropper PlaceObstacle(Bounds inBounds)
    {
        Vector3 point;
        if (!GetRandomPoint(inBounds, out point))
        {
            Debug.LogError("Couldn't find a position for an obstacle!");
            return null;
        }

        Quaternion randomRotation = Quaternion.Euler(0, Random.Range(0.0f, 360.0f), 0);

        GameObject newObstacle =
            Instantiate(obstacles[Random.Range(0, obstacles.Count)], point,
            randomRotation);

        ObjectDropper dropper = newObstacle.GetComponent<ObjectDropper>();
        if (dropper)
            dropper.fallTime += Random.Range(-0.1f, 0.1f);

        return dropper;
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
