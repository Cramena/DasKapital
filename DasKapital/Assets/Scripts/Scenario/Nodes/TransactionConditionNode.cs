using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransactionConditionNode : ConditionNode
{
    public override void Activate()
    {
        base.Activate();
        ExchangeService.instance.onTransaction += CheckTransaction;
    }

    public void CheckTransaction()
    {
        ScenarioService.instance.OnNodeStep();
        ExchangeService.instance.onTransaction -= CheckTransaction;
    }
}
