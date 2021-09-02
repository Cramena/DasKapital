using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class RecipeIcon : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Recipe recipe;
    private Image icon;
    public float recipeDisplayDelay = 0.5f;
    private float recipeDisplayTimer;
    public float recipeHideDelay = 0.1f;
    private float recipeHideTimer;
    public bool hovering;
    private bool disablePending;


    private void Start()
    {
        icon = GetComponent<Image>();
        icon.sprite = recipe.result.icon;
        icon.preserveAspect = true;
    }

    private void Update()
    {
        if (hovering && !UIService.instance.recipePanelDisplaying)
        {
            recipeDisplayTimer += Time.deltaTime;
            if (recipeDisplayTimer >= recipeDisplayDelay)
            {
                UIService.instance.ShowRecipe(recipe);
                recipeDisplayTimer = 0;
            }
        }
        else if (recipeDisplayTimer != 0)
        {
            recipeDisplayTimer = 0;
        }
    }

    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        // UIService.instance.ShowRecipe(recipe);
        hovering = true;
    }

    public void OnPointerExit(PointerEventData pointerEventData)
    {
        hovering = false;
        UIService.instance.HideRecipe();
    }
}
