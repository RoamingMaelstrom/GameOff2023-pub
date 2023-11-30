using System;
using System.Collections;
using System.Collections.Generic;
using SOEvents;
using TMPro;
using UnityEngine;

public class BoidSelectedTextLogic : MonoBehaviour
{
    [SerializeField] GameObjectSOEvent selectedBoidEvent;
    [SerializeField] BoidContainer boidContainer;
    [SerializeField] TextMeshProUGUI boidSelectedText;


    private void Awake() 
    {
        selectedBoidEvent.AddListener(ChangeText);
        ChangeText(null);
    }

    private void ChangeText(GameObject boidObject)
    {
        if (boidObject == null)
        {   
            boidSelectedText.SetText("");
            return;
        }

        BoidChain chain = boidContainer.GetBoidChainContaining(boidObject.GetComponent<Boid>().droneID);

        boidSelectedText.SetText(string.Format("[{0}] {1}", chain.chainID, chain.droneDisplayName));
        boidSelectedText.color = chain.boidColour;
    }
}
