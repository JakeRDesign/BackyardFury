using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootPlayerMode : MonoBehaviour
{

    public GameObject launcherObject;

    [Header("Projectile Settings")]
    public List<GameObject> projectilePrefabs;
    public float shootStrength = 11.0f;
    public Vector3 startingAngle;
    private Vector3 shootRotation;

    [Header("Arc Settings")]
    public Material arcLineMaterial;
    public float arcLineWidth = 0.1f;
    private LineRenderer arcLineRenderer;

    private Camera mainCamera;
    private UIController uiController;
    private PlayerController parentController;

    private float shootPowerAbs = 0.0f;

    void Awake()
    {
        // create the linerenderer used to display the trajectory
        arcLineRenderer = gameObject.AddComponent<LineRenderer>();
        arcLineRenderer.positionCount = 0;
        arcLineRenderer.material = arcLineMaterial;
        arcLineRenderer.widthMultiplier = arcLineWidth;

        // grab the camera which is used to change the controls to make sense
        // depending on which direction we're facing
        GameObject camera = GameObject.FindGameObjectWithTag("MainCamera");
        mainCamera = camera.GetComponent<Camera>();

        GameObject uiObject = GameObject.FindGameObjectWithTag("UIController");
        uiController = uiObject.GetComponent<UIController>();

        parentController = GetComponent<PlayerController>();

        shootRotation = startingAngle;
    }

    void Update()
    {
        // get the sign of the camera's forward/right vectors so we can move
        // the arc in the same direction that the camera is facing
        float front = Mathf.Sign(mainCamera.transform.forward.z);
        float right = Mathf.Sign(mainCamera.transform.right.x);

        float xRot = Input.GetAxis("Vertical") * front;
        float yRot = Input.GetAxis("Horizontal") * right;

        const float rotSpeed = 30.0f;

        shootRotation.x += xRot * rotSpeed * Time.deltaTime;
        shootRotation.y += yRot * rotSpeed * Time.deltaTime;

        Matrix4x4 matRotX =
            Matrix4x4.Rotate(Quaternion.Euler(shootRotation.x, 0, 0));
        Matrix4x4 matRotY =
            Matrix4x4.Rotate(Quaternion.Euler(0, shootRotation.y, 0));
        Matrix4x4 matRotation = matRotX * matRotY;

        Vector3 dir = matRotation.GetColumn(0);
        float e = Elastic(shootPowerAbs);
        float randFactor = 0.01f * e;
        dir.x += Mathf.Sin(Time.timeSinceLevelLoad * 19.5f) * randFactor;
        dir.y += Mathf.Cos(Time.timeSinceLevelLoad * 24.0f) * randFactor;
        dir.z += Mathf.Cos(Time.timeSinceLevelLoad * 26.0f) * randFactor;

        Vector3 shootForce = dir * shootStrength;

        const float arcDelta = 0.016f;
        const int arcRes = (int)(3.0f / arcDelta);

        arcLineRenderer.positionCount = arcRes;
        Vector3 lastPos = launcherObject.transform.position;
        Vector3 tempForce = shootForce;
        for (int i = 0; i < arcRes; ++i)
        {
            arcLineRenderer.SetPosition(i, lastPos);

            tempForce += Physics.gravity * arcDelta;
            lastPos += tempForce * arcDelta;
        }

        const float increaseSpeed = 0.2f;
        if (Input.GetButton("Fire1"))
        {
            shootPowerAbs += Time.deltaTime * increaseSpeed;
            if (shootPowerAbs > 1.0f)
                shootPowerAbs = 1.0f;
        }
        else
        {
            if (shootPowerAbs > 0.05f)
            {
                ShootProjectile(shootForce);
            }
            shootPowerAbs = 0.0f;
        }
        uiController.SetShotMeter(Elastic(shootPowerAbs));
    }

    private void ShootProjectile(Vector3 shootForce)
    {
        // choose a projectile
        GameObject projectile = projectilePrefabs[Random.Range(0, projectilePrefabs.Count)];

        GameObject newProjectile = Instantiate(projectile);
        newProjectile.transform.position =
            launcherObject.transform.position;
        newProjectile.GetComponent<Rigidbody>().velocity = shootForce;

        // call any attached functions
        if (parentController.onShoot != null)
            parentController.onShoot(newProjectile);
    }

    public void EnableMode()
    {
        SetEnabled(true);
    }

    public void DisableMode()
    {
        SetEnabled(false);
    }

    private void SetEnabled(bool b)
    {
        arcLineRenderer.enabled = b;
        enabled = b;
        shootPowerAbs = 0.0f;
    }

    private float Elastic(float abs)
    {
        return Mathf.Pow(abs, 0.5f);
    }
}
