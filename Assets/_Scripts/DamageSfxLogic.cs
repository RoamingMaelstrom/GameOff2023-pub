using System.Collections.Generic;
using SOEvents;
using UnityEngine;

public class DamageSfxLogic : MonoBehaviour
{
    [SerializeField] GameObjectFloatSOEvent droneDealtDamage;
    [SerializeField] GameObjectFloatSOEvent droneDamagedEvent;
    [SerializeField] GameObjectFloatSOEvent playerDealDamageEvent;
    [SerializeField] List<string> droneDealDamageSfx = new();
    [SerializeField] List<string> droneDamagedSfx = new();
    [SerializeField] List<string> playerDealDamageSfx = new();

    private void Awake() 
    {
        droneDealtDamage.AddListener(PlayDroneDealingDamageSfx);
        droneDamagedEvent.AddListener(PlayDroneDamagedSfx);
        playerDealDamageEvent.AddListener(PlayPlayerDealDamageSfx);
    }

    private void PlayDroneDamagedSfx(GameObject damagedDrone, float damageValue)
    {
        if (damageValue <= 300 && ScaleManager.PlayerScaleGlobal > 2) return;
        if (damageValue <= 500 && ScaleManager.PlayerScaleGlobal > 3) return;
        if (damageValue <= 750 && ScaleManager.PlayerScaleGlobal > 4) return;

        GliderSFX.Play.RandomAtPoint(damagedDrone.transform.position, droneDamagedSfx.ToArray());
    }

    private void PlayDroneDealingDamageSfx(GameObject damagedObstacle, float damageValue)
    {
        if (damageValue <= 25 && ScaleManager.PlayerScaleGlobal > 2) return;
        if (damageValue <= 75 && ScaleManager.PlayerScaleGlobal > 3) return;
        if (damageValue <= 200 && ScaleManager.PlayerScaleGlobal > 4) return;

        GliderSFX.Play.RandomAtPoint(damagedObstacle.transform.position, droneDealDamageSfx.ToArray());
    }


    private void PlayPlayerDealDamageSfx(GameObject arg0, float arg1)
    {
        GliderSFX.Play.RandomStandard(playerDealDamageSfx.ToArray());
    }


}
