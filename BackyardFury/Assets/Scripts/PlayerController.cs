using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

// Attach this to an empty object and set up all exposed things in inspector

[System.Serializable]
public enum TurnMode
{
    BUILD,
    SHOOT
}

public class PlayerController : MonoBehaviour
{
    // any object from which the projectile should fire
    public GameObject launcherObject;

    // whether we're shooting or building - default switch key is 
    // right click or alt
    public TurnMode currentMode = TurnMode.BUILD;

    [Header("Arc Settings")]
    // normal material will do
    public Material arcLineMaterial;
    public float arcLineWidth = 0.1f;
    private LineRenderer arcLineRenderer;

    [Header("Projectile Settings")]
    // a prefab of an object with a RigidBody attached to it!
    public GameObject projectilePrefab;
    public float shootStrength = 11.0f;
    public Vector3 startingAngle;
    private Vector3 shootRotation;

    [Header("Build Settings")]
    // a default cube will do
    public GameObject buildingPrefab;
    // build zone info - make sure PlayerControllerEditor.cs is in the Editor
    // folder in the root Assets folder!
    public Bounds buildZone;
    //public Vector3 zoneCenter;
    //public Vector3 zoneSize;
    // any material will do, probably a translucent one
    public Material ghostMaterial;
    // material to show when transparent block can't be placed
    public Material ghostMaterialError;
    private GameObject ghostBuilding;
    // units to snap to, will snap every 1 meter for X and Z and every 0.5 for Y
    public Vector3 buildSnap = new Vector3(1.0f, 0.5f, 1.0f);
    public List<GameObject> buildingObjects;

    private RectTransform _cursorImage;
    private Camera _mainCamera;
    private GameController _gameController;

    // shooting delegate/event for GameController to handle the end of the turn
    public delegate void ProjectileShotEvent(GameObject projectile);
    public ProjectileShotEvent onShoot;

    void Awake()
    {
        shootRotation = startingAngle;
        if (arcLineRenderer == null)
        {
            arcLineRenderer = gameObject.AddComponent<LineRenderer>();
            arcLineRenderer.positionCount = 0;
            arcLineRenderer.material = arcLineMaterial;
            arcLineRenderer.widthMultiplier = arcLineWidth;
        }
        if (ghostBuilding == null)
        {
            ghostBuilding = Instantiate(buildingPrefab);

            ghostBuilding.GetComponent<MeshRenderer>().material = ghostMaterial;
            ghostBuilding.SetActive(false);

            // remove buildingcomponent before rigidbody because 
            // buildingcomponent depends on it!
            Destroy(ghostBuilding.GetComponent<BuildingComponent>());
            // remove rigidbody and boxcollider from ghost building so placing 
            // boxes with rigidbodies doesn't throw them away instantly
            Destroy(ghostBuilding.GetComponent<Rigidbody>());
            // make collider a trigger so it doesn't collide but we can still
            // use it to get its extents
            ghostBuilding.GetComponent<BoxCollider>().isTrigger = true;
        }

        // grab references to objects
        GameObject cursor = GameObject.FindGameObjectWithTag("UICursor");
        _cursorImage = cursor.GetComponent<RectTransform>();
        GameObject camera = GameObject.FindGameObjectWithTag("MainCamera");
        _mainCamera = camera.GetComponent<Camera>();
        GameObject controller = GameObject.FindGameObjectWithTag("GameController");
        Assert.IsNotNull(controller, "GameController must exist and be tagged!!");
        _gameController = controller.GetComponent<GameController>();

        StartBuildMode();
        Disable();
    }

    void Start()
    {
    }

    void Update()
    {
        switch (currentMode)
        {
            case TurnMode.BUILD:
                UpdateBuildMode();
                break;
            case TurnMode.SHOOT:
                UpdateShootMode();
                break;
        }

        if (Input.GetButtonDown("Fire2"))
        {
            // don't allow switching modes if we're in the build phase
            if (_gameController.IsBuildPhase())
                return;

            if (currentMode == TurnMode.BUILD)
                StartShootMode();
            else
                StartBuildMode();
        }
    }

