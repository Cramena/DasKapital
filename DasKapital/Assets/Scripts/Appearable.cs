using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Appearable : MonoBehaviour
{
    private Animator animator;
    public bool displayed = false;
    public System.Action onDisappearing;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }  

    private void OnEnable()
    {
        displayed = true;
        if (animator == null) return;
        animator.SetTrigger("OnEnable");
    }

    public void LaunchDisappear()
    {
        displayed = false;
        print("Disappearing");
        animator.SetTrigger("Disappear");
        onDisappearing?.Invoke();
    }
}
