using UnityEngine;
using SOEvents;

public class LifeSpanLimiter : MonoBehaviour
{
    // This should usually be despawnEvent that returns the object to the Object Pool.
    [SerializeField] public GameObjectFloatSOEvent onLifeEndEvent;
    [SerializeField] bool autoStart = true;
    public float startLifespan;
    public float spanRemaining;

    public bool running {get; private set;}

    private void Start() 
    {
        if (autoStart) StartCountdown(startLifespan);   
    }

    private void OnEnable() 
    {
        if (autoStart) StartCountdown(startLifespan);   
    }

    // Use this method to activate and make this class actually do something.
    public void StartCountdown(float startLifespanValue)
    {
        startLifespan = startLifespanValue;
        spanRemaining = startLifespan;
        running = true;
    }

    private void FixedUpdate() 
    {
        if (!running) return;

        spanRemaining -= Time.fixedDeltaTime;

        if (spanRemaining <= 0) 
        {
            running = false;
            onLifeEndEvent.Invoke(gameObject, 0);
        }
    }

    private void OnDisable() => running = false;

}
