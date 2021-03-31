using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    public RectTransform target;
    
    void FixedUpdate()
    {
        GetComponent<RectTransform>().position = target.position;
    }
}
