using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Recipe", menuName = "DasKapital/Recipe", order = 0)]
public class Recipe : ScriptableObject 
{
    public List<CommoditySO> components;
    public CommoditySO result;
    public string description;
}
