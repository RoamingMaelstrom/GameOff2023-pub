using UnityEngine;

public class UpdateFirstTimeLogic : MonoBehaviour
{
    [SerializeField] SaveObject firstTimeSaveObject;
    [SerializeField] SaveObject fromMainMenuSaveObject;

    private void Start() 
    {
        firstTimeSaveObject.Set(firstTimeSaveObject.GetValueInt() + 1);
        fromMainMenuSaveObject.Set(1);
    }
}
