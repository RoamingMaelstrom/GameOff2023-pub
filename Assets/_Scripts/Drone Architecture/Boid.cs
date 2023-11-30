using System.Collections.Generic;
using SOEvents;
using UnityEngine;

public class Boid : MonoBehaviour
{
    public int droneID;
    public int scale;
    public int droneNumberCap = 25;
    [SerializeField] GameObjectSOEvent returnToPoolEvent;
    [SerializeField] GameObjectSOEvent fireWeaponEvent;
    [SerializeField] SpriteRenderer centreGraphic;
    [SerializeField] public SpriteRenderer pointMarkGraphic;
    [SerializeField] CircleCollider2D boidCentreCollider;
    public List<Collider2D> collidersInRange = new();
    public List<EnemyAI> enemiesInRange = new();

    public List<Rigidbody2D> droneList = new();
    public Vector2 centre;
    public Vector2 averageRotationVector;
    public float averageRotation;

    public float separationMod = 0.5f;
    public float alignmentMod = 1f;
    public float cohesionMod = 1.5f;
    public float baseForceMag = 100f;

    public int numberOfDrones;
    public float maxSpeed = 10;
    [Range(1, 100)] public float cohesionDistanceCeiling = 10f;
    [Range(1, 100)] public int seperationCheckCounterMod = 10;

    [SerializeField] public Vector2 centreBias;
    [SerializeField] [Range(0f, 1f)] float centreBiasMag = 0.75f;

    List<Vector2> storedDirToNearest = new();
    List<float> storedMaxSpeedMultiplier = new();

    int altSeparationCounter = 0;
    public bool circularMovement = false;
    public bool collapseToCentre = false;
    [Range(-1f, 1f)] public float aggressionMultiplier = 0;

    public bool lowHealthNearCentre = false;

    public bool altRotation = false;

    public bool followObject = false;
    public GameObject followingObject;
    public bool showCentre = false;

    [SerializeField] BoidRangedTargeting boidRangedTargeting;
    public BoidFiringInfo boidFiringInfo;
    [SerializeField] float firingProbability = 0f;
    public List<AdditionalObjectInfo> targetsForRangedDrones = new();
    public Vector3 rangedTargetPos;
    public Rigidbody2D playerBody;
    public bool followPlayerByDefault;

    public bool scaleDistanceEnabled = true;

    private void Start() 
    {
        UpdateCentreGraphic();
        if (boidFiringInfo.weaponType != WeaponType.NONE) boidRangedTargeting.gameObject.SetActive(true);

        boidCentreCollider.radius = 1.5f * Mathf.Pow(ScaleManager.ScaleSizeFactorGlobal, scale - 1);

        cohesionDistanceCeiling = CalculateCohesionDistanceCeiling(this, cohesionMod);
        seperationCheckCounterMod = CalculateSeparationCheckCounterMod(separationMod);
    }

    void FixedUpdate()
    {
        UpdateCentreGraphic();
        UpdateDetectors();

        altSeparationCounter ++;
        if (altSeparationCounter % 60 == 0) altRotation = !altRotation;

        if (numberOfDrones == 0) return;
        if (followingObject != null)
        {
            if (!followingObject.activeInHierarchy) followingObject = null;
        }

        centre = CalculateCentre();
        averageRotationVector = CalculateAverageRotationVector();

        bool obstacleCloseToCentre = ColliderCloseToCentre(centre, cohesionDistanceCeiling, collidersInRange);
        


        if (followPlayerByDefault && followObject && followingObject == null) followingObject = playerBody.gameObject;

        for (int i = 0; i < droneList.Count; i++)
        {    
            Rigidbody2D boid =  droneList[i];

            Vector2 deltaCentre = (Vector3)centre - boid.transform.position;
            float workingDistanceCeiling = (lowHealthNearCentre && BoidIsLowHealth(boid)) ? cohesionDistanceCeiling / 2f : cohesionDistanceCeiling;
            bool insideCohesionBounds = deltaCentre.sqrMagnitude < workingDistanceCeiling * workingDistanceCeiling;

            if (collapseToCentre && obstacleCloseToCentre && Mathf.Abs(deltaCentre.sqrMagnitude / (cohesionDistanceCeiling * cohesionDistanceCeiling)) > 0.1f) insideCohesionBounds = false;

            Vector2 toNearestBoid = insideCohesionBounds ? CalculateNearestBoidVector(i) : deltaCentre.normalized; 
            Vector2 rotationDirVector = insideCohesionBounds ? CalculateRotationDirVector(i, averageRotationVector) : deltaCentre.normalized;
            if (altRotation) rotationDirVector = insideCohesionBounds ? GetLocalRotationDirVector(boid) : deltaCentre.normalized;

            if (circularMovement && !(collapseToCentre && obstacleCloseToCentre)) rotationDirVector = Vector2.Perpendicular(deltaCentre).normalized;

            float lowHealthCohesionMod = BoidIsLowHealth(boid) ? 2f : 1f;

            float sumWeights = separationMod + (cohesionMod * lowHealthCohesionMod) + alignmentMod;
            float alignmentForce = alignmentMod * baseForceMag / sumWeights;
            float cohesionForce = cohesionMod * lowHealthCohesionMod * baseForceMag / sumWeights;
            float separationForce = separationMod * baseForceMag / sumWeights;

            // Alignment
            boid.AddForce(rotationDirVector * alignmentForce);
            // Cohesion
            boid.AddForce(deltaCentre.normalized * cohesionForce);
            // Seperation
            boid.AddForce(toNearestBoid * separationForce);

            if (i + altSeparationCounter % 50 == 0) storedMaxSpeedMultiplier[i] = Random.Range(0.5f, 1.5f);

            boid.velocity = boid.velocity.normalized * Mathf.Min(boid.velocity.magnitude, maxSpeed);

            boid.transform.rotation = UpdateBoidRotation(boid);
        }

        TryFire();
        UpdateDetectors();
    }

    private static float CalculateCohesionDistanceCeiling(Boid boid, float cohesion) 
    {
        float output = (2.5f - cohesion) * Mathf.Pow(ScaleManager.ScaleSizeFactorGlobal, boid.scale - 1);
        return output;
    }

    private static int CalculateSeparationCheckCounterMod(float separation) => (int)((0.5f + separation) * 10);

    private void TryFire()
    {
        if (boidFiringInfo.weaponType == WeaponType.NONE) return;
        if (targetsForRangedDrones.Count == 0) return;

        rangedTargetPos = GetTargetForRangedDrones();

        firingProbability += droneList.Count * Time.fixedDeltaTime * boidFiringInfo.fireRatePerDrone;
        int counter = 0;
        do
        {
            counter ++;
            if (Random.Range(0, 1f) < firingProbability)
            {
                firingProbability --;   
                fireWeaponEvent.Invoke(gameObject);
            }
            else break;
        } while (firingProbability >= 1f && counter < 25);

    }

    private Vector3 GetTargetForRangedDrones()
    {
        float nearestSqr = Mathf.Infinity;
        Vector3 nearest = targetsForRangedDrones[0].transform.position;
        int nearestScaleDistance = Mathf.Abs(targetsForRangedDrones[0].scale - scale);

        foreach (var enemy in enemiesInRange)
        {
            float distanceSqr = ((Vector2)enemy.transform.position - centre).sqrMagnitude;
            if (distanceSqr < nearestSqr)
            {
                nearest = enemy.transform.position;
                nearestSqr = distanceSqr;
            }
        }

        if (enemiesInRange.Count > 0) return nearest;

        foreach (var objectInfo in targetsForRangedDrones)
        {
            int scaleDistance = Mathf.Abs(objectInfo.scale - nearestScaleDistance);
            if (!scaleDistanceEnabled) scaleDistance = 0;

            float distanceSqr = ((Vector2)objectInfo.transform.position - centre).sqrMagnitude;
            if (distanceSqr < nearestSqr || (scaleDistance < nearestScaleDistance && Random.Range(0f, 1f) < 0.5f))
            {
                nearest = objectInfo.transform.position;
                nearestSqr =  distanceSqr;
                nearestScaleDistance = scaleDistance;
            }
        }
        return nearest;
    }

    private static bool ColliderCloseToCentre(Vector3 centre, float maxDistance, List<Collider2D> collidersInRange)
    {
        float maxDistanceSqr = maxDistance * maxDistance;
        foreach (var collider in collidersInRange)
        {
            if ((collider.transform.position - centre).sqrMagnitude < maxDistanceSqr / 4f) return true;
        }
        return false;
    }

