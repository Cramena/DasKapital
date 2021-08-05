using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UILinker : MonoBehaviour
{
    private RectTransform rect;
    public RectTransform target;

    void Start()
    {
        rect = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    private void LateUpdate()
    {
        if (target == null) return;
        rect.position = target.position;
    }
}
