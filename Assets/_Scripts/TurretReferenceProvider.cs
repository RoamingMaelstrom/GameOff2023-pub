using System;
using System.Collections;
using System.Collections.Generic;
using SOEvents;
using UnityEngine;

public class TurretReferenceProvider : MonoBehaviour
{
    [SerializeField] TurretChildRegisterSOEvent provideEnemyStateManagerReferenceEvent;
    [SerializeField] EnemyStateManager enemyStateManager;

    private void Awake() 
    {
        provideEnemyStateManagerReferenceEvent.AddListener(ProvideReference);
    }

    private void ProvideReference(TurretChildRegister register)
    {
        register.enemyStateManagerReference = enemyStateManager;
    }
}
