using SOEvents;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class DamageTextManager : MonoBehaviour
{
    [SerializeField] GameObjectFloatSOEvent obstacleTakeDamageEvent;
    [SerializeField] GameObjectFloatSOEvent playerTakeDamageEvent;
    [SerializeField] GameObjectFloatSOEvent droneTakeDamageEvent;
    [SerializeField] ObjectPoolMain objectPoolMain;
    [SerializeField] float baseDamageSize = 0.01f;
    [SerializeField] float baseVelocity;
    [SerializeField] Canvas textCanvas;

    [SerializeField] int obstacleDamageTextID = 21000;
    [SerializeField] int playerDamageTextID = 21001;

    public float damageCache;
    public int cacheCount = 0;
    public int cacheLimit = 4;
    public float cacheTimer = 0;
    public float cacheTimeLimit = 0.06f;

    public GameObject lastObstacle;

    private void Awake() 
    {
        obstacleTakeDamageEvent.AddListener(OnObstacleDamaged);
        playerTakeDamageEvent.AddListener(SpawnPlayerDamagedText);
        droneTakeDamageEvent.AddListener(SpawnPlayerDamagedText);
    }

    private void FixedUpdate() 
    {
        cacheTimer += Time.fixedDeltaTime;
        if (cacheTimer >= cacheTimeLimit || cacheCount >= cacheLimit)
        {
            if (damageCache != 0 && lastObstacle != null) SpawnDamageText(lastObstacle, damageCache);
            ResetDamageCache();
        }
    }

    private void ResetDamageCache()
    {
        damageCache = 0;
        cacheCount = 0;
        cacheTimer = 0;
    }

    private void OnObstacleDamaged(GameObject damagedObstacle, float damageValue)
    {   
        if (damagedObstacle != lastObstacle && lastObstacle != null && damageCache > 0) 
        {
            SpawnDamageText(lastObstacle, damageCache);
            ResetDamageCache();
        }

        lastObstacle = damagedObstacle;
        cacheCount ++;
        damageCache += damageValue;
    }


    private void SpawnPlayerDamagedText(GameObject damagedPlayerControlled, float damageValue)
    {
        GameObject damageText = objectPoolMain.GetObject(playerDamageTextID);
        damageText.transform.SetParent(textCanvas.transform, false);
        damageText.transform.position = damagedPlayerControlled.transform.position;

        float fontSize = baseDamageSize * ScaleManager.PlayerScaleGlobal;
        switch (damageValue)
        {
            case <= 200: break;
            case < 800: fontSize *= 4; break;
            case < 2400: fontSize *= 8; break;
            default: fontSize *= 24; break;
        }

        TextMeshProUGUI textMesh = damageText.GetComponent<TextMeshProUGUI>();
        textMesh.text = ((int)damageValue).ToString();
        textMesh.fontSize = fontSize;
        damageText.GetComponent<Rigidbody2D>().velocity = Random.insideUnitCircle.normalized * baseVelocity * ScaleManager.PlayerScaleGlobal;
    }


    private void SpawnDamageText(GameObject damagedObstacle, float damageValue)
    {
        GameObject damageText = objectPoolMain.GetObject(obstacleDamageTextID);
        damageText.transform.SetParent(textCanvas.transform, false);
        damageText.transform.position = damagedObstacle.transform.position;

        float fontSize = baseDamageSize * ScaleManager.PlayerScaleGlobal;
        switch (damageValue)
        {
            case < 20: break;
            case < 40: fontSize *= 1.5f; break;
            case < 70: fontSize *= 2; break;
            case < 100: fontSize *= 3; break;
            case < 400: fontSize *= 4; break;
            case < 600: fontSize *= 6; break;
            case < 800: fontSize *= 8; break;
            default: fontSize *= 16; break;
        }

        TextMeshProUGUI textMesh = damageText.GetComponent<TextMeshProUGUI>();
        textMesh.text = ((int)damageValue).ToString();
        textMesh.fontSize = fontSize;
        damageText.GetComponent<Rigidbody2D>().velocity = Random.insideUnitCircle.normalized * baseVelocity * 4f * ScaleManager.PlayerScaleGlobal;
    }
}
