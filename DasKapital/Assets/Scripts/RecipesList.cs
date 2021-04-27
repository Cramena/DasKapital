using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RecipesList", menuName = "DasKapital/RecipesList")]
public class RecipesList : ScriptableObject
{
    public List<Recipe> recipes = new List<Recipe>();
    public bool numerable;
}