    private bool BoidIsLowHealth(Rigidbody2D boid)
    {
        Health boidHealth = boid.GetComponent<Health>();
        return boidHealth.GetCurrentHp() / boidHealth.maxHp < 0.333f;
    }

    private Vector2 GetLocalRotationDirVector(Rigidbody2D boid)
    {
        Vector2 averageVector = Vector2.zero;
        int count = 0;

        for (int i = 0; i < droneList.Count; i++)
        {
            if ((droneList[i].transform.position - boid.transform.position).magnitude < cohesionDistanceCeiling / 4f) 
            {
                averageVector += droneList[i].GetComponent<Rigidbody2D>().velocity.normalized; 
                count ++;
            }
        }
        return count == 0 ? boid.velocity.normalized : (averageVector / count).normalized;
    }

    private void UpdateDetectors()
    {
        boidCentreCollider.transform.position = centreBias;
        boidRangedTargeting.transform.position = centreBias;
        if (numberOfDrones == 0)
        {
            boidCentreCollider.gameObject.SetActive(false);
            boidRangedTargeting.gameObject.SetActive(false);
        }
        else
        {
            boidCentreCollider.gameObject.SetActive(true);
            if (boidFiringInfo.weaponType != WeaponType.NONE) boidRangedTargeting.gameObject.SetActive(true);
        }
    }

    private Quaternion UpdateBoidRotation(Rigidbody2D boid)
    {
        float newRotation = 360 - Mathf.Atan2(boid.velocity.x, boid.velocity.y) * Mathf.Rad2Deg;
        float oldRotation = boid.transform.rotation.eulerAngles.z;
        if (newRotation > oldRotation + 180) oldRotation += 360;
        else if (newRotation < oldRotation - 180) oldRotation -= 360;
        return Quaternion.Euler(0, 0, Mathf.Clamp(newRotation, oldRotation - 15, oldRotation + 15));
    }

    public void AddBoid(Rigidbody2D boidBody)
    {
        droneList.Add(boidBody);
        storedDirToNearest.Add(Vector2.zero);
        storedMaxSpeedMultiplier.Add(1f);
        numberOfDrones++;
    }

    public void RemoveBoid(int index)
    {
        Rigidbody2D boidBody = droneList[index];
        droneList.RemoveAt(index);
        storedDirToNearest.RemoveAt(index);
        storedMaxSpeedMultiplier.RemoveAt(index);
        numberOfDrones --;
        returnToPoolEvent.Invoke(boidBody.gameObject);
    }

    public void RemoveBoid(GameObject droneObject)
    {
        int index = droneList.FindIndex(o => o.gameObject == droneObject);
        if (index == -1) return;
        RemoveBoid(index);
    }

    private Vector2 CalculateRotationDirVector(int i, Vector2 averageRotationVector)
    {
        float rotation = droneList[i].transform.rotation.eulerAngles.z;
        Vector2 rotationDirVector = new Vector2(Mathf.Sin(rotation * Mathf.Deg2Rad), Mathf.Cos(rotation * Mathf.Deg2Rad)).normalized;
        return (averageRotationVector - rotationDirVector).normalized;
    }

    private Vector2 CalculateNearestBoidVector(int i)
    {
        if ((altSeparationCounter + i) % seperationCheckCounterMod == 0) UpdateStoredVectorToNearestBoid(i);
        return storedDirToNearest[i];
    }

    private void UpdateStoredVectorToNearestBoid(int i)
    {
        Rigidbody2D boid = droneList[i];
        Rigidbody2D nearestDrone = NearestBoid(boid.transform.position, i);

        Vector2 toNearestBoid = nearestDrone.transform.position - boid.transform.position;
        if (toNearestBoid.sqrMagnitude < 0.01f * ScaleManager.PlayerScaleGlobal * ScaleManager.PlayerScaleGlobal) toNearestBoid = Random.insideUnitCircle;
        storedDirToNearest[i] = toNearestBoid.normalized;
    }

