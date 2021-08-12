using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProductionEffectService : MonoBehaviour
{
    public static ProductionEffectService instance;
    public RectTransform rect;
    public ProductionParticle particlePrefab;
    public RectTransform productionTarget;
    public float particleSpawnInterval = 0.1f;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            throw new System.Exception($"Too many {this} instances!");
        }
    }

    public void LaunchProductionEffect(List<Commodity> _commodities)
    {
        foreach (Commodity commodity in _commodities)
        {
            StartCoroutine(SpawnProductionParticles(commodity));
        }
    }

    IEnumerator SpawnProductionParticles(Commodity _commodity)
    {
        int particlesAmount = _commodity.profile.isDurable ? _commodity.profile.valuePerUse * 3 : _commodity.profile.exchangeValue * 3;
        particlesAmount += _commodity.type.index == 1 ? 3 : 0;
        for (int i = 0; i < particlesAmount; i++)
        {
            ProductionParticle currentParticle = Instantiate(particlePrefab, transform);
            currentParticle.rect.position = _commodity.rect.position;
            currentParticle.InitializeProductionParticle(_commodity, productionTarget);
            yield return new WaitForSeconds(particleSpawnInterval);
        }
    }
}
