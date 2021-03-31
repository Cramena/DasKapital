using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum CoinType
{
    None,
    Material,
    Salary,
    Profit
}

public class DistributionCommodity : MonoBehaviour
{
    [HideInInspector] public Commodity commodity;
    public Image icon;
    public List<Color> typeColors = new List<Color>();
    public CoinType type;
    public CommoditySO coinSO;
    private bool selfDestructPending;

    public void Initialize(CoinType _coinType)
    {
        commodity = GetComponent<Commodity>();
        commodity.InitializeProfile(coinSO);
        commodity.draggable = false;
        transform.SetParent(UIService.instance.coinsPanel);
        switch(_coinType)
        {
            case CoinType.Material :
                icon.color = typeColors[0];
                break;
            case CoinType.Salary :
                icon.color = typeColors[1];
                break;
            case CoinType.Profit :
                icon.color = typeColors[2];
                break;
        }
        type = _coinType;
    }

    public void GetDistributedTo(UITarget _target)
    {
        commodity.target = _target;
        commodity.StartLerp();
        selfDestructPending = true;
    }

    private void Update() 
    {
        if (selfDestructPending && commodity.state != CommodityState.Lerp)
        {
            Destroy(gameObject);
        }
    }
}
