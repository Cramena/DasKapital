using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DynamicBubble : MonoBehaviour
{
    private Appearable appearable;
    public TMP_Text text;

    private void Start()
    {
        appearable = GetComponent<Appearable>();
    }

    public void InitializeBubble(string _text)
    {
        text.text = _text;
    }

    public void Die()
    {
        appearable.LaunchDisappear();
    }
}
