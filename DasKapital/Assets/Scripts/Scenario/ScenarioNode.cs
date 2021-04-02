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
        ConditionNode condition = GetComponent<ConditionNode>();
        if (condition != null)
        {
            condition.Activate();
        }
    }

    public virtual void OnNodeLeft()
    {
        onNodeLeft.Invoke();
    }
}
