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
        if (commodity.profile.usesAmount <= 0)
        {
            Destroy(gameObject);
            return false;
        }
        commodity.profile.exchangeValue -= commodity.profile.valuePerUse;
        commodity.value = commodity.profile.exchangeValue;
        commodity.SetUsesUI();
        return true;
    }
}
