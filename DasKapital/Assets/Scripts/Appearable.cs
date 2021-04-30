using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Appearable : MonoBehaviour
{
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }  

    private void OnEnable()
    {
        if (animator == null) return;
        animator.SetTrigger("OnEnable");
    }

    public void LaunchDisappear()
    {
        animator.SetTrigger("Disappear");
        print("Launching disappear");
    }
}
