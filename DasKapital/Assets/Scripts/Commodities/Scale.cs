using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scale : MonoBehaviour
{
    private RectTransform rect;
    public float maxAngle = 45;
    private float targetAngle;
    private Vector3 targetRotation;
    private Vector3 currentRotation;
    public float targetMoveSpeed;
    public float actualMoveSpeed;
    public float lerpSnapThreshold;

    private void Awake() 
    {
        rect = GetComponent<RectTransform>();
        SetScale(0);
    }

    private void Start() 
    {
        ExchangeService.instance.onBalanceUpdate += SetScale;
    }

    private void Update() 
    {
        if (targetAngle > targetRotation.z)
        {
            targetRotation.z += targetMoveSpeed * Time.deltaTime;
            targetRotation.z = Mathf.Min(targetRotation.z, targetAngle);
        }
        else 
        {
            targetRotation.z -= targetMoveSpeed * Time.deltaTime;
            targetRotation.z = Mathf.Max(targetRotation.z, targetAngle);
        }
        if (Mathf.Abs(currentRotation.z - targetRotation.z) < lerpSnapThreshold)
        {
            currentRotation = targetRotation;
        }
        else
        {
            currentRotation = Vector3.Lerp(currentRotation, targetRotation, actualMoveSpeed * Time.deltaTime);
        }
        rect.rotation = Quaternion.Euler(currentRotation);
    }

    public void SetScale(float _ratio)
    {
        targetAngle = _ratio * maxAngle;
    }
}
