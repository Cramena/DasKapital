using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class RecipeIcon : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Recipe recipe;
    private Image icon;


    private void Start()
    {
        icon = GetComponent<Image>();
        icon.sprite = recipe.result.icon;
        icon.preserveAspect = true;
    }

    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        UIService.instance.ShowRecipe(recipe);
    }

    public void OnPointerExit(PointerEventData pointerEventData)
    {
        UIService.instance.HideRecipe();
    }
}
