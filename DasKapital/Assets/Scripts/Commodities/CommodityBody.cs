using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommodityBody : MonoBehaviour
{
    [HideInInspector] public RectTransform rect;
    private Commodity commodity;

    public AnimationCurve lerpCurve;
    public float lerpDuration;
    private float lerpTimer;
    private Vector2 startingPosition;
    private Vector2 targetPosition;
    private float lerpProgress;
    bool lerping;
    private Vector3 pos;
    private Transform canvas;

    void Awake()
    {
        commodity = GetComponent<Commodity>();
        rect = GetComponent<RectTransform>();
    }

    private void FixedUpdate() 
    {
        if (lerpTimer < lerpDuration)
        {
        //     if (!lerping)
        //     {
        //         lerping = true;
        //     }
        //     lerpTimer += Time.fixedDeltaTime;
        //     lerpProgress = lerpCurve.Evaluate(lerpTimer/lerpDuration);
        //     rect.anchoredPosition = Vector3.Lerp(startingPosition, targetPosition, lerpProgress);
        // }
        // else if (lerping)
        // {
        //     lerping = false;
        //     print("Lerping over");
        //     commodity.canClick = true;
        }
    }

    public void LerpTo(Vector2 _targetPosition)
    {
        lerpTimer = 0;
        lerpProgress = 0;

        startingPosition = rect.anchoredPosition;
        targetPosition = _targetPosition;
        commodity.canClick = false;
    }
}
