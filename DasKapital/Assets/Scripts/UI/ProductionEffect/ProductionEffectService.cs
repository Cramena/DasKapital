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
    public Queue<ProductionParticle> particlesPool = new Queue<ProductionParticle>();
    public int poolSize = 500;
    public System.Action<Commodity> onCommodityConsumed;

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
        InitializePool();
    }

    void InitializePool()
    {
        for (int i = 0; i < poolSize; i++)
        {
            ProductionParticle newParticle = Instantiate(particlePrefab, transform);
            newParticle.gameObject.SetActive(false);
            particlesPool.Enqueue(newParticle);
        }
    }

    ProductionParticle GetParticle()
    {
        ProductionParticle particle = particlesPool.Dequeue();
        particle.gameObject.SetActive(true);
        return particle;
    }

    public void ReturnParticle(ProductionParticle _particle)
    {
        _particle.gameObject.SetActive(false);
        particlesPool.Enqueue(_particle);
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
        // if (_commodity.profile.isDurable)
        // {
            _commodity.animator.SetTrigger("Bump");
        // }
        particlesAmount += _commodity.type.index == 1 ? 3 : 0;
        for (int i = 0; i < particlesAmount; i++)
        {
            ProductionParticle currentParticle = GetParticle();
            currentParticle.rect.position = _commodity.rect.position;
            currentParticle.InitializeProductionParticle(_commodity, productionTarget);
            yield return new WaitForSeconds(particleSpawnInterval);
        }
        if (!_commodity.OnUsed())
        {
            onCommodityConsumed?.Invoke(_commodity);
        }
        else
        {
            _commodity.animator.SetTrigger("StopBump");
        }
    }
}
