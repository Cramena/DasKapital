using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AsteriskPanel : MonoBehaviour
{
    public Text text;
    public string currentKey;
    public RectTransform rect;

    public void InitializePanel(string _text)
    {
        GetComponent<HorizontalLayoutGroup>().enabled = true;
        currentKey = _text;
        text.text = LocalisationService.instance.Translate(_text);
        LayoutRebuilder.ForceRebuildLayoutImmediate(rect);
    }
}
