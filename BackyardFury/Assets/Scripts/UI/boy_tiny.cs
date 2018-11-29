using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class boy_tiny : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    public RectTransform boi;
    public float minY = 0.0f;
    public float maxY = 10.0f;
    public float hideSpeed = 5.0f;

    bool hiding = false;
    float hideTime = 0.0f;

    void Update()
    {
        float d = Time.deltaTime * (hiding ? -1 : 1) * hideSpeed;

        hideTime += d;
        if (hideTime > 1.0f)
            hideTime = 1.0f;
        if (hideTime < 0.0f)
            hideTime = 0.0f;

        float t = tween(hideTime);

        Vector2 anchored = boi.anchoredPosition;
        anchored.y = minY + (maxY - minY) * t;

        boi.anchoredPosition = anchored;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        hiding = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        hiding = false;
    }

    float tween(float t)
    {
        // y = 3.330669e-16 - 1.261905*x + 8.214286*x^2 - 5.952381*x^3
        return 0.0f - (1.619048f * t) + (9.761905f * (t * t)) - (7.142857f * (t * t * t));
    }
}
