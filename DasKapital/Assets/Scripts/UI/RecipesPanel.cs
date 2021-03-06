using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RecipesPanel : MonoBehaviour
{
    public Text title;
    public Text description;
    public RectTransform rect;
    public Appearable appearable;
    public System.Action onDeath;

    public void InitializePanel(Recipe _recipe)
    {
        title.text = _recipe.result.commodityName;
        GetComponent<Animator>().SetTrigger("OnEnable");
        description.text = LocalisationService.instance.Translate(_recipe.description);
        // UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
        LayoutRebuilder.ForceRebuildLayoutImmediate(rect);
     }

     private void OnDisable()
     {
         onDeath?.Invoke();
     }
}
