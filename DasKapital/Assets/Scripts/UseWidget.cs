using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UseWidget : MonoBehaviour
{
    public GameObject activatedBar;
    public Text valuePerUseText;

    public void InitializeUse(bool _activated, int _value)
    {
        activatedBar.SetActive(_activated);
        valuePerUseText.text = _activated ? _value.ToString() : "";
    }

    private void OnDisable()
    {
        activatedBar.SetActive(false);
        valuePerUseText.text = "";
    }
}
