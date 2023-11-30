using UnityEngine;
using SOEvents;

public class TurretChildRegister : MonoBehaviour
{
    [SerializeField] TurretChildRegisterSOEvent provideEnemyStateManagerReferenceEvent;
    [SerializeField] GameObject turretObject;
    public EnemyStateManager enemyStateManagerReference;

    private void OnEnable() 
    {
        provideEnemyStateManagerReferenceEvent.Invoke(this);
        enemyStateManagerReference.TryRegisterObject(turretObject);
    }

    private void OnDisable() 
    {
        enemyStateManagerReference.TryUnregisterObject(turretObject);
    }
}
