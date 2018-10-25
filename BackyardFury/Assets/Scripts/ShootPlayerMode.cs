using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XInputDotNetPure;

public class ShootPlayerMode : PlayerModeBase
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
    public float aimSensitivity = 1.0f;
    private LineRenderer arcLineRenderer;

    private float shootPowerAbs = 0.0f;
    private float shootPreviewSpeed = 2.0f;
    private float previewTime = 0.0f;

    public override void Awake()
    {
        base.Awake();
        // create the linerenderer used to display the trajectory
        arcLineRenderer = gameObject.AddComponent<LineRenderer>();
        arcLineRenderer.positionCount = 0;
        arcLineRenderer.material = arcLineMaterial;
        arcLineRenderer.widthMultiplier = arcLineWidth;

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

        float xRot_P1 = xRawInput * -front * 200;
        float yRot_P1 = yRawInput * right * 200;

        shootRotation.x += xRot_P1 * aimSensitivity * Time.deltaTime;
        shootRotation.y += yRot_P1 * aimSensitivity * Time.deltaTime;

        // limit rotation
        float maxY = 140.0f * -front;
        float minY = 40.0f * -front;

        float maxX = 89.0f * -front;
        float minX = 0.0f * -front;

        // this is kinda gross, just ignore it
        // it's done because the min isn't actually the min on one team
        if (shootRotation.y < Mathf.Min(maxY, minY))
            shootRotation.y = Mathf.Min(maxY, minY);
        if (shootRotation.y > Mathf.Max(maxY, minY))
            shootRotation.y = Mathf.Max(maxY, minY);

        if (shootRotation.x < Mathf.Min(maxX, minX))
            shootRotation.x = Mathf.Min(maxX, minX);
        if (shootRotation.x > Mathf.Max(maxX, minX))
            shootRotation.x = Mathf.Max(maxX, minX);

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

        // get time point of the arc pulsy thing
        previewTime += Time.deltaTime * shootPreviewSpeed * (shootPowerAbs + 0.5f);
        float previewTimeMod = previewTime % arcPreviewLength;

        arcLineRenderer.positionCount = arcRes;
        Vector3 lastPos = launcherObject.transform.position;
        Vector3 tempForce = shootForce;
        for (int i = 0; i < arcRes; ++i)
        {
            arcLineRenderer.SetPosition(i, lastPos);

            tempForce += Physics.gravity * arcDelta;
            lastPos += tempForce * arcDelta;

            if(i * arcDelta < previewTimeMod)
            {
                arcLineMaterial.SetVector("_HighlightPos", lastPos);
            }
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