    private void UpdateCentreGraphic()
    {
        if (numberOfDrones == 0 || followingObject == playerBody.gameObject)
        {
            pointMarkGraphic.gameObject.SetActive(false);
            centreGraphic.gameObject.SetActive(false);
            return;
        }

        pointMarkGraphic.gameObject.SetActive(true);
        pointMarkGraphic.transform.position = centre;
        pointMarkGraphic.transform.localScale = Vector3.one * 0.075f * Mathf.Pow(ScaleManager.ScaleSizeFactorGlobal, ScaleManager.PlayerScaleGlobal - 1f);

        if (!showCentre)
        {
            centreGraphic.gameObject.SetActive(false);
            return;
        }
        // Todo: centreGraphic is unused. Can safely be removed
        //centreGraphic.gameObject.SetActive(true);
        centreGraphic.transform.position = centre;
        centreGraphic.transform.localScale = Vector3.one * cohesionDistanceCeiling * 2f;

    }

    private Vector2 CalculateCentre()
    {
        if (followObject && followingObject != null) return followingObject.transform.position;

        Vector2 boidCentre = Vector2.zero;
        const float standardWeight = 100;
        float currentWeight;
        float divisor = 0;
        float referenceDistanceSqr = cohesionDistanceCeiling * cohesionDistanceCeiling;

        foreach (var boid in droneList)
        {
            float distanceSqr = ((Vector2)boid.transform.position - centre).sqrMagnitude;
            currentWeight = standardWeight * Mathf.Clamp(((2 * referenceDistanceSqr) - distanceSqr) / referenceDistanceSqr, 0.01f, 1);

            boidCentre += (Vector2)boid.transform.position * currentWeight;
            divisor += currentWeight;
        }

        boidCentre /= divisor;

        // Anchoring
        boidCentre = (centreBias * centreBiasMag) + ((1 - centreBiasMag) * boidCentre);

        return AddAggressionAdjustment(boidCentre, aggressionMultiplier);
    }

    private Vector2 AddAggressionAdjustment(Vector2 boidCentre, float aggression)
    {
        Collider2D otherCollider = GetNearestOtherCollider();
        if (otherCollider == null) return boidCentre;
        return ((1 - aggression) * boidCentre) + (Vector2)otherCollider.transform.position * aggression;
    }

    private Collider2D GetNearestOtherCollider()
    {
        if (collidersInRange.Count == 0) return null;

        float nearestSqr = Mathf.Infinity;
        Collider2D nearestCollider = collidersInRange[0];
        int colliderScale = 0;
        int i = 0;
        int counter = 0;
        do
        {
            counter ++;
            if (collidersInRange[i] == null)
            {
                collidersInRange.RemoveAt(i);
                i--;
                continue;
            }
            if (collidersInRange[i].TryGetComponent(out AdditionalObjectInfo objectInfo))
            {
                if (objectInfo.scale < colliderScale) continue;

                float newSqr = (collidersInRange[i].transform.position - boidCentreCollider.transform.position).sqrMagnitude;
                if (newSqr < nearestSqr && objectInfo.scale >= colliderScale) 
                {  
                    nearestSqr = newSqr;
                    nearestCollider = collidersInRange[i];   
                    colliderScale = objectInfo.scale;          
                }
            }
            i ++;
        }
        while(i < collidersInRange.Count && counter < 100);

        return nearestCollider;
    }

    private Vector2 CalculateAverageRotationVector()
    {
        float rotationSum = 0;
        foreach (var boid in droneList) rotationSum += (boid.transform.rotation.eulerAngles.z + 360f) % 360f;
        averageRotation = 360f - (rotationSum / droneList.Count);
        return new Vector2(Mathf.Sin(averageRotation * Mathf.Deg2Rad), Mathf.Cos(averageRotation * Mathf.Deg2Rad)).normalized;
    }

    private Rigidbody2D NearestBoid(Vector3 boidPos, int excludeIndex)
    {
        Rigidbody2D nearestBoid = droneList[0];

        for (int i = 0; i < droneList.Count; i++)
        {
            if (i == excludeIndex) continue;
            float deltaPosSqr = (boidPos - droneList[i].transform.position).sqrMagnitude;
            if (deltaPosSqr < (boidPos - nearestBoid.transform.position).sqrMagnitude) nearestBoid = droneList[i];
        }

        return nearestBoid;
    }
}



[System.Serializable] 
public struct BoidFiringInfo
{
    public WeaponType weaponType;
    public float fireRatePerDrone;
    public int munitionID;
    public float munitionSpeed;
}
