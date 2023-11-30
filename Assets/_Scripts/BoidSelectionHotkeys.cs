using UnityEngine;
using UnityEngine.InputSystem;
using SOEvents;
using System;

public class BoidSelectionHotkeys : MonoBehaviour
{
    [SerializeField] SOEvent defeatEvent;
    [SerializeField] SOEvent victoryEvent;
    [SerializeField] GameObjectSOEvent selectedBoidEvent;
    [SerializeField] BoidContainer boidContainer;
    [SerializeField] BoidController boidController;

    [SerializeField] bool controlsEnabled = true;

    private void Awake() 
    {
        defeatEvent.AddListener(DisableControls);
        victoryEvent.AddListener(DisableControls);
    }

    private void DisableControls()
    {
        controlsEnabled = false;
    }

    private void OnSelectDrone1(InputValue value)
    {
        if (value.Get<float>() != 1) return;
        MainSelectionLogic(1);
    }

    private void OnSelectDrone2(InputValue value)
    {
        if (value.Get<float>() != 1) return;
        MainSelectionLogic(2);
    }

    private void OnSelectDrone3(InputValue value)
    {
        if (value.Get<float>() != 1) return;
        MainSelectionLogic(3);
    }

    private void OnSelectDrone4(InputValue value)
    {
        if (value.Get<float>() != 1) return;
        MainSelectionLogic(4);
    }

    private void OnSelectDrone5(InputValue value)
    {
        if (value.Get<float>() != 1) return;
        MainSelectionLogic(5);
    }

    private void OnSelectDrone6(InputValue value)
    {
        if (value.Get<float>() != 1) return;
        MainSelectionLogic(6);
    }


    public void MainSelectionLogic(int chainID)
    {
        if (!controlsEnabled) return;
        BoidChain boidChain = boidContainer.GetBoidChainByChainID(chainID);
        Boid selectedBoid = boidChain.GetBoid();

        if (selectedBoid == null) return;
        if (selectedBoid.numberOfDrones == 0) return;

        if (boidController.boidControlling != null)
        {
            if (boidController.boidControlling.droneID == selectedBoid.droneID)
            {
                boidController.SetBoidControlling(null);
                selectedBoidEvent.Invoke(null);
                return;
            }
        }

        boidController.SetBoidControlling(selectedBoid);
        selectedBoidEvent.Invoke(selectedBoid.gameObject);
    }

}
