using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ExchangeService : MonoBehaviour
{
    public static ExchangeService instance;
    public ProductionErrorMessage errorMessage;
    public List<Stock> stocks = new List<Stock>();
    public List<UITarget> homeTargets = new List<UITarget>();
    public List<UITarget> otherTargets = new List<UITarget>();
    public ValueCounter homeCounter;
    public ValueCounter otherCounter;
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
                if (otherStockIndex != -1 && otherStockIndex != _commodity.lastTarget.stockID) 
                {
                    List<Commodity> loadedCommodities = (from slot in otherTargets
                                                        where slot.loadedCommodity != null && slot.loadedCommodity != _commodity
                                                        select slot.loadedCommodity).ToList();
                    stocks[otherStockIndex].GetCommodities(loadedCommodities);
                }
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
                mainValue += commodity.profile.exchangeValue;
            }
            foreach (Commodity commodity in otherSelectedCommodities)
            {
                otherValue += commodity.profile.exchangeValue;
            }
            
            if (mainValue == otherValue)
            {
                if (stocks[otherStockIndex].GetFreeSlotsAmount() < mainSelectedCommodities.Count ||
                    stocks[0].GetFreeSlotsAmount() < otherSelectedCommodities.Count)
                {
                    print("Not enough space");
                    errorMessage.LaunchError("Pas assez d'espace dans le stock !");
                    return;
                }
                stocks[otherStockIndex].GetCommodities(mainSelectedCommodities);
                stocks[0].GetCommodities(otherSelectedCommodities);
                onTransaction?.Invoke();
            }
            else
            {
                errorMessage.LaunchError("Les deux paniers n'ont pas la même valeur !");
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
        homeCounter.SetTargetValue(mainValue);        
        otherCounter.SetTargetValue(otherValue);        
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

    public void DisableTradingStocksContent()
    {
        DestroyOtherStockContent();
        foreach (UITarget target in homeTargets)
        {
            stocks[0].GetCommodities(mainSelectedCommodities);
        }
    }

    public void DestroyOtherStockContent()
    {
        foreach (UITarget target in otherTargets)
        {
            target.DestroyCommodity();
        }
    }

    public void GetCommodities(Commodity _commodity, bool _targetsHome)
    {
        print("Trading stock gets commodity");
        List<UITarget> targets = _targetsHome ? homeTargets : otherTargets;
        if (!_targetsHome && otherStockIndex != _commodity.target.stockID && otherStockIndex != -1)
        {
            List<Commodity> loadedCommodities = (from slot in targets
                                                where slot.loadedCommodity != null && slot.loadedCommodity != _commodity
                                                select slot.loadedCommodity).ToList();
            stocks[otherStockIndex].GetCommodities(loadedCommodities);
            errorMessage.LaunchError("Un seul stock à la fois !");
        }
        List<UITarget> freeSlots = (from slot in targets
                                where slot.loadedCommodity == null
                                select slot).ToList();
        _commodity.StartLerp();
        freeSlots[0].OnCommodityPlaced(_commodity);
    }
}
