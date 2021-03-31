using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ScenarioNode : MonoBehaviour
{
    public UnityEvent onNodeEntered;
    public UnityEvent onNodeLeft;


    public virtual void OnNodeEntered()
    {
        onNodeEntered.Invoke();
    }

    public virtual void OnNodeLeft()
    {
        onNodeLeft.Invoke();
    }
}
