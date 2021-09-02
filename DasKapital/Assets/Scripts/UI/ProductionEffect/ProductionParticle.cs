using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProductionParticle : MonoBehaviour
{
    public RectTransform rect;
    public Image icon;
    public float dieThresholdDistance = 20;
    public float angleThreshold;
    public float rotationSpeed;
    public float lowSpeed;
    public float highSpeed;
    public float lerpSpeed;
    private float targetSpeed;
    private float currentSpeed;
    private Vector2 TargetDirection
    {
        get {return (target.position - rect.position).normalized;}
    }

    private bool facingTarget
    {
        get {return Vector2.Angle(currentDirection, TargetDirection) <= angleThreshold;}
    }
    private Vector2 currentDirection;
    private Vector2 movement;
    private RectTransform target;
    public static System.Action onProductionParticleDeath;


    public void InitializeProductionParticle(Commodity _commodity, RectTransform _target)
    {
        icon.sprite = _commodity.type.icon;
        rect.position = _commodity.rect.position;
        currentDirection = Random.insideUnitCircle.normalized;
        target = _target;
        currentSpeed = highSpeed;
        targetSpeed = highSpeed;
    }

    void Update()
    {
        CheckAngle();
        Turn();
        Accelerate();
        Move();
        ChecktargetReached();
    }

    void CheckAngle()
    {
        if (facingTarget)
        {
            targetSpeed = highSpeed;
        }
        else
        {
            targetSpeed = lowSpeed;
        }
    }

    void Turn()
    {
        float angleMovement = Vector2.SignedAngle(currentDirection, TargetDirection) > 0 ? rotationSpeed : -rotationSpeed;
        Vector2 temp = new Vector2(currentDirection.x * Mathf.Cos(angleMovement * Time.deltaTime)
                        - currentDirection.y * Mathf.Sin(angleMovement * Time.deltaTime),
                        currentDirection.x * Mathf.Sin(angleMovement * Time.deltaTime)
                        + currentDirection.y * Mathf.Cos(angleMovement * Time.deltaTime));
        currentDirection = temp.normalized;
    }

    void Accelerate()
    {
        currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, lerpSpeed * Time.deltaTime);
    }

    void Move()
    {
        rect.position += (Vector3)currentDirection * currentSpeed * Time.deltaTime;
    }

    private void ChecktargetReached()
    {
        if (Vector2.Distance(rect.position, target.position) <= dieThresholdDistance)
        {
            Die();
        }
    }

    private void Die()
    {
        onProductionParticleDeath?.Invoke();
        ProductionEffectService.instance.ReturnParticle(this);
    }
}
