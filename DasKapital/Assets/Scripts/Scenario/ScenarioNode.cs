using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ScenarioNode : MonoBehaviour
{
    public UnityEvent onNodeEntered;
    public UnityEvent onNodeLeft;
    private ConditionNode condition;
    public bool hasCondition
    {
        get { return condition != null; }
    }
    
    private void Awake()
    {
        condition = GetComponent<ConditionNode>();
    }

    public virtual void OnNodeEntered(bool _rewind)
    {
        if (_rewind) 
        {
            // ScenarioService.instance.SetContinueButtonActive(true);
            return;
        }
        
        onNodeEntered.Invoke();
        // ConditionNode condition = GetComponent<ConditionNode>();
        if (condition != null)
        {
            condition.Activate();
        }
        // else
        // {
        //     ScenarioService.instance.SetContinueButtonActive(true);
        // }
    }

    public virtual void OnNodeRewind()
    {
        // ScenarioService.instance.SetContinueButtonActive(true);
    }

    public virtual void OnNodeLeft()
    {
        onNodeLeft.Invoke();
    }
}
