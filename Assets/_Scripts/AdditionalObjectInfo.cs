using UnityEngine;

public class AdditionalObjectInfo : MonoBehaviour
{
    public int scale;
    [SerializeField] float baseMatter;
    public float matter;
    [SerializeField] float minSize;
    [SerializeField] float maxSize;
    public float size;
    public float minSpeed;
    public float maxSpeed;
    public float baseHealth;
    public bool hasStartingTorque;
    public DeathParticleType deathParticleType;

    public void GenerateRandomSize()
    {
        size = Random.Range(minSize, maxSize);
        float matterMultiplier = size / minSize;
        matter = baseMatter * matterMultiplier * matterMultiplier;
    }

    public float GetHealthValue() => baseHealth * size * size / (minSize * minSize);
    public float GetMatterValue() => baseMatter * scale * size * size / (minSize * minSize);
    public float GetRandomSpeed() => Random.Range(minSpeed, maxSpeed);
}


public enum DeathParticleType
{
    NONE, 
    ASTEROID_BROWN,
    ASTEROID_RED,
    ASTEROID_GREY,
    DEBRIS, 
    ENEMY_BLUE,
    ENEMY_RED,
    SMOKE
}
