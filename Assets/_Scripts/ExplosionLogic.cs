using System.Collections.Generic;
using UnityEngine;

public class ExplosionLogic : MonoBehaviour
{
    [SerializeField] List<ParticleSystem> particleSystems;
    [SerializeField] float lifeSpan = 2f;
    [SerializeField] DamageDealer damageDealer;
    [SerializeField] float dDScale = 2f;

    public float timer = 0;

    private void Start() 
    {
        GliderSFX.Play.AtPoint("explosion1", transform.position);
        damageDealer.transform.localScale = Vector3.one * dDScale;

        foreach (var particles in particleSystems)
        {
            particles.Stop();
            particles.Play();
        }
    }

    private void FixedUpdate() 
    {
        timer += Time.fixedDeltaTime; 
        if (timer >= lifeSpan) Destroy(gameObject);
    }
}
