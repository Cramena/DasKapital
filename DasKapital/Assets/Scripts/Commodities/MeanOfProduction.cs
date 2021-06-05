using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MeanOfProduction : UIOwner
{
    public List<Commodity> loadedCommodities = new List<Commodity>();
    public ProductionErrorMessage errorMessage;
    public List<UITarget> targets = new List<UITarget>();
    public UITarget productionTarget;
    public System.Action<CommoditySO> onCommodityProduced;

    private void Start() 
    {
        GetComponent<Appearable>().onDisappearing += () =>
        {
            SetContentEnabled(false);
        };
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
        onCommodityProduced += ScenarioService.instance.RegisterProduce;
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
        Recipe recipe = CommoditiesService.instance.GetCommodityByComponents(loadedCommodities);
        if (recipe != null && productionTarget.loadedCommodity == null)
        {
            CommoditySO producedCommoditySO = recipe.result;
            Commodity produceInstance = CommoditiesService.instance.SpawnCommodity(producedCommoditySO);
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
            onCommodityProduced?.Invoke(producedCommoditySO);
            CommoditiesService.instance.CheckRecipes(recipe);
        }
        else if (recipe == null)
        {
            print("No produce from: ");
            for (var i = 0; i < loadedCommodities.Count; i++)
            {
                print(loadedCommodities[i].type);
            }
            List<Commodity> workforces = (from commodity in loadedCommodities
                                          where commodity.type.index == 1
                                          select commodity).ToList();
            if (ScenarioService.instance.workforceIntroduced && workforces.Count == 0)
            {
                errorMessage.LaunchError("Nécessite une force de travail !");
            }
            else
            {
                errorMessage.LaunchError("Ingrédients incorrects !");
            }
        }
        else
        {
            errorMessage.LaunchError("Retirez le produit précédent !");
            print("Clear the production space");
        }
    }

    private void OnEnable()
    {
        SetContentEnabled(true);
    }

    // private void OnDisable()
    // {
    //     SetContentEnabled(false);
    // }

    void SetContentEnabled(bool _enabled)
    {
        foreach (UITarget target in targets)
        {
            if (target.loadedCommodity == null) continue;
            if (!_enabled)
            {
                target.loadedCommodity.animator.SetBool("Deadly", false);
                target.loadedCommodity.animator.SetTrigger("Disappear");
            }
            else
            {
                target.loadedCommodity.gameObject.SetActive(_enabled);
            }
        }
        if (productionTarget.loadedCommodity != null) 
        {
            
            if (!_enabled)
            {
                productionTarget.loadedCommodity.animator.SetBool("Deadly", false);
                productionTarget.loadedCommodity.animator.SetTrigger("Disappear");
            }
            else
            {
                productionTarget.loadedCommodity.gameObject.SetActive(_enabled);
            }
        }
    }
    
    public void GetCommodities(Commodity _commodity)
    {
        List<UITarget> freeSlots = (from slot in targets
                                where slot.loadedCommodity == null
                                select slot).ToList();
        _commodity.StartLerp();
        freeSlots[0].OnCommodityPlaced(_commodity);
    }

}
