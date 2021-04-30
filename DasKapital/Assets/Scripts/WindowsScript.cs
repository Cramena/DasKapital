using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindowsScript : MonoBehaviour
{
    #if !UNITY_WEBGL
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }
    #endif
}
