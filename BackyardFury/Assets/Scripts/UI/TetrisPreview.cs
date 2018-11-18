using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TetrisPreview : MonoBehaviour {

    [Header("Preview Slots")]
    public List<TetrisSlot> previewSlots;

    public void UpdatePreviews(Queue<GameObject> tetrisQueue)
    {
        Queue<GameObject> newQueue = new Queue<GameObject>(tetrisQueue);
        newQueue.Dequeue();
        GameObject[] objs = newQueue.ToArray();

        int toShow = Mathf.Min(objs.Length, previewSlots.Count);
        for(int i = 0; i < toShow; ++i)
        {
            previewSlots[i].SetPiece(objs[i]);
        }
    }

}
