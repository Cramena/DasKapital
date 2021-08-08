using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FloatingNoise : MonoBehaviour
{
	private RectTransform rect;
	public float maxDistance = 0.25f;
	public float noiseFrequency = 0.25f;
	public float noiseFrequencyRange = 0.1f;
	public float noiseSpeed = 1;
	public float directionLerpSpeed = 0.1f;
	private float noiseTimer;
	private float currentNoiseFrequency;
	private Vector2 lastDirection;
	private Vector2 targetDirection;
	private Vector2 noiseDirection;
	
    void Start()
    {
		rect = GetComponent<RectTransform>();
    }
	
    void FixedUpdate()
    {
        if (noiseTimer > 0)
		{
			noiseTimer -= Time.fixedDeltaTime;
		}
		else
		{
			noiseTimer = noiseFrequency + Random.Range(-noiseFrequencyRange, noiseFrequencyRange);
			currentNoiseFrequency = noiseTimer;
			NoiseMove();
		}

		// if (Mathf.Abs(rect.anchoredPosition.x) + Mathf.Abs(rect.anchoredPosition.y) > maxDistance)
		// {
		// 	StopMovement();
		// }
		UpdateDirection();
		Move();


	}

	void StopMovement()
	{
		lastDirection = noiseDirection;
		targetDirection = -rect.anchoredPosition.normalized;
	}

	void NoiseMove()
	{
		float xMove = Random.Range(-1f, 1f);
		float yMove = Random.Range(-1f, 1f);
		lastDirection = noiseDirection;
		targetDirection = new Vector2(xMove, yMove) //;
		- (rect.anchoredPosition.normalized * (rect.anchoredPosition.magnitude / maxDistance));
		targetDirection.Normalize();
	}

	void UpdateDirection()
	{
		noiseDirection = Vector2.Lerp(lastDirection, targetDirection, directionLerpSpeed * currentNoiseFrequency);
	}

	void Move()
	{
		rect.anchoredPosition += noiseDirection * noiseSpeed * Time.deltaTime;
	}
}
