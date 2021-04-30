using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DurableComponent : MonoBehaviour
{
    private Commodity commodity;

    private void Awake()
    {
        commodity = GetComponent<Commodity>();
        commodity.onDurableUsed += OnUsed;
    }

    public bool OnUsed()
    {
        commodity.profile.usesAmount--;
        commodity.profile.exchangeValue -= commodity.profile.valuePerUse;
        commodity.value = commodity.profile.exchangeValue;
        commodity.SetUsesUI();
        if (commodity.profile.usesAmount <= 0)
        {
            commodity.animator.SetTrigger("Disappear");
            return false;
        }
        return true;
    }
}
