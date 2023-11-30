using System;
using System.Collections;
using System.Collections.Generic;
using SOEvents;
using UnityEngine;
using System.Linq;
using System.Runtime.CompilerServices;

public class MineLogic : MonoBehaviour
{
    [SerializeField] GameObjectSOEvent despawnEvent;
    [SerializeField] SpriteRenderer detectorLightRenderer;
    [SerializeField] SpriteRenderer mainLightRenderer;
    [SerializeField] Color greenColour;
    [SerializeField] Color redColour;
    [SerializeField] Rigidbody2D mineBody;
    [SerializeField] GameObject explosionPrefab;
    
    [SerializeField] GameObject focusObject;
    [SerializeField] List<GameObject> trackedObjects = new();

    [SerializeField] float timeUntilExplodes = 3f;
    [SerializeField] float attractionAcceleration = 10f;
    [SerializeField] float timerDecayRate = 4f;
    [SerializeField] float timer = 0;
    [SerializeField] bool hasExploded = false;

    [SerializeField] float beepTimer = 0;
    [SerializeField] float beepThreshold = 0.8f;
    private float defaultBeepThreshold;
    [SerializeField] int beepIndex = 0;
    [SerializeField] List<string> beepSfx = new();


    private void Start() 
    {
        defaultBeepThreshold = beepThreshold;
    }

    private void OnEnable() 
    {
        timer = 0;
        hasExploded = false;

    }

    private void FixedUpdate() 
    {
        beepTimer += Time.fixedDeltaTime;
        timer += Time.fixedDeltaTime;

        if (focusObject == null && timer < timeUntilExplodes * 0.75f) timer -= Time.fixedDeltaTime * timerDecayRate;
        timer = Mathf.Clamp(timer, 0f, timeUntilExplodes);

        float a1 = detectorLightRenderer.color.a;
        float a2 = mainLightRenderer.color.a;

        Color targetColour = new Color(Mathf.Lerp(greenColour.r, redColour.r, 2 * timer / timeUntilExplodes), Mathf.Lerp(greenColour.g, redColour.g, 2 * timer / timeUntilExplodes), Mathf.Lerp(greenColour.b, redColour.b, 2 * timer / timeUntilExplodes));
        

        detectorLightRenderer.color = new Color(targetColour.g, targetColour.g, targetColour.b, a1);
        detectorLightRenderer.color = new Color(targetColour.r, targetColour.g, targetColour.b, a2);

        if (focusObject != null) mineBody.AddForce((focusObject.transform.position - mineBody.transform.position).normalized * attractionAcceleration * mineBody.mass);
        if (timer >= timeUntilExplodes && !hasExploded) StartCoroutine(Explode());

        HandleBeeping();
    }

    private void HandleBeeping()
    {
        if (beepTimer >= beepThreshold)
        {
            beepTimer = 0;

            if (timer != 0)
            {
                GliderSFX.Play.AtPoint(beepSfx[beepIndex], transform.position);
                beepIndex = Mathf.Clamp(beepIndex + 1, 0, beepSfx.Count - 1);
                beepThreshold = Mathf.Clamp(beepThreshold *= 0.66f, 0.2f, defaultBeepThreshold);
            }
            else
            {
                beepThreshold = defaultBeepThreshold;
                beepIndex = 0;
            }
        }
    }

    private IEnumerator Explode()
    {
        hasExploded = true;
        Instantiate(explosionPrefab, transform.position, Quaternion.identity, null);
        yield return new WaitForSeconds(0.1f);
        despawnEvent.Invoke(transform.parent.gameObject);
        yield return null;
    }

    private void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("obstacle_despawner")) return;
        if (other.TryGetComponent(out Health otherhealth))
        {
            beepTimer = beepThreshold;
            trackedObjects.Add(other.gameObject);
            if (focusObject == null) focusObject = other.gameObject;
            else if (other.gameObject.transform.localScale.sqrMagnitude > focusObject.transform.localScale.sqrMagnitude) focusObject = other.gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D other) 
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("obstacle_despawner")) return;

        int index = trackedObjects.FindIndex(o => o.gameObject.GetInstanceID() == other.gameObject.GetInstanceID());
        if (index != -1) trackedObjects.RemoveAt(index);

        if (focusObject == null) SelectNewTarget(mineBody.transform.position, trackedObjects);
        else if (other.gameObject.GetInstanceID() == focusObject.gameObject.GetInstanceID()) focusObject = SelectNewTarget(mineBody.transform.position, trackedObjects);
    }

    private static GameObject SelectNewTarget(Vector2 minePos, List<GameObject> trackedObjects)
    {
        if (trackedObjects.Count == 0) return null;

        GameObject output = trackedObjects[0];
        float outputDeltaSqr = ((Vector2)output.transform.position - minePos).sqrMagnitude;

        for (int i = 1; i < trackedObjects.Count; i++)
        {
            if (trackedObjects[i].transform.localScale.sqrMagnitude < output.transform.localScale.sqrMagnitude) continue;

            float newDeltaSqr = ((Vector2)trackedObjects[i].transform.position - minePos).sqrMagnitude;
            if (trackedObjects[i].transform.localScale.sqrMagnitude > output.transform.localScale.sqrMagnitude) 
            {
                output = trackedObjects[i];
                outputDeltaSqr = newDeltaSqr;
                continue;
            }
            
            if (newDeltaSqr < outputDeltaSqr) output = trackedObjects[i];
        }

        return output;
    }
}
