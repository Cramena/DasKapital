using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ValueCounter : MonoBehaviour
{
    public Text valueText;
    public float valueLerpThreshold = 0.1f;
    public float valueLerpSpeed = 0.2f;
    private float value;
    private float targetValue;
    
    private void OnEnable()
    {
        value = 0;
        targetValue = 0;
        valueText.text = value.ToString();
    }

    public void SetTargetValue(float _value)
    {
        targetValue = (int)_value;
    }

    private void Update() 
    {
        if (targetValue != value)
        {
            print("Target and value are different");
            if (Mathf.Abs(value - targetValue) < valueLerpThreshold)
            {
                print("Threshold reached");
                value = targetValue;
                valueText.text = /*value == 0 ? "0" : */ value.ToString();
            }
            else
            {
                value = Mathf.Lerp(value, targetValue, valueLerpSpeed * Time.deltaTime);
                valueText.text = Mathf.RoundToInt(value).ToString();
            }
        }
    }

}
