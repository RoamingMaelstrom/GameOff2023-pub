using UnityEngine;
using UnityEngine.InputSystem;
using SOEvents;
using UnityEngine.EventSystems;
using System.Collections;

public class PlayerFiring : MonoBehaviour
{
    [SerializeField] SOEvent playerFireWeaponsEvent;
    float isFiring;

    void OnLMB(InputValue value)
    {
        isFiring = value.Get<float>();
        StartCoroutine(CheckIfMouseClickedUIDelayed());
    }

    IEnumerator CheckIfMouseClickedUIDelayed()
    {
        yield return new WaitForEndOfFrame();
        if (EventSystem.current.IsPointerOverGameObject()) isFiring = 0f;
        yield return null;
    }

    private void FixedUpdate() => HandleFiring();

    private void HandleFiring()
    {
        if (isFiring == 0) return;
        Debug.Log("Pew Pew...");
        playerFireWeaponsEvent.Invoke();
    }


}



