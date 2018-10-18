using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XInputDotNetPure;

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
    // the length of the trajectory preview in seconds
    // so 3 = the end of the arc is where the projectile will be after
    // 3 seconds :)
    public float arcPreviewLength = 3.0f;
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
        if (uiController.IsInPauseMenu())
            return;

        // get the sign of the camera's forward/right vectors so we can move
        // the arc in the same direction that the camera is facing
        float front = Mathf.Sign(mainCamera.transform.forward.z);
        // ignore the right axis since it's broken after moving the camera
        float right = 1;

        GamePadState state = GamePad.GetState((PlayerIndex)parentController.playerIndex);

        float xRawInput = state.ThumbSticks.Left.Y;
        float yRawInput = state.ThumbSticks.Left.X;

        // get keyboard input if available
        if (Mathf.Abs(Input.GetAxis("Horizontal")) > 0)
            yRawInput = Input.GetAxis("Horizontal");
        if (Mathf.Abs(Input.GetAxis("Vertical")) > 0)
            xRawInput = Input.GetAxis("Vertical");

        float xRot_P1 = xRawInput * Time.deltaTime * -front * 200;
        float yRot_P1 = yRawInput * Time.deltaTime * right * 200;

        const float rotSpeed = 30.0f;

        shootRotation.x += xRot_P1 * rotSpeed * Time.deltaTime;
        shootRotation.y += yRot_P1 * rotSpeed * Time.deltaTime;

        //shootRotation.x += xRot_P2 * rotSpeed * Time.deltaTime;
        //shootRotation.y += yRot_P2 * rotSpeed * Time.deltaTime;

        Matrix4x4 matRotX =
            Matrix4x4.Rotate(Quaternion.Euler(shootRotation.x, 0, 0));
        Matrix4x4 matRotY =
            Matrix4x4.Rotate(Quaternion.Euler(0, shootRotation.y, 0));
        Matrix4x4 matRotation = matRotX * matRotY;

        Vector3 dir = matRotation.GetColumn(0);

        // wobblywobbles
        float e = Elastic(shootPowerAbs);
        float randFactor = 0.01f * e;
        dir.x += Mathf.Sin(Time.timeSinceLevelLoad * 19.5f) * randFactor;
        dir.y += Mathf.Cos(Time.timeSinceLevelLoad * 24.0f) * randFactor;
        dir.z += Mathf.Cos(Time.timeSinceLevelLoad * 26.0f) * randFactor;

        Vector3 shootForce = dir * shootStrength;

        // each point in the arc is 16ms of movement from the last one
        const float arcDelta = 0.016f;
        int arcRes = (int)(arcPreviewLength / arcDelta);

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
        if (Input.GetMouseButton(0) || state.Buttons.A == ButtonState.Pressed)
        {
            shootPowerAbs += Time.deltaTime * increaseSpeed;
            if (shootPowerAbs > 1.0f)
                shootPowerAbs = 1.0f;
        }
        else
        {
            if (shootPowerAbs > 0.05f)
                ShootProjectile(shootForce);
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
