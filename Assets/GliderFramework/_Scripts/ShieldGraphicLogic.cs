using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SOEvents;

public class ShieldGraphicLogic : MonoBehaviour
{
    [SerializeField] Health health;
    [SerializeField] GameObject shieldGraphicPrefab;

    [Header("Shield Opacity Variables")]
    [SerializeField] float baseDecayRate = 0.4f;
    [SerializeField] float shieldDownDecayRate = 0.8f;
    [SerializeField] float shieldUpIncreaseRate = 3.2f;
    [SerializeField] float opacityPercentMultiplierOnDamaged = 4f;
    [SerializeField] float maxOpacity = 0.8f;
    [SerializeField] float minOpacity = 0.1f;

    [Header("SFX Clip Names")]
    [SerializeField] string shieldDownClipName;
    [SerializeField] string shieldUpClipName;

    private SpriteRenderer shieldSpriteRenderer;
    private float previousShieldHp;

    private void Start() 
    {
        GameObject shieldSpriteObject = Instantiate(shieldGraphicPrefab, Vector2.zero, Quaternion.Euler(0, 0, 90), health.transform);
        shieldSpriteRenderer = shieldSpriteObject.GetComponent<SpriteRenderer>();
        shieldSpriteRenderer.color = shieldSpriteRenderer.color + new Color(0, 0, 0, 1);
        previousShieldHp = health.GetCurrentShieldHp();
    }

    private void Update()
    {
        shieldSpriteRenderer.transform.localPosition = Vector3.zero;
    }

    private void FixedUpdate() 
    {
        if (health.shieldHpMax > 0) ShowShieldGraphic(Time.fixedDeltaTime);
        else DeactivateShieldGraphic();
    }

    private void ShowShieldGraphic(float deltaTime)
    {
        shieldSpriteRenderer.gameObject.SetActive(true);
        float currentShieldHp = health.GetCurrentShieldHp();
        Color newColour = shieldSpriteRenderer.color;

        float decayRate = GetDecayRate(currentShieldHp, deltaTime);
        float increaseOnDamaged = GetOpacityIncreaseFromDamage(currentShieldHp);

        if (previousShieldHp > 0 && currentShieldHp <= 0) GliderSFX.Play.RelativeToTransform(shieldDownClipName, transform);
        if (previousShieldHp <= 0 && currentShieldHp > 0) StartCoroutine(ShieldUp());

        newColour.a = Mathf.Clamp(newColour.a + increaseOnDamaged - decayRate, minOpacity, maxOpacity);
        shieldSpriteRenderer.color = newColour;

        previousShieldHp = currentShieldHp;
    }

    private float GetDecayRate(float currentShieldHp, float deltaTime)
    {
        float decayRate = baseDecayRate;
        if (currentShieldHp <= 0) decayRate += shieldDownDecayRate;

        decayRate *= deltaTime;
        return decayRate;
    }

    private float GetOpacityIncreaseFromDamage(float currentShieldHp)
    {
        if (previousShieldHp <= currentShieldHp) return 0;

        float shieldPercent = currentShieldHp / health.shieldHpMax;
        float percentDamage = (previousShieldHp - currentShieldHp) / health.shieldHpMax;
        return (2 - shieldPercent) * percentDamage *  opacityPercentMultiplierOnDamaged;
    }

    private void DeactivateShieldGraphic() => shieldSpriteRenderer.gameObject.SetActive(false);

    private IEnumerator ShieldUp()
    {
        GliderSFX.Play.RelativeToTransform(shieldUpClipName, transform);

        float counter = 0;
        float dt;

        while (shieldSpriteRenderer.color.a < maxOpacity && counter < 2f)
        {
            dt = Time.fixedDeltaTime;
            counter += dt;

            Color colour = shieldSpriteRenderer.color;
            colour.a += shieldUpIncreaseRate * dt;
            shieldSpriteRenderer.color = colour;
            yield return new WaitForSeconds(dt);
        }

        counter = 0;

        while (counter < 0.5f && shieldSpriteRenderer.color.a > 0.2f)
        {
            dt = Time.fixedDeltaTime;
            counter += dt;

            Color colour = shieldSpriteRenderer.color;
            colour.a -= baseDecayRate * dt;
            shieldSpriteRenderer.color = colour;
            yield return new WaitForSeconds(dt);
        }
    }
}