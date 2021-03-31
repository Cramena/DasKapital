using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MeanOfProduction : UIOwner
{
    public List<Commodity> loadedCommodities = new List<Commodity>();
    public List<UITarget> targets = new List<UITarget>();
    public UITarget productionTarget;
    public System.Action onCommodityProduced;

    private void Start() 
    {
        foreach (UITarget target in targets)
        {
            target.onCommodityPlaced += (Commodity _commodity) => 
            { 
                if (!loadedCommodities.Contains(_commodity))
                {
                    loadedCommodities.Add(_commodity); 
                }
            };
            target.onCommodityUnloaded += UnloadCommodity;
        }
    }

    public void OnCommodityClicked(Commodity _commodity)
    {
        if (!loadedCommodities.Contains(_commodity))
        {
            LoadCommodity(_commodity);
        }
        else
        {
            UnloadCommodity(_commodity);
        }
    }

    public void LoadCommodity(Commodity _commodity)
    {
        loadedCommodities.Add(_commodity);
        for (var i = 0; i < loadedCommodities.Count; i++)
        {
            loadedCommodities[i].transform.SetParent(transform.GetChild(i));
            loadedCommodities[i].body.LerpTo(Vector2.zero);
        }
    }

    void UnloadCommodity(Commodity _commodity)
    {
        loadedCommodities.Remove(_commodity);
    }

    public void ProduceCommodity()
    {
        CommoditySO producedCommodity = CommoditiesService.instance.GetCommodityByComponents(loadedCommodities);
        if (producedCommodity != null && productionTarget.loadedCommodity == null)
        {
            Commodity produceInstance = CommoditiesService.instance.SpawnCommodity(producedCommodity);
            List<CommodityProfile> profiles = (from commodity in loadedCommodities
                                               select commodity.profile).ToList();
            produceInstance.TransferComponentsValue(profiles);
            productionTarget.OnCommodityPlaced(produceInstance);
            List<Commodity> toRemove = new List<Commodity>();
            foreach (Commodity component in loadedCommodities)
            {
                if (!component.OnUsed())
                {
                    toRemove.Add(component);
                }
            }
            loadedCommodities = (loadedCommodities.Where(x => !toRemove.Contains(x))).ToList();
            onCommodityProduced?.Invoke();
        }
        else if (producedCommodity == null)
        {
            print("No produce");
        }
        else
        {
            print("Clear the production space");
        }
    }

}
