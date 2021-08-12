using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MOPTargetDoor : MonoBehaviour
{
    private Image image;
    public Color emptyColor;
    public Color fullColor;
    public float lerpSpeed = 0.1f;
    private float maxFillValue;
    private float currentFilledValue;
    public float ValueFillPercent
    {
        get { return currentFilledValue / maxFillValue; }
    }
    public System.Action onFilledUp;
    private bool fillingUp;
    
    void Awake()
    {
        image = GetComponent<Image>();
        ProductionParticle.onProductionParticleDeath += AddValue;
    }

    // Update is called once per frame
    void Update()
    {
        if (fillingUp)
        {
            LerpFillToValue();
        }
        else
        {
            LerpDown();
        }
        SetColor();
    }

    void SetColor()
    {
        if (fillingUp)
        {
            image.color = Color.Lerp(emptyColor, fullColor, image.fillAmount);
        }
        else
        {
            image.color = fullColor;
        }
    }

    void LerpFillToValue()
    {
        if (image.fillAmount != ValueFillPercent)
        {
            image.fillAmount = Mathf.Lerp(image.fillAmount, ValueFillPercent, lerpSpeed * Time.deltaTime);
            if (Mathf.Abs(ValueFillPercent - image.fillAmount)  < 0.05f)
            {
                image.fillAmount = ValueFillPercent;
                if (image.fillAmount == 1)
                {
                    CheckFilledUp();
                }
            }
        }
    }

    void LerpDown()
    {
        if (image.fillAmount < 0.01f)
        {
            image.fillAmount = 0;
            return;
        }
        image.fillAmount = Mathf.Lerp(image.fillAmount, 0, lerpSpeed * Time.deltaTime);
    }

    void CheckFilledUp()
    {
        onFilledUp?.Invoke();
        fillingUp = false;
    }

    public void AddValue()
    {
        currentFilledValue++;
    }

    public void InitializeMOPDoor(int _maxFillAmount)
    {
        image.fillAmount = 0;
        fillingUp = true;
        maxFillValue = _maxFillAmount;
        currentFilledValue = 0;
    }
}
