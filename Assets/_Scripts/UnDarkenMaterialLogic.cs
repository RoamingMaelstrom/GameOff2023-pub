using System.Collections;
using SOEvents;
using UnityEngine;

public class UnDarkenMaterialLogic : MonoBehaviour
{
    [SerializeField] SOEvent startScalingDelayedEvent;
    [SerializeField] float scalingAnimationDuration = 3f;
    [SerializeField] Color darkColour = new Color(0.45f, 0.45f, 0.45f, 1f);
    [SerializeField] Color targetColour = Color.white;
    [SerializeField] int runOnScale = 2;
    [SerializeField] Material scaleMaterial;

    private void Awake() 
    {
        startScalingDelayedEvent.AddListener(StartUndarkeningRoutine);
        scaleMaterial.SetColor("_Color", darkColour);
    }

    private void StartUndarkeningRoutine()
    {
        if (ScaleManager.PlayerScaleGlobal == runOnScale) StartCoroutine(UndarkenMaterial());
    }

    private IEnumerator UndarkenMaterial()
    {
        Color localColour = scaleMaterial.GetColor("_Color");
        float colourChange = localColour.r - targetColour.r;
        float rateOfChange = Time.fixedDeltaTime / scalingAnimationDuration;

        float delta = colourChange * rateOfChange;

        float timer = 0;
        int counter = 0;
        while (timer < scalingAnimationDuration && counter < 1000)
        {
            counter ++;
            localColour -= new Color(delta, delta, delta, 0);
            scaleMaterial.SetColor("_Color", localColour);
            timer += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
        
        yield return null;
    }
}
