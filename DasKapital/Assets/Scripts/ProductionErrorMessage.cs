using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProductionErrorMessage : MonoBehaviour
{
    public Text text;
    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void LaunchError(string _errorMessage)
    {
        text.text = _errorMessage;
        animator.SetTrigger("DisplayError");
    }
}