    void UpdateBuildMode()
    {
        Vector3 cursorPos = Input.mousePosition;
        _cursorImage.position = cursorPos;

        // cast ray from mouse position to choose where to build
        Ray r = _mainCamera.ScreenPointToRay(cursorPos);
        RaycastHit[] hits = Physics.RaycastAll(r);

        // stuff to keep track of what we're hovering over
        float cDist = Mathf.Infinity;
        Vector3 cPos = Vector3.zero;
        Vector3 cNorm = Vector3.zero;
        GameObject clickedOn = null;

        foreach (RaycastHit h in hits)
        {
            // don't detect itself
            if (h.collider.gameObject == ghostBuilding)
                continue;
            // don't detect triggers - only physical colliders
            if (h.collider.isTrigger)
                continue;

            // keep track of the closest hit to the camera
            if (h.distance < cDist)
            {
                cPos = h.point;
                cNorm = h.normal;
                cDist = h.distance;
                clickedOn = h.collider.gameObject;
            }
        }

        // nothing was moused over!
        if (cDist == Mathf.Infinity)
            return;

        Vector3 newPos = cPos;
        // push back in the direction that the raycast "bounces"
        newPos += cNorm * 0.5f;

        // raycast downwards to place on the ground
        Ray downRay = new Ray(newPos + Vector3.up, Vector3.down);
        RaycastHit downHit;
        if (Physics.Raycast(downRay, out downHit, Mathf.Infinity, ~0, QueryTriggerInteraction.Ignore))
            newPos.y = downHit.point.y + 0.5f;
        else
            return; // exit if there's no ground

        // factors to multiply and divide by to snap to the desired measurement
        // Max(0.0001, x) makes sure we don't divide by 0 and is small enough
        // to be effectively the same as having no snapping
        float xFactor = 1.0f / Mathf.Max(0.0001f, buildSnap.x);
        float zFactor = 1.0f / Mathf.Max(0.0001f, buildSnap.z);

        // snappy snap
        newPos.x = Mathf.Round(newPos.x * xFactor) / xFactor;
        newPos.z = Mathf.Round(newPos.z * zFactor) / zFactor;

        ghostBuilding.transform.position = newPos;

        // check if the place is buildable
        BoxCollider ghostCollider = ghostBuilding.GetComponent<BoxCollider>();
        bool isValidPosition = Encapsulates(buildZone, ghostCollider.bounds);

        ghostBuilding.GetComponent<MeshRenderer>().material = isValidPosition ? ghostMaterial : ghostMaterialError;

        // click to build
        if (Input.GetButtonDown("Fire1") && isValidPosition)
        {
            // make our new building
            GameObject newBuilding = Instantiate(buildingPrefab);
            BuildingComponent cmp = newBuilding.GetComponent<BuildingComponent>();
            // place where the ghost cube is
            newBuilding.transform.position = newPos;

            // add destroy event to detect when we lose
            cmp.onDestroy += BuildingDestroyed;
            // add to the buildingObjects list to detect when we lose
            buildingObjects.Add(newBuilding);

            // to keep it from being super easy to destroy peoples bases,
            // attach boxes with spring joints which make the buildings
            // stay standing if they weren't hit very hard
            SpringJoint newSpring = null;
            if (clickedOn.tag == "BuildingBox")
            {
                Rigidbody clickedBody = clickedOn.GetComponent<Rigidbody>();

                // make new spring joint
                newSpring = newBuilding.AddComponent<SpringJoint>();
                // attach to what we clicked on
                newSpring.connectedBody = clickedBody;
                newSpring.enableCollision = true;
                // break force might need tweaking - should probably expose
                newSpring.breakForce = 10.0f;
            }

            // let the component know it's been placed so it can start
            // detecting if it needs to break
            cmp.Placed(newSpring);
        }
    }

    void UpdateShootMode()
    {
        // get the sign of the camera's forward/right vectors so we can move
        // the arc in the same direction that the camera is facing
        float front = Mathf.Sign(_mainCamera.transform.forward.z);
        float right = Mathf.Sign(_mainCamera.transform.right.x);

        float xRot = Input.GetAxis("Vertical") * front;
        float yRot = Input.GetAxis("Horizontal") * right;

        const float rotSpeed = 20.0f;

        shootRotation.x += xRot * rotSpeed * Time.deltaTime;
        shootRotation.y += yRot * rotSpeed * Time.deltaTime;

        Matrix4x4 matRotX =
            Matrix4x4.Rotate(Quaternion.Euler(shootRotation.x, 0, 0));
        Matrix4x4 matRotY =
            Matrix4x4.Rotate(Quaternion.Euler(0, shootRotation.y, 0));
        Matrix4x4 matRotation = matRotX * matRotY;

        Vector3 dir = matRotation.GetColumn(0);
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

        if (Input.GetButtonDown("Fire1"))
        {
            GameObject newProjectile = Instantiate(projectilePrefab);
            newProjectile.transform.position =
                launcherObject.transform.position;
            newProjectile.GetComponent<Rigidbody>().velocity = shootForce;

            // call any attached functions
            onShoot(newProjectile);
        }
    }

    void StartBuildMode()
    {
        currentMode = TurnMode.BUILD;

        ghostBuilding.SetActive(true);
        arcLineRenderer.enabled = false;
        _cursorImage.gameObject.SetActive(true);
    }

    void StartShootMode()
    {
        currentMode = TurnMode.SHOOT;

        ghostBuilding.SetActive(false);
        arcLineRenderer.enabled = true;
        _cursorImage.gameObject.SetActive(false);
    }

    public void Enable()
    {
        this.enabled = true;
        // start the last mode we were in 
        if (currentMode == TurnMode.SHOOT)
            StartShootMode();
        else
            StartBuildMode();
    }

    public void Disable()
    {
        this.enabled = false;
        arcLineRenderer.enabled = false;
        this.ghostBuilding.gameObject.SetActive(false);
    }

    private void BuildingDestroyed(GameObject destroyed)
    {
        buildingObjects.Remove(destroyed);
        if (_gameController.IsBuildPhase())
            return;
        if (buildingObjects.Count == 0)
            _gameController.PlayerLost(this);
    }

    private bool Encapsulates(Bounds a, Bounds b)
    {
        return a.Contains(b.min) && a.Contains(b.max);
    }

}
