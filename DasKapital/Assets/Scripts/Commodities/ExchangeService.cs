using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExchangeService : MonoBehaviour
{
    public static ExchangeService instance;
    public List<Stock> stocks = new List<Stock>();
    public List<UITarget> homeTargets = new List<UITarget>();
    public List<UITarget> otherTargets = new List<UITarget>();
    public List<Commodity> mainSelectedCommodities = new List<Commodity>();
    public List<Commodity> otherSelectedCommodities = new List<Commodity>();
    public TradingStock homeTradingStock;
    public TradingStock otherTradingStock;
    [HideInInspector] public int otherStockIndex = -1;
    
    public System.Action onTransaction;
    public System.Action<float> onBalanceUpdate;


    private void Awake() 
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            throw new System.Exception($"Too many {this}");
        }
    }

    private void Start() 
    {
        foreach (UITarget target in homeTargets)
        {
            target.onCommodityPlaced += (Commodity _commodity) => 
            { 
                mainSelectedCommodities.Add(_commodity); 
                onBalanceUpdate?.Invoke(GetBalance());
            };
            target.onCommodityUnloaded += (Commodity _commodity) => 
            {
                mainSelectedCommodities.Remove(_commodity); 
                onBalanceUpdate?.Invoke(GetBalance());
            };
        }
        foreach (UITarget target in otherTargets)
        {
            target.onCommodityPlaced += (Commodity _commodity) => 
            { 
                if (otherStockIndex != -1 && otherStockIndex != _commodity.lastTarget.stockID) return;
                otherStockIndex = _commodity.lastTarget.stockID;
                otherSelectedCommodities.Add(_commodity); 
                onBalanceUpdate?.Invoke(GetBalance());
            };
            target.onCommodityUnloaded += (Commodity _commodity) => 
            { 
                otherSelectedCommodities.Remove(_commodity); 
                if (otherSelectedCommodities.Count == 0) otherStockIndex = -1;
                onBalanceUpdate?.Invoke(GetBalance());
            };
        }
    }

    public void ProcessExchange()
    {
        if (mainSelectedCommodities.Count != 0 || otherSelectedCommodities.Count != 0) 
        {
            int mainValue = 0;
            int otherValue = 0;

            foreach (Commodity commodity in mainSelectedCommodities)
            {
                mainValue += commodity.type.exchangeValue;
            }
            foreach (Commodity commodity in otherSelectedCommodities)
            {
                otherValue += commodity.type.exchangeValue;
            }
            
            if (mainValue == otherValue)
            {
                if (stocks[otherStockIndex].GetFreeSlotsAmount() < mainSelectedCommodities.Count ||
                    stocks[0].GetFreeSlotsAmount() < otherSelectedCommodities.Count)
                {
                    print("Not enough space");
                    return;
                }
                stocks[otherStockIndex].GetCommodities(mainSelectedCommodities);
                stocks[0].GetCommodities(otherSelectedCommodities);
                onTransaction?.Invoke();
            }
            else
            {
                stocks[otherStockIndex].GetCommodities(otherSelectedCommodities);
                stocks[0].GetCommodities(mainSelectedCommodities);
            }
        }

        mainSelectedCommodities.Clear();
        otherSelectedCommodities.Clear();
        otherStockIndex = -1;
        onBalanceUpdate?.Invoke(GetBalance());
    }

    float GetBalance()
    {
        float mainValue = 0;
        float otherValue = 0;
        foreach (Commodity commodity in mainSelectedCommodities)
        {
            mainValue += commodity.profile.exchangeValue;
        }
        foreach (Commodity commodity in otherSelectedCommodities)
        {
            otherValue += commodity.profile.exchangeValue;
        }
        if (mainValue == 0 && otherValue == 0)
        {
            return 0;
        }
        else if (mainValue == 0)
        {
            return -1;
        }
        else if (otherValue == 0)
        {
            return 1;
        }
        else if (mainValue == otherValue)
        {
            return 0;
        }
        else if (mainValue >= otherValue)
        {
            return 1-(otherValue / mainValue);
        }
        else
        {
            return -(1-(mainValue / otherValue));
        }
    }

    private void OnDisable()
    {
        foreach (UITarget target in otherTargets)
        {
            target.DestroyCommodity();
        }
        foreach (UITarget target in homeTargets)
        {
            stocks[0].GetCommodities(mainSelectedCommodities);
        }
    }
}
