using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowFirstTimeFlashingLogic : MonoBehaviour
{
    [SerializeField] SaveObject firstTimeSaveObject;
    [SerializeField] SaveObject fromMainMenuSaveObject;
    [SerializeField] GameObject glowingEffectObject;



    private void Awake()
    {
        glowingEffectObject.SetActive(firstTimeSaveObject.GetValueInt() <= 0);
        fromMainMenuSaveObject.Set(0);
    }
}
