using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XInputDotNetPure;

public class BuildPlayerMode : PlayerModeBase
{

    // sensitivity of cursor when using controller
    public float cursorSensitivity = 500.0f;
    // list of placed buildings to check if we lost
    public List<GameObject> buildingObjects;
    // grid size to snap buildings to
    public Vector3 buildSnap = new Vector3(1.0f, 0.5f, 1.0f);
    // prefab used to build
    public GameObject buildingPrefab;
    public GameObject verticalLinePrefab;

    private GameObject ghostBuilding;
    private GameObject verticalLine;

    // whether or not we're waiting for a box to be placed
    private bool waitingForBox = false;

    // any material will do, probably a translucent one
    public Material ghostMaterial;
    // material to show when transparent block can't be placed
    public Material ghostMaterialError;

    // last known position of the mouse so we can detect if mouse is being
    // moved, used for disabling mouse input for controller
    private Vector3 lastMousePosition;

    public override void Awake()
    {
        base.Awake();
        // create the translucent box
        MakeGhostBuilding();

        //verticalLine = Instantiate(verticalLinePrefab);
    }

    void Update()
    {
        if (gameController.IsPaused())
            return;

        // cast ray from mouse position to choose where to build
        Ray r = mainCamera.ScreenPointToRay(uiController.GetCursorPos());
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
            if (h.collider.gameObject.tag != "Ground")
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
        Ray downRay = new Ray(newPos + Vector3.up * 10.0f, Vector3.down);
        RaycastHit downHit;
        if (Physics.Raycast(downRay, out downHit, Mathf.Infinity, ~0, QueryTriggerInteraction.Ignore))
            newPos.y = downHit.point.y;
        else
            return; // exit if there's no ground

        newPos.y += 0.05f;

        // factors to multiply and divide by to snap to the desired measurement
        // Max(0.0001, x) makes sure we don't divide by 0 and is small enough
        // to be effectively the same as having no snapping
        float xFactor = 1.0f / Mathf.Max(0.0001f, buildSnap.x);
        float zFactor = 1.0f / Mathf.Max(0.0001f, buildSnap.z);

        // snappy snap
        newPos.x = Mathf.Round(newPos.x * xFactor) / xFactor;
        newPos.z = Mathf.Round(newPos.z * zFactor) / zFactor;

        if(verticalLine)
        verticalLine.transform.position = new Vector3(newPos.x, cPos.y, newPos.z);

        ghostBuilding.transform.position = newPos;

        // check if the place is buildable
        BoxCollider ghostCollider = ghostBuilding.GetComponent<BoxCollider>();
        bool isValidPosition =
            Encapsulates(parentController.buildZone, ghostCollider.bounds) &&
            !waitingForBox && !ghostBuilding.GetComponent<GhostBox>().IsIntersecting();

        List<string> allowedTags = new List<string>();
        allowedTags.Add("Ground");
        allowedTags.Add("BuildingBox");

        //isValidPosition = isValidPosition && (allowedTags.Contains(downHit.collider.gameObject.tag));

        ghostBuilding.GetComponent<MeshRenderer>().material = isValidPosition ? ghostMaterial : ghostMaterialError;

        // click to build
        GamePadState state = GamePad.GetState((PlayerIndex)parentController.playerIndex);
        if ((Input.GetMouseButton(0) || state.Buttons.A == ButtonState.Pressed) && isValidPosition)
        {
            // make our new building
            GameObject newBuilding = Instantiate(buildingPrefab);
            BuildingComponent cmp = newBuilding.GetComponent<BuildingComponent>();
            // place where the ghost cube is
            newBuilding.transform.position = newPos;
            // set the flag which stops us from bulding while box is falling
            waitingForBox = true;

            // add destroy event to detect when we lose
            cmp.onDestroy += BuildingDestroyed;
            cmp.onFinishedPlacing += x => waitingForBox = false;
            // add to the buildingObjects list to detect when we lose
            buildingObjects.Add(newBuilding);
        }
    }
    private void BuildingDestroyed(GameObject destroyed)
    {
        buildingObjects.Remove(destroyed);
        if (gameController.IsBuildPhase())
            return;
        if (buildingObjects.Count == 0)
            gameController.PlayerLost(parentController);
    }

    private bool Encapsulates(Bounds a, Bounds b)
    {
        return a.Contains(b.min) && a.Contains(b.max);
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
        ghostBuilding.gameObject.SetActive(b);
        //verticalLine.SetActive(b);
        uiController.SetCursorVisible(b);
        enabled = b;
    }

    private void MakeGhostBuilding()
    {
        ghostBuilding = Instantiate(buildingPrefab);

        ghostBuilding.GetComponent<MeshRenderer>().material = ghostMaterial;
        ghostBuilding.SetActive(false);

        // remove buildingcomponent before rigidbody because 
        // buildingcomponent depends on it!
        Destroy(ghostBuilding.GetComponent<BuildingComponent>());
        // remove rigidbody and boxcollider from ghost building so placing 
        // boxes with rigidbodies doesn't throw them away instantly
        ghostBuilding.GetComponent<Rigidbody>().isKinematic = true;
        // make collider a trigger so it doesn't collide but we can still
        // use it to get its extents
        ghostBuilding.GetComponent<BoxCollider>().isTrigger = true;
        // and remove object dropper so it doesn't weirdly drop from the sky
        Destroy(ghostBuilding.GetComponent<ObjectDropper>());

        ghostBuilding.AddComponent<GhostBox>();
    }
}
