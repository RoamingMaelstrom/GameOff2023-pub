using UnityEngine;
using SOEvents;
using System.Collections.Generic;
using System;

public class ParticleManager : MonoBehaviour
{
    [SerializeField] GameObjectFloatSOEvent playerTakeDamageEvent;
    [SerializeField] GameObjectFloatSOEvent despawnOnKilledEvent;
    [SerializeField] List<GameObject> particleSystemPrefabs;
    [SerializeField] List<ParticleSystem> particleSystems;
    [SerializeField] ParticleSystem playerDamagedParticleSystem;

    [SerializeField] float particleScaleMultiplier = 1f;
    [SerializeField] float playerParticleScaleMultiplier = 0.1f;

    private void Awake() 
    {
        despawnOnKilledEvent.AddListener(GenerateParticles);
        playerTakeDamageEvent.AddListener(GeneratePlayerDamagedParticles);
    }

    private void GeneratePlayerDamagedParticles(GameObject playerBodyObject, float damageValue)
    {
        if (playerDamagedParticleSystem == null) return;
        playerDamagedParticleSystem.transform.position = playerBodyObject.transform.position;
        playerDamagedParticleSystem.transform.localScale = Vector3.one * Mathf.Pow(ScaleManager.ScaleSizeFactorGlobal, ScaleManager.PlayerScaleGlobal) * playerParticleScaleMultiplier;
        playerDamagedParticleSystem.Play();
    }

    private void GenerateParticles(GameObject obstacle, float arg1)
    {
        if (obstacle.TryGetComponent(out AdditionalObjectInfo objectInfo)) GenerateParticles(objectInfo.transform.position, objectInfo.transform.localScale.x, objectInfo.deathParticleType);
    }

    private void GenerateParticles(Vector3 position, float size, DeathParticleType deathParticleType)
    {
        ParticleSystem particleSystem;

        switch (deathParticleType)
        {
            case DeathParticleType.NONE: particleSystem = null; break;
            case DeathParticleType.ASTEROID_BROWN: particleSystem = particleSystems[0]; break;
            case DeathParticleType.ASTEROID_RED: particleSystem = particleSystems[1]; break;
            case DeathParticleType.ASTEROID_GREY: particleSystem = particleSystems[2]; break;
            case DeathParticleType.DEBRIS: particleSystem = particleSystems[3]; break;
            case DeathParticleType.ENEMY_BLUE: particleSystem = particleSystems[4]; break;
            default: return;
        }

        if (particleSystem == null) return;
        particleSystem.transform.position = position;
        particleSystem.transform.localScale = Vector3.one * size * particleScaleMultiplier;
        particleSystem.Play();
    }

    private void Start() 
    {
        foreach (var psPrefab in particleSystemPrefabs)
        {
            particleSystems.Add(Instantiate(psPrefab, new Vector3(-10000, 10000, 0), Quaternion.identity).GetComponent<ParticleSystem>());
        }
    }

}