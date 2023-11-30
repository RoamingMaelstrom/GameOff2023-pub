using TMPro;
using UnityEngine;
using SOEvents;
using UnityEngine.UI;
using System;

public class DronePanelLogic : MonoBehaviour
{
    [SerializeField] IntSOEvent requestPurchaseEvent;
    [SerializeField] IntSOEvent upgradeDroneEvent;
    [SerializeField] IntSOEvent boidUpgradedEvent;
    [SerializeField] ScoreLogic scoreLogic;
    [SerializeField] BoidContainer boidContainer;
    [SerializeField] int chainID;
    [SerializeField] Image boidColourImage;
    [SerializeField] TextMeshProUGUI droneNameText;
    [SerializeField] TextMeshProUGUI droneScaleText;
    [SerializeField] TextMeshProUGUI droneLevelText;
    [SerializeField] TextMeshProUGUI droneNumberCapText;
    [SerializeField] TextMeshProUGUI droneDescriptionText;
    [SerializeField] TextMeshProUGUI upgradeButtonText;
    [SerializeField] Button upgradeButton;

    [SerializeField] TextMeshProUGUI unableToPurchaseText;


    private BoidChain boidChain;
    private Boid currentBoid;

    private void Awake() 
    {
        boidUpgradedEvent.AddListener(CheckForUpgrade);
        upgradeDroneEvent.AddListener(UpdateButtonClickable);
        requestPurchaseEvent.AddListener(UpdateButtonClickable);
    }

    private void UpdateButtonClickable(int arg0)
    {
        if (boidChain == null) 
        {
            upgradeButton.interactable = false;
            return;
        }

        bool buttonClickable = boidChain.level >= boidChain.upgradeCostList.Count ? SetButtonClickable(boidChain.upgradeCostList[0], 100) : SetButtonClickable(boidChain.upgradeCostList[boidChain.level], boidChain.boidList[boidChain.level].scale);

        SetUnableToPurchaseText(buttonClickable);
    }

    private void OnEnable() 
    {
        boidChain = boidContainer.GetBoidChainByChainID(chainID);
        currentBoid = boidChain.GetBoid();
        boidColourImage.color = boidChain.boidColour;
        if (currentBoid == null) UpdateFieldsNotUnlocked();
        else UpdateFieldsUnlocked();
    }

    private void UpdateFieldsNotUnlocked()
    {
        droneNameText.SetText(boidChain.droneDisplayName);
        droneScaleText.SetText(string.Format("Scale: {0}-{1}", boidChain.boidList[0].scale, boidChain.boidList[2].scale));
        droneLevelText.SetText(string.Format(""));
        droneNumberCapText.SetText(string.Format("Drone Cap: {0}", boidChain.boidList[0].droneNumberCap));
        droneDescriptionText.SetText(boidChain.droneDescription);

        upgradeButtonText.SetText(string.Format("Unlock {0}", boidChain.upgradeCostList[boidChain.level]));
        bool buttonClickable = SetButtonClickable(boidChain.upgradeCostList[boidChain.level], boidChain.boidList[boidChain.level].scale);
        SetUnableToPurchaseText(buttonClickable);
    }

    private void SetUnableToPurchaseText(bool canPurchase)
    {
        if (canPurchase || boidChain.IsMaxLevel())
        {
            unableToPurchaseText.SetText("");
            return;
        }

        if (ScaleManager.PlayerScaleGlobal < boidChain.boidList[boidChain.level].scale)
        {
            unableToPurchaseText.SetText("Scale too Low");
            return;
        }

        if (!scoreLogic.CanAfford(boidChain.upgradeCostList[boidChain.level]))
        {
            unableToPurchaseText.SetText("Cannot afford");
            return;
        }

        unableToPurchaseText.SetText("");
    }

    private bool SetButtonClickable(int cost, int scaleToPress)
    {
        if (boidChain.IsMaxLevel()) return upgradeButton.interactable = false;
        if (!scoreLogic.CanAfford(cost)) return upgradeButton.interactable = false;
        if (ScaleManager.PlayerScaleGlobal < scaleToPress) return upgradeButton.interactable = false;
        return upgradeButton.interactable = true;
    }

    private void CheckForUpgrade(int upgradedChainID)
    {
        if (chainID != upgradedChainID) return;
        //level++;
        //currentScale++;
        currentBoid = boidChain.GetBoid();
        if (currentBoid == null) UpdateFieldsNotUnlocked();
        else UpdateFieldsUnlocked();
    }

    private void UpdateFieldsUnlocked()
    {
        upgradeButton.interactable = true;

        droneNameText.SetText(boidChain.droneDisplayName);
        droneScaleText.SetText(string.Format("Scale: {0}", currentBoid.scale));
        droneLevelText.SetText(string.Format("Level: {0}/3", boidChain.level));
        droneNumberCapText.SetText(string.Format("Drones: {0}/{1}", currentBoid.numberOfDrones, currentBoid.droneNumberCap));
        droneDescriptionText.SetText(boidChain.droneDescription);

        if (!boidChain.IsMaxLevel()) upgradeButtonText.SetText(string.Format("Scale {0}", boidChain.upgradeCostList[boidChain.level]));
        else upgradeButtonText.SetText("Max Level");

        bool buttonClickable = SetButtonClickable(boidChain.upgradeCostList[Mathf.Min(boidChain.level, 2)], currentBoid.scale + 1);
        SetUnableToPurchaseText(buttonClickable);
    }
}
