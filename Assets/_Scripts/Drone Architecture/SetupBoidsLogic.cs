using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetupBoidsLogic : MonoBehaviour
{
    [SerializeField] BoidContainer container;
    [SerializeField] Rigidbody2D playerbody;

    private void Start() 
    {
        foreach (var chain in container.boidChains)
        {   
            foreach (var boid in chain.GetAllBoids())
            {
                boid.followingObject = playerbody.gameObject;
                boid.followObject = true;
                boid.playerBody = playerbody;
            }
        }
    }
}
