using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerInformationTextLogic : MonoBehaviour
{
    [SerializeField] BasePlayerController basePlayerController;
    [SerializeField] CircleCollider2D playerPickupCollider;
    [SerializeField] Health playerHealth;
    [SerializeField] DamageDealer playerDamageDealer;

    [SerializeField] TextMeshProUGUI playerDamageIntervalText;
    [SerializeField] TextMeshProUGUI playerDpsText;
    [SerializeField] TextMeshProUGUI playerDamageReductionText;
    [SerializeField] TextMeshProUGUI playerPickupRadiusText;
    [SerializeField] TextMeshProUGUI playerSpeedText;
    [SerializeField] TextMeshProUGUI playerScaleText;

    private void Update() 
    {
        playerDamageIntervalText.SetText(string.Format("Damage Interval: {0:n2}s", playerDamageDealer.dotInterval));
        playerDpsText.SetText(string.Format("Damage per Second (DPS): {0:n1}", playerDamageDealer.dotDamageValue / playerDamageDealer.dotInterval));
        playerDamageReductionText.SetText(string.Format("Damage Taken : {0:n1}%", playerHealth.damageTakeMultiplier * 100f));
        playerPickupRadiusText.SetText(string.Format("Pickup Radius : {0:n0}km", playerPickupCollider.radius * ScaleManager.PlayerScaleGlobal * ScaleManager.ScaleSizeFactorGlobal));
        playerSpeedText.SetText(string.Format("Speed : {0:n1}km/s", basePlayerController.GetMaxSpeed() * 10f));
        playerScaleText.SetText(string.Format("Scale : {0}", ScaleManager.PlayerScaleGlobal));
    }
}
