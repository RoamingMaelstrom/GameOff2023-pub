using System;
using SOEvents;
using UnityEngine;

public class BoidController : MonoBehaviour
{
    [SerializeField] GameObjectSOEvent selectedBoidEvent;
    [SerializeField] BoidControllerBindings boidControllerBindings;
    [SerializeField] public Boid boidControlling;
    public Boid previouslyControlledBoid;

    [SerializeField] [Range(0.5f, 2.0f)] public float alignment = 1f;
    [SerializeField] [Range(0.5f, 2.0f)] public float cohesion = 1f;
    [SerializeField] [Range(0.5f, 2.0f)] public float separation = 1f;

    public bool useCircularFormation;
    public bool followSelectedObject;
    public GameObject selectedObject;
    public Vector3 centrePos;

    [SerializeField] [Range(-1f, 1f)] public float aggression = 0f;
    public bool lowHealthDronesNearCentre;
    public bool collapseOnCentre;
    public bool followPlayerByDefault;

    private void Awake() 
    {
        selectedBoidEvent.AddListener(SetBoidsCentreVisibility);
    }

    private void SetBoidsCentreVisibility(GameObject boidObject)
    {

        if (previouslyControlledBoid != null) previouslyControlledBoid.showCentre = false;
        if (boidControlling != null) boidControlling.showCentre = true;
        if (previouslyControlledBoid != boidControlling) GliderSFX.Play.RandomStandard("close_window1", "close_window2");
    }

    public void SetBoidControlling(Boid newBoid)
    {
        previouslyControlledBoid = boidControlling;
        if (newBoid == null)
        {
            boidControlling = null;
            return;
        }
        LoadBoidSettings(newBoid);
        boidControlling = newBoid;
    }

    private void LoadBoidSettings(Boid boid)
    {
        alignment = boid.alignmentMod;
        cohesion = boid.cohesionMod;
        separation = boid.separationMod;
        boid.cohesionDistanceCeiling = CalculateCohesionDistanceCeiling(boid, cohesion);
        boid.seperationCheckCounterMod = CalculateSeparationCheckCounterMod(separation);

        useCircularFormation = boid.circularMovement;
        lowHealthDronesNearCentre = boid.lowHealthNearCentre;

        aggression = boid.aggressionMultiplier;
        collapseOnCentre = boid.collapseToCentre;

        followSelectedObject = boid.followObject;
        selectedObject = boid.followingObject;
        followPlayerByDefault = boid.followPlayerByDefault;


        centrePos = boid.centreBias;

        boidControllerBindings.SetValuesOnBoidSwitch();
    }

    private void FixedUpdate() 
    {
        SetBoidControlsUIVisibility();
        if (boidControlling == null) return;
        if (boidControlling.numberOfDrones == 0)
        {
            boidControlling = null;
            return;
        }

        SetBoidForceModifiers();
        SetFormation();
        SetAggressionBehaviour();
        SetBoidCentre();
    }

    private void SetBoidControlsUIVisibility()
    {
        boidControllerBindings.SetPanelActive(boidControlling != null);
    }

    private void SetBoidCentre()
    {
        if (selectedObject != null)
        {   
            if (!selectedObject.activeInHierarchy) selectedObject = null;
        }

        boidControlling.followingObject = selectedObject;
        boidControlling.followObject = followSelectedObject;
        boidControlling.followPlayerByDefault = followPlayerByDefault;
        boidControlling.centreBias = centrePos;
    }

    private void SetAggressionBehaviour()
    {
        boidControlling.aggressionMultiplier = aggression;
        boidControlling.collapseToCentre = collapseOnCentre;
    }

    private void SetFormation()
    {
        boidControlling.circularMovement = useCircularFormation;
        boidControlling.lowHealthNearCentre = lowHealthDronesNearCentre;
    }

    private void SetBoidForceModifiers()
    {
        boidControlling.alignmentMod = alignment;
        boidControlling.cohesionMod = cohesion;
        boidControlling.cohesionDistanceCeiling = CalculateCohesionDistanceCeiling(boidControlling, cohesion);
        boidControlling.separationMod = separation;
        boidControlling.seperationCheckCounterMod = CalculateSeparationCheckCounterMod(separation);
    }

    // Todo: Should be able to remove these
    private static float CalculateCohesionDistanceCeiling(Boid boid, float cohesion) => (2.5f - cohesion) * Mathf.Pow(ScaleManager.ScaleSizeFactorGlobal, boid.scale - 1);

    private static int CalculateSeparationCheckCounterMod(float separation) => (int)((0.5f + separation) * 10);
}
