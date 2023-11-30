using UnityEngine;
using UnityEngine.InputSystem;
using SOEvents;
using System.Collections;
using System;

public class BasePlayerController : MonoBehaviour
{
    [SerializeField] SOEvent defeatEvent;
    [SerializeField] SOEvent victoryEvent;
    [SerializeField] SOEvent startScalingEvent;
    [SerializeField] SOEvent startScalingDelayedEvent;
    [SerializeField] SOEvent togglePauseScreenEvent;
    [SerializeField] BoolSOEvent updatePauseCounterEvent;
    [SerializeField] GameObject upgradeTabContent;

    [SerializeField] public float playerThrust = 10000f;
    [SerializeField] public float maxSpeed = 10f;
    [SerializeField] public float playerBoostMultiplier = 3f;

    [Header("To Implement")]
    [SerializeField] public float boostCostPerSecond = 1f;

    private Vector2 movement;
    private float boosting;
    public Vector2 mousePos;

    [SerializeField] DamageDealer playerDamageDealer;
    [SerializeField] public Rigidbody2D playerBody;
    [SerializeField] float scaleFactor = 4f;
    [SerializeField] float scalingAnimationDuration = 3f;

    [SerializeField] bool controlsEnabled = true;

    private void Awake() 
    {
        startScalingEvent.AddListener(StartScalingRoutine);
        startScalingDelayedEvent.AddListener(SwitchPlayerLayers);
        defeatEvent.AddListener(DisableControls);
        victoryEvent.AddListener(DisableControls);
    }

    private void DisableControls()
    {
        controlsEnabled = false;
    }

    private void SwitchPlayerLayers()
    {
        string newLayer = "scale1_player";
        switch (ScaleManager.PlayerScaleGlobal)
        {
            case 2: newLayer = "scale2_player"; break;
            case 3: newLayer = "scale3_player"; break;
            case 4: newLayer = "scale4_player"; break;
            case 5: newLayer = "scale5_player"; break;
        }

        playerBody.gameObject.layer = LayerMask.NameToLayer(newLayer);
        playerDamageDealer.gameObject.layer = LayerMask.NameToLayer(newLayer);
    }

    private void StartScalingRoutine()
    {
        StartCoroutine(ScalingAnimation());
    }

    private IEnumerator ScalingAnimation()
    {
        float additionalMaxSpeed = maxSpeed * (scaleFactor - 2f);
        float additionalThrust = playerThrust * (scaleFactor - 1f);
        float additionalScale = playerBody.transform.localScale.x * (scaleFactor - 1f);
        float additionalDamage = playerDamageDealer.dotDamageValue * (scaleFactor  / 2f);

        float rateOfChange = Time.fixedDeltaTime / scalingAnimationDuration;

        float timer = 0;
        while (timer < scalingAnimationDuration)
        {
            timer += Time.fixedDeltaTime;
            maxSpeed += additionalMaxSpeed * rateOfChange;
            playerThrust += additionalThrust * rateOfChange;
            playerBody.transform.localScale += Vector3.one * (additionalScale * rateOfChange);
            playerDamageDealer.dotDamageValue += additionalDamage * rateOfChange;
            yield return new WaitForFixedUpdate();
        }

    }

    public void FixedUpdate() 
    {
        if (!playerBody) return;
        HandleThrust();
    }

    void OnBoost(InputValue value) => boosting = 0;

    void OnMousePosition(InputValue value) => mousePos = value.Get<Vector2>();

    void OnMove(InputValue value) => movement = value.Get<Vector2>();

    void OnPause(InputValue value) 
    {
        if (!controlsEnabled) return;
        togglePauseScreenEvent.Invoke();
    }
    private void OnToggleUpgradeTab(InputValue value)
    {
        upgradeTabContent.SetActive(!upgradeTabContent.activeInHierarchy);
        updatePauseCounterEvent.Invoke(upgradeTabContent.activeInHierarchy);
    }

    // Todo: Add Boost consumes matter logic.

    public void AddUpgradeMaxSpeedModifier(float value) => maxSpeed += value * Mathf.Pow(ScaleManager.ScaleSizeFactorGlobal, ScaleManager.PlayerScaleGlobal - 1);
    public float GetMaxSpeed() => maxSpeed;

    private void HandleThrust()
    {
        float thrustMagnitude = (boosting == 0) ? playerThrust : playerThrust * playerBoostMultiplier;
        Vector2 force = movement.normalized * thrustMagnitude;

        playerBody.AddForce(force);
        playerBody.velocity = playerBody.velocity.normalized * Mathf.Min(playerBody.velocity.magnitude, GetMaxSpeed());
    }
}
