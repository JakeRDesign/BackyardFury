using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TetrisSlot : MonoBehaviour
{

    public float pieceScale = 40.0f;

    GameObject currentPiece;

    private void Update()
    {
        if (currentPiece != null)
            currentPiece.transform.Rotate(Vector3.up * 90.0f * Time.deltaTime);
    }

    public void SetPiece(GameObject newPiece)
    {
        if (currentPiece != null)
            Destroy(currentPiece);

        currentPiece = Instantiate(newPiece, transform);
        currentPiece.transform.localPosition = Vector3.zero;
        currentPiece.transform.localScale = Vector3.one * pieceScale;

        CleanPiece(currentPiece.transform);
    }

    void CleanPiece(Transform t)
    {
        Destroy(t.GetComponent<ObjectDropper>());
        Destroy(t.GetComponent<BuildingComponent>());

        Destroy(t.GetComponent<Rigidbody>());
        Destroy(t.GetComponent<BoxCollider>());

        foreach (Transform other in t)
            CleanPiece(other);
    }

}
