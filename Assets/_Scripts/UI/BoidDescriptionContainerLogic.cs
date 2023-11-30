using SOEvents;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BoidDescriptionContainerLogic : MonoBehaviour
{
    [SerializeField] IntSOEvent requestPurchaseEvent;
    [SerializeField] BoidContainer boidContainer;
    [SerializeField] ScoreLogic scoreLogic;
    [SerializeField] int boidChainID;
    [SerializeField] string hotkey;
    [SerializeField] GameObject content;
    [SerializeField] Image droneColourImage;
    [SerializeField] TextMeshProUGUI droneHotkeyText;
    [SerializeField] TextMeshProUGUI droneNameText;
    [SerializeField] TextMeshProUGUI droneScaleText;
    [SerializeField] TextMeshProUGUI droneNumberText;
    [SerializeField] Button buyButton;
    [SerializeField] Button bulkBuyButton;

    [SerializeField] TextMeshProUGUI buyButtonText;
    [SerializeField] TextMeshProUGUI bulkBuyButtonText;

    BoidChain boidChain;
    Boid currentBoid;

    bool isShowing = true;
    
    private void Start() 
    {
        boidChain = boidContainer.GetBoidChainByChainID(boidChainID);
        droneColourImage.color = boidChain.boidColour;
    }

    private void FixedUpdate() 
    {
        if (boidChain.level == 0)
        {
            if (isShowing) content.gameObject.SetActive(false);
            isShowing = false;
            return;
        }

        if (!isShowing)
        {
            content.gameObject.SetActive(true);
            isShowing = true;
        }

        bool canBuy = true;
        bool canBulkBuy = true;

        currentBoid = boidChain.GetBoid();

        droneHotkeyText.SetText(string.Format("[{0}]", hotkey));
        droneNameText.SetText(boidChain.droneDisplayName);
        droneScaleText.SetText(string.Format("Scale\n{0}", currentBoid.scale));
        droneNumberText.SetText(string.Format("{0}/{1}", currentBoid.numberOfDrones, currentBoid.droneNumberCap));

        if (currentBoid.scale > ScaleManager.PlayerScaleGlobal)
        {
            canBuy = false;
            canBulkBuy = false;
        }

        DronePurchaseInfo info = boidChain.GetDronePurchaseInfo();

        buyButtonText.SetText(string.Format("Buy\n{0}", info.GetCost(currentBoid.numberOfDrones)));
        bulkBuyButtonText.SetText(string.Format("Buy {0}\n{1}", info.bulkBuySize, info.GetBulkCost(currentBoid.numberOfDrones)));

        if (!scoreLogic.CanAfford(info.GetCost(currentBoid.numberOfDrones))) canBuy = false;
        if (!scoreLogic.CanAfford(info.GetBulkCost(currentBoid.numberOfDrones))) canBulkBuy = false;

        if (currentBoid.numberOfDrones + 1 > currentBoid.droneNumberCap) canBuy = false;
        if (currentBoid.numberOfDrones + info.bulkBuySize > currentBoid.droneNumberCap) canBulkBuy = false;

        buyButton.interactable = canBuy;
        bulkBuyButton.interactable = canBulkBuy;
    }

    public void TryBuyDrone()
    {
        requestPurchaseEvent.Invoke(currentBoid.droneID);
    }

    public void TryBulkBuyDrones()
    {
        requestPurchaseEvent.Invoke(currentBoid.droneID + 5000);
    }
}
