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
    private int otherStockIndex = -1;
    

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
        // Commodity.onClick += SelectCommodity;
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
                onBalanceUpdate?.Invoke(GetBalance());
            };
        }
    }

    public void SelectCommodity(Commodity _commodity)
    {
        // if (_commodity.currentStock == 0)
        // {
        //     if (mainSelectedCommodities.Contains(_commodity))
        //     {
        //         mainSelectedCommodities.Remove(_commodity);
        //         stocks[0].SortCommodities(mainSelectedCommodities);
        //     }
        //     else
        //     {
        //         mainSelectedCommodities.Add(_commodity);
        //         homeTradingStock.GetCommodities(mainSelectedCommodities);
        //     }
        // }
        // else if (otherSelectedCommodities.Count == 0 || otherStockIndex == _commodity.currentStock)
        // {
        //     otherStockIndex = otherSelectedCommodities.Count == 0 ? _commodity.currentStock : otherSelectedCommodities[0].currentStock;
        //     if (otherSelectedCommodities.Contains(_commodity))
        //     {
        //         otherSelectedCommodities.Remove(_commodity);
        //         stocks[otherStockIndex].SortCommodities(otherSelectedCommodities);
        //     }
        //     else
        //     {
        //         otherSelectedCommodities.Add(_commodity);
        //         otherTradingStock.GetCommodities(otherSelectedCommodities);
        //     }
        // }

        // onBalanceUpdate?.Invoke(GetBalance());
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
                print("Proceeding with exchange. Other index" + otherStockIndex);
                // stocks[0].RemoveCommodities(mainSelectedCommodities);
                // stocks[otherStockIndex].RemoveCommodities(otherSelectedCommodities);

                stocks[0].GetCommodities(otherSelectedCommodities);
                stocks[otherStockIndex].GetCommodities(mainSelectedCommodities);
            }
            else
            {
                stocks[0].GetCommodities(mainSelectedCommodities);
                stocks[otherStockIndex].GetCommodities(otherSelectedCommodities);
            }
        }

        // foreach (Stock stock in stocks)
        // {
        //     stock.SortCommodities();
        // }

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
            print("Both sides are null");
            return 0;
        }
        else if (mainValue == 0)
        {
            print("Home stock is empty");
            return -1;
        }
        else if (otherValue == 0)
        {
            print("Other stock is empty");
            return 1;
        }
        else if (mainValue == otherValue)
        {
            print("Balance is equal");
            return 0;
        }
        else if (mainValue >= otherValue)
        {
            print($"Balance favors main val. Other value: {otherValue} Main value: {mainValue}.");
            return 1-(otherValue / mainValue);
        }
        else
        {
            print($"Balance favors other val. Main value: {mainValue}. Other value: {otherValue}");
            print(-(float)(mainValue / otherValue));
            return -(1-(mainValue / otherValue));
        }
    }
}
