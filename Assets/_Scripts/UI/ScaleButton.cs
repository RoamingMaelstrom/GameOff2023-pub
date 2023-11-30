using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScaleButton : MonoBehaviour
{
    [SerializeField] ScaleManager scaleManager;
    [SerializeField] Button button;
    [SerializeField] TextMeshProUGUI buttonText;

    bool buttonLocked = false;

    private void FixedUpdate() 
    {
        if (scaleManager.PlayerScale < 4)
        {
            buttonText.SetText(string.Format("Scale\n{0:n0}", scaleManager.GetMatterNeededToScale()));
            button.interactable = scaleManager.CanScale() && !buttonLocked;
        }
        else if (scaleManager.PlayerScale == 4)
        {
            buttonText.SetText(string.Format("Consume\n{0:n0}", scaleManager.GetMatterNeededToScale()));
            button.interactable = scaleManager.CanScale() && !buttonLocked;
        }
        else
        {
            buttonText.SetText("");
            button.interactable = false;
        }
    }

    public void Scale()
    {
        if (!scaleManager.CanScale()) return;
        scaleManager.TryIncrementScale();
        buttonLocked = true;
        StartCoroutine(UnlockButtonDelayed(ScaleManager.TotalScalingDurationGlobal));
    }

    private IEnumerator UnlockButtonDelayed(int delay)
    {
        yield return new WaitForSeconds(delay);
        buttonLocked = false;
    }
}
