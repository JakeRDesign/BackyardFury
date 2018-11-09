using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using XInputDotNetPure;

public class BuildPlayerMode : PlayerModeBase
{

    // list of placed buildings to check if we lost
    private List<GameObject> buildingObjects = new List<GameObject>();
    // grid size to snap buildings to
    public Vector3 buildSnap = new Vector3(1.0f, 0.5f, 1.0f);
    // prefab used to build
    public List<GameObject> buildingPresets;
    private GameObject selectedPreset;

    public GameObject gridPrefab;

    private GameObject ghostBuilding;

    // whether or not we're waiting for a box to be placed
    //private bool waitingForBox = false;
    private int boxesToWait = 0;

    // any material will do, probably a translucent one
    public Material ghostMaterial;
    // material to show when transparent block can't be placed
    public Material ghostMaterialError;

    // last known position of the mouse so we can detect if mouse is being
    // moved, used for disabling mouse input for controller
    private Vector3 lastMousePosition;

    // grab a reference to the grid's material so we can set the highlight
    // position to be the position of the building
    Material gridMaterial;
    GameObject gridObject;
    Vector3 buildingPos = Vector3.zero;

    // keep track of the last known state of the A button so we know which
    // frame it was pressed
    private ButtonState lastAState = ButtonState.Released;

    public override void Awake()
    {
        base.Awake();
        // create the translucent box
        //MakeGhostBuilding();
        SelectBuildPreset(0);

        // create our grid
        gridObject = Instantiate(gridPrefab);
        Vector3 gridPos = parentController.buildZone.center;
        gridPos.y = parentController.buildZone.min.y + 0.1f;
        gridObject.transform.position = gridPos;
        gridObject.transform.localScale = (parentController.buildZone.extents * 2.0f) / 10.0f;
        // grab the material for shader stuff
        gridMaterial = gridObject.GetComponent<MeshRenderer>().material;
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

        // lift up a little to make 100% sure we're not through the ground
        newPos.y += 0.05f;

        // factors to multiply and divide by to snap to the desired measurement
        // Max(0.0001, x) makes sure we don't divide by 0 and is small enough
        // to be effectively the same as having no snapping
        float xFactor = 1.0f / Mathf.Max(0.0001f, buildSnap.x);
        float zFactor = 1.0f / Mathf.Max(0.0001f, buildSnap.z);

        // snappy snap
        newPos.x = Mathf.Round(newPos.x * xFactor) / xFactor;
        newPos.z = Mathf.Round(newPos.z * zFactor) / zFactor;

        // stick the ghost building in its newly snapped position
        ghostBuilding.transform.position = newPos;

        // update building position for the highlight on the grid
        buildingPos -= (buildingPos - newPos) * Time.deltaTime * 10.0f;
        gridMaterial.SetVector("_HighlightPos", buildingPos);

        bool intersecting = IntersectingRecursive(ghostBuilding.transform);

        // check if the place is buildable
        BoxCollider ghostCollider = ghostBuilding.GetComponent<BoxCollider>();
        // condition for being buildable:
        //      - entire box is within the build zone
        //      - box isn't intersecting with anything (obstacles)
        //      - previously placed box has finished being placed
        bool isValidPosition =
            //Encapsulates(parentController.buildZone, ghostCollider.bounds) &&
            EncapsulatesRecursive(parentController.buildZone, ghostBuilding.transform) &&
            (boxesToWait == 0) && !intersecting;////!ghostBuilding.GetComponent<GhostBox>().IsIntersecting();

        // set translucent box's material based on whether 
        // or not it can be placed
        //ghostBuilding.GetComponent<MeshRenderer>().material =
        //    isValidPosition ? ghostMaterial : ghostMaterialError;
        SetGhostMaterial(ghostBuilding.transform, isValidPosition ? ghostMaterial : ghostMaterialError);

        // click to build
        GamePadState state = GamePad.GetState((PlayerIndex)parentController.playerIndex);
        if ((Input.GetMouseButtonDown(0) || (state.Buttons.A == ButtonState.Pressed && lastAState != ButtonState.Pressed)) && isValidPosition)
        {
            // make our new building
            GameObject newBuilding = Instantiate(selectedPreset);//Instantiate(boxPrefab);
            //BuildingComponent cmp = newBuilding.GetComponent<BuildingComponent>();
            // place where the ghost cube is
            newBuilding.transform.position = newPos;
            // set the flag which stops us from bulding while box is falling
            //waitingForBox = true;

            PlacedPreset(newBuilding.transform);

            // add destroy event to detect when we lose
            //cmp.onDestroy += BuildingDestroyed;
            //cmp.onFinishedPlacing += x => waitingForBox = false;
            // add to the buildingObjects list to detect when we lose
            buildingObjects.Add(newBuilding);
        }

        lastAState = state.Buttons.A;
    }

    private void PlacedPreset(Transform obj)
    {
        BuildingComponent cmp = obj.GetComponent<BuildingComponent>();
        if (cmp != null)
        {
            boxesToWait++;
            cmp.onDestroy += BuildingDestroyed;
            cmp.onFinishedPlacing += x => boxesToWait--;
        }

        foreach (Transform child in obj)
            PlacedPreset(child);
    }

