using UnityEngine;
using UnityEngine.UI;
using SOEvents;

public class TargetHealthBarLogic : MonoBehaviour
{
    [SerializeField] GameObjectSOEvent playerStartTargetingEvent;
    [SerializeField] SOEvent playerStopTargetingEvent;

    [SerializeField] Image hpBackground;
    [SerializeField] Color hpBackgroundColour = new Color(1, 1, 1, 1);
    [SerializeField] Image hpFill;
    [SerializeField] Color hpFillColour = new Color(0, 1, 1, 1);
    [SerializeField] float hpOffset;


    [SerializeField] Image shieldBackground;
    [SerializeField] Color shieldBackgroundColour = new Color(1, 0, 1, 1);
    [SerializeField] Image shieldFill;
    [SerializeField] Color shieldFillColour = new Color(0, 0, 1, 1);
    [SerializeField] float shieldOffset;

    GameObject target;

    private void Awake() 
    {

        playerStartTargetingEvent.AddListener(PlayerStartTargetingEventHandler);
        playerStopTargetingEvent.AddListener(PlayerStopTargetingEventHandler);

        hpBackground.color = hpBackgroundColour;
        hpFill.color = hpFillColour;

        shieldBackground.color = shieldBackgroundColour;
        shieldFill.color = shieldFillColour;
    }


    private void LateUpdate() 
    {
        if (!target)
        {
            DeactivateHealthBar();
            DeactivateShieldBar();
            return;
        }

        if (target.TryGetComponent(out Health targetHealth))
        {
            UpdateHealthBarFill(targetHealth);

            if (targetHealth.shieldHpMax <= 0) 
            {
                DeactivateShieldBar();
                UpdateBarsPosition(hpOffset);
                return;
            }

            UpdateShieldBarFill(targetHealth);
            UpdateBarsPosition(shieldOffset);
        }
    }

    void DeactivateHealthBar()
    {
        hpBackground.gameObject.SetActive(false);
        hpFill.gameObject.SetActive(false);
    }

    void DeactivateShieldBar()
    {
        shieldBackground.gameObject.SetActive(false);
        shieldFill.gameObject.SetActive(false);
    }

    private void PlayerStartTargetingEventHandler(GameObject newTarget) => target = newTarget;
    private void PlayerStopTargetingEventHandler() => target = null;

    public void UpdateHealthBarFill(Health targetHealth)
    {
        hpBackground.gameObject.SetActive(true);
        hpFill.gameObject.SetActive(true);
        float fillAmount = targetHealth.GetCurrentHp() / targetHealth.maxHp;
        hpFill.fillAmount = fillAmount;   
    }


    private void UpdateBarsPosition(float offsetValue)
    {
        float rotation = Camera.main.transform.rotation.eulerAngles.z;

        transform.localRotation = Quaternion.Euler(0, 0, rotation);
        transform.position = target.transform.position + new Vector3(Mathf.Sin(-rotation * Mathf.Deg2Rad) * offsetValue,
         Mathf.Cos(-rotation * Mathf.Deg2Rad) * offsetValue, 0);
    }

    private void UpdateShieldBarFill(Health targetHealth)
    {
        shieldBackground.gameObject.SetActive(true);
        shieldFill.gameObject.SetActive(true);
        float fillAmount = targetHealth.GetCurrentShieldHp() / targetHealth.shieldHpMax;
        shieldFill.fillAmount = fillAmount;   
    }
}
