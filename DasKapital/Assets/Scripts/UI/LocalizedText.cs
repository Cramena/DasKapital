using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LocalizedText : MonoBehaviour
{
    public string key;
    private TMP_Text tmpText;
    private Text text;

    private void Awake()
    {
        tmpText = GetComponent<TMP_Text>();
        text = GetComponent<Text>();
    }

    private void OnEnable()
    {
        if (tmpText != null)
            tmpText.text = LocalisationService.instance.Translate(key);
        else
            text.text = LocalisationService.instance.Translate(key);
    }
}