    // callback function for when a box is destroyed
    private void BuildingDestroyed(GameObject destroyed)
    {
        // remove from the list that keeps track of all boxes
        // just so we can know when there's 0 left for ending the game
        buildingObjects.Remove(destroyed);
        // don't make people lose if all of their boxes get destroyed in the 
        // build phase
        if (gameController.IsBuildPhase())
            return;

        // loser haha
        if (buildingObjects.Count == 0)
            gameController.PlayerLost(parentController);
    }

    // little helper function to check if one bounds encapsulates another
    // returns true if 'a' encapsulates all of 'b'
    private bool Encapsulates(Bounds a, Bounds b)
    {
        return a.Contains(b.min) && a.Contains(b.max);
    }

    public void EnableMode() { SetEnabled(true); }
    public void DisableMode() { SetEnabled(false); }

    // function just called by EnableMode and DisableMode to reduce redundant code
    private void SetEnabled(bool b)
    {
        // hide/show grid
        gridObject.SetActive(b);
        // hide/show UI cursor
        uiController.SetCursorVisible(b);
        // hide/show translucent building
        ghostBuilding.gameObject.SetActive(b);

        // enable/disable this component
        enabled = b;
    }

    public void SelectBuildPreset(int index)
    {
        Assert.IsTrue(index >= 0 && index < buildingPresets.Count,
            "Build Preset Index should be between 0 and " + (buildingPresets.Count - 1).ToString());
        selectedPreset = buildingPresets[index];

        if (ghostBuilding != null)
            Destroy(ghostBuilding);
        ghostBuilding = MakePresetGhost(selectedPreset);
    }

    private GameObject MakePresetGhost(GameObject toGhost)
    {
        GameObject preset = Instantiate(toGhost);

        Vector3 minPoint = GetFuncPoint(preset.transform, Vector3.Min);
        Vector3 maxPoint = GetFuncPoint(preset.transform, Vector3.Max, false);

        Debug.Log("Min Point: " + minPoint.ToString());

        Vector3 size = (maxPoint - minPoint);
        Vector3 center = minPoint + (size / 2.0f);

        MakeGhostObject(preset.transform);

        //BoxCollider newCollider = preset.AddComponent<BoxCollider>();
        //newCollider.size = size;
        //newCollider.center = center;
        //newCollider.isTrigger = true;

        return preset;
    }

    private void SetGhostMaterial(Transform obj, Material newMaterial)
    {
        MeshRenderer rnd = obj.GetComponent<MeshRenderer>();
        if (rnd != null)
            rnd.material = newMaterial;

        foreach(Transform t in obj)
            SetGhostMaterial(t, newMaterial);
    }

    // recursively makes objects ready to be a ghost object
    private void MakeGhostObject(Transform obj)
    {
        MeshRenderer rnd = obj.GetComponent<MeshRenderer>();
        if (rnd != null)
            rnd.material = ghostMaterial;

        // remove rigidbody and boxcollider from ghost building so placing 
        // boxes with rigidbodies doesn't throw them away instantly
        Rigidbody rb = obj.GetComponent<Rigidbody>();
        if (rb != null)
            rb.isKinematic = true;
        BoxCollider box = obj.GetComponent<BoxCollider>();
        if (box != null)
            box.isTrigger = true;

        ObjectDropper dropper = obj.GetComponent<ObjectDropper>();
        if (dropper != null)
        {
            Destroy(dropper);
            GhostBox newGhost = obj.gameObject.AddComponent<GhostBox>();
        }

        foreach (Transform child in obj)
            MakeGhostObject(child);
    }

    private bool IntersectingRecursive(Transform t)
    {
        GhostBox gb = t.GetComponent<GhostBox>();
        if (gb != null)
            if (gb.IsIntersecting())
                return true;

        foreach (Transform child in t)
        {
            if (IntersectingRecursive(child))
                return true;
        }

        return false;
    }

    private bool EncapsulatesRecursive(Bounds a, Transform obj)
    {
        BoxCollider box = obj.GetComponent<BoxCollider>();
        if (box != null)
            if (!Encapsulates(a, box.bounds))
                return false;

        foreach (Transform t in obj)
            if (!EncapsulatesRecursive(a, t))
                return false;

        return true;
    }

    private Vector3 GetFuncPoint(Transform t, System.Func<Vector3, Vector3, Vector3> compareFunc, bool findMin = true)
    {
        Vector3 min = Vector3.zero;

        BoxCollider col = t.GetComponent<BoxCollider>();
        if (col != null)
        {
            Vector3 point = col.center;// col.bounds.min;
            if (!findMin)
                point += col.size;
            else
                point -= col.size;

            min = compareFunc(min, point);
        }

        foreach (Transform child in t)
        {
            Vector3 childMin = GetFuncPoint(child, compareFunc, findMin);
            min = compareFunc(childMin, min);
        }

        return min;
    }

    private void MakeGhostBuilding()
    {
        ghostBuilding = Instantiate(buildingPresets[0]);//Instantiate(boxPrefab);

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
