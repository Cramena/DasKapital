using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CommoditiesService : MonoBehaviour
{
    public static CommoditiesService instance;
    public Commodity commodityPrefab;
    public CommoditySO plusValue;
    public CommoditySO workforce;
    public List<CommoditySO> spawnableCommodities = new List<CommoditySO>();
    public List<Recipe> recipes = new List<Recipe>();
    public bool numerableRecipes;
    public Worker worker;
    private int totalProbabilityWeight;
    public System.Action onCommodityPlacementRegistered;
    public System.Action<CommoditySO, List<int>> onCommoditiesEdition;

    private void Awake() 
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            throw new System.Exception($"Too many {this} instances");
        }
        InitializeTotalProbabilityWeight();
    }

    public Recipe GetCommodityByComponents(List<Commodity> _components)
    {
        List<CommoditySO> tempComponents;
        foreach (Recipe recipe in recipes)
        {
            tempComponents = (from _component in _components
                             select _component.type).ToList();
            if (recipe.components.Count != tempComponents.Count) continue; 
            foreach (CommoditySO component in recipe.components)
            {
                if (tempComponents.Contains(component))
                {
                    tempComponents.Remove(component);
                }
                else
                {
                    break;
                }
            }
            if (tempComponents.Count > 0)
            {
                continue;
            }
            else
            {
                return recipe;
            }
        }
        return null;
    }

    public Commodity SpawnCommodity(CommoditySO _type)
    {
        Commodity instatiatedCommodity = Instantiate(commodityPrefab, transform);
        instatiatedCommodity.InitializeProfile(_type);
        return instatiatedCommodity;
    }

    public void CheckWorkforceEmptied(CommoditySO _type)
    {
        if (_type == workforce)
        {
            worker.PopWorkForce();
        }
    }

    void InitializeTotalProbabilityWeight()
    {
        foreach (CommoditySO commodity in spawnableCommodities)
        {
            totalProbabilityWeight += commodity.probabilityWeight;
        }
    }

    public CommoditySO GetRandomCommodity()
    {
        int random = Random.Range(1, totalProbabilityWeight+1);
        int probabilitySum = 0;
        for (var i = 0; i < spawnableCommodities.Count; i++)
        {
            probabilitySum += spawnableCommodities[i].probabilityWeight;
            if (random <= probabilitySum)
            {
                return spawnableCommodities[i];
            }
        }
        return null;
    }

    public void RegisterCommodityPlacement()
    {
        onCommodityPlacementRegistered?.Invoke();
    }

    public void EditCommodities()
    {
        onCommoditiesEdition?.Invoke(workforce, new List<int>() {104, 107, 110, 113});
    }

    public void UpdateRecipesList(RecipesList _recipesList)
    {
        recipes =_recipesList.recipes;
        numerableRecipes = _recipesList.numerable;
    }

    public void CheckRecipes(Recipe _recipe)
    {
        if (numerableRecipes && recipes.Contains(_recipe))
        {
            recipes.Remove(_recipe);
        }
    }
}
