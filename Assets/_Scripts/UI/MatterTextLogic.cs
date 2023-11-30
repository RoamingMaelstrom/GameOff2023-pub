using UnityEngine;
using TMPro;

public class MatterTextLogic : MonoBehaviour
{
    [SerializeField] ScoreLogic scoreLogic;
    [SerializeField] TextMeshProUGUI matterText;

    private void Update() 
    {
        matterText.SetText(string.Format("Matter\n{0:n0}", scoreLogic.GetCurrentMatter()));
    }
}
