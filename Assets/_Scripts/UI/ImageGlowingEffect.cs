using UnityEngine.UI;
using UnityEngine;

public class ImageGlowingEffect : MonoBehaviour
{
    [SerializeField] Image image;
    [SerializeField] float flashFrequency = 0.5f;
    [SerializeField] float minAlpha = 0.25f;
    [SerializeField] float maxAlpha = 0.75f;

    [SerializeField] private Color spriteColour;
    [SerializeField] private float progress;

    private void Start() 
    {
        spriteColour = image.color;
        image.color -= new Color(0, 0, 0, 1.0f - minAlpha);
        progress = Random.Range(0, Mathf.PI * 2f);
        flashFrequency *= 0.5f;
    }

    private void FixedUpdate() 
    {
        progress += Time.fixedDeltaTime * flashFrequency;
        float a = minAlpha + Mathf.Abs(Mathf.Sin(progress * Mathf.PI) * (maxAlpha - minAlpha));
        image.color = new Color(spriteColour.r, spriteColour.g, spriteColour.b, a);
    }
}
