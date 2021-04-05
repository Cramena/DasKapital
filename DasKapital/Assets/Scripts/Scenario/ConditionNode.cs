using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ConditionNode : MonoBehaviour
{
    public virtual void Activate()
    {
        ScenarioService.instance.SetContinueButtonActive(false);
    }
}
