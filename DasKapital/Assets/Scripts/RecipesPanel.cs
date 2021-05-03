using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RecipesPanel : MonoBehaviour
{
    public Text title;
    public Text description;
    public RectTransform rect;

    public void InitializePanel(Recipe _recipe)
    {
        title.text = _recipe.result.commodityName;
        description.text = _recipe.description;
        UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
        LayoutRebuilder.ForceRebuildLayoutImmediate(rect);
     }
}
