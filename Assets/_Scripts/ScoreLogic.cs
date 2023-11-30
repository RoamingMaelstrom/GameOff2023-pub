using System;
using System.Collections.Generic;
using SOEvents;
using UnityEngine;

public class ScoreLogic : MonoBehaviour
{
    [SerializeField] SOEvent defeatEvent;
    [SerializeField] SOEvent victoryEvent;
    [SerializeField] GameObjectFloatSOEvent playerTakeDamageEvent;
    [SerializeField] GameObjectFloatSOEvent pickupDropEvent;
    [SerializeField] float totalScore = 0;
    [SerializeField] float matter = 1000;
    [SerializeField] Health playerHealth;
    public float timer = 0;

    [SerializeField] List<string> playerDamagedSfx;
    [SerializeField] int maxScore = 20000000;

    bool isCounting = true;

    private void Awake() 
    {
        pickupDropEvent.AddListener(AddMatter);
        playerTakeDamageEvent.AddListener(SubtractMatterOnly);
        playerTakeDamageEvent.AddListener(PlayDamagedSfx);

        defeatEvent.AddListener(StopCounting);
        victoryEvent.AddListener(VictoryLogic);

        playerHealth.maxHp = maxScore;
        playerHealth.ManualSetCurrentHp(matter);
    }

    private void VictoryLogic()
    {
        totalScore = maxScore;
        StopCounting();
    }

    private void StopCounting()
    {
        isCounting = false;
    }

    private void PlayDamagedSfx(GameObject arg0, float damageValue)
    {
        if (damageValue / matter < 0.005f) return;
        GliderSFX.Play.RandomStandard(playerDamagedSfx.ToArray());
    }

    private void FixedUpdate() 
    {
        if (isCounting) timer += Time.fixedDeltaTime;
    }

    public bool CanAfford(int value)
    {
        return matter >= value;
    }

    public float GetTotalScore() => totalScore;
    public float GetCurrentMatter() => matter;

    public void AddMatter(float value)
    {
        if (!isCounting) return;
        matter += value;
        if (ScaleManager.PlayerScaleGlobal >= 4) matter += value;
        totalScore += value;
        totalScore = Mathf.Min(totalScore, maxScore);
        playerHealth.ManualSetCurrentHp(matter);
    }

    public void AddMatterNotScore(float value)
    {
        matter += value;
        playerHealth.ManualSetCurrentHp(matter);
    }

    private void AddMatter(GameObject arg0, float value) => AddMatter(value);

    public void SubtractMatter(float value)
    {
        if (!isCounting) return;
        matter -= value;
        playerHealth.ManualSetCurrentHp(matter);
    }

    private void SubtractMatterOnly(GameObject arg0, float value) => matter -= value;
}
