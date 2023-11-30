using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class HowToPlayPageLogic : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI pageChangeButtonText;
    [SerializeField] Image arrow;

    [SerializeField] GameObject page1;
    [SerializeField] GameObject page2;


    private void Start() 
    {
        page1.SetActive(true);
        page2.SetActive(false);
        pageChangeButtonText.SetText("Page 1/2");
        arrow.transform.localRotation = Quaternion.Euler(0, 0, 180);
    }

    public void SwitchPages()
    {
        if (page1.activeInHierarchy)
        {
            page1.SetActive(false);
            page2.SetActive(true);
            pageChangeButtonText.SetText("Page 2/2");
            arrow.transform.localRotation = Quaternion.Euler(0, 0, 0);
            return;
        }

        page1.SetActive(true);
        page2.SetActive(false);
        pageChangeButtonText.SetText("Page 1/2");
        arrow.transform.localRotation = Quaternion.Euler(0, 0, 180);
    }

}
