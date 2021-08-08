using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UseWidget : MonoBehaviour
{
    public Image activatedBar;
    public Text valuePerUseText;
    public Animator animator;

    public void InitializeUse(bool _activated, int _value)
    {
        activatedBar.gameObject.SetActive(_activated);
        valuePerUseText.text = _activated ? _value.ToString() : "";
    }

    private void OnDisable()
    {
        activatedBar.gameObject.SetActive(false);
        valuePerUseText.text = "";
    }

    public void SetIdleAnim()
    {
        animator.SetTrigger("Idle");
    }
    
    public void LaunchDepleteAnim()
    {
        activatedBar.gameObject.SetActive(true);
        animator.SetTrigger("Deplete");
    }
}
