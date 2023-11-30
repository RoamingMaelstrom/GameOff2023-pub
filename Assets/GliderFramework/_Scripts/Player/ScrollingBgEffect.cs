using UnityEngine;
using UnityEngine.UI;

public class ScrollingBgEffect : MonoBehaviour
{
    [SerializeField] float parallaxStrength;
    [Tooltip("Set to value that the Main Camera will start with")]
    [SerializeField] float referenceOrthographicSize;

    [SerializeField] Sprite bgSprite;
    [SerializeField] float spriteScale = 2f;
    [SerializeField] [Range(0f, 1f)] float spriteOpacity;

    [SerializeField] Image[] images = new Image[4];


    Vector2 imageSize;
    Vector3 referencePos;
    RectTransform rectTransform;



    private void Start() 
    {
        referencePos = Vector2.zero;

        rectTransform = GetComponent<RectTransform>();
        rectTransform.anchoredPosition = Vector2.zero;

        SetupImages();
    }

    private void LateUpdate() 
    {
        // Creates the parallax effect.
        Vector3 distance = Camera.main.transform.position * parallaxStrength;
        Vector3 rawPosition = Camera.main.WorldToScreenPoint(distance) - new Vector3(Screen.width / 2, Screen.height / 2, 0);
        rawPosition *= Camera.main.orthographicSize / referenceOrthographicSize;
        rawPosition -= referencePos;
        rectTransform.anchoredPosition = new Vector3(rawPosition.x, rawPosition.y, 0);

        LoopBackground();

        // rectTransform.localRotation = Quaternion.Euler(0, 0, - Camera.main.transform.rotation.eulerAngles.z);
    }



    // Looping effect so the camera cannot leave the background.
    private void LoopBackground()
    {
        if (rectTransform.anchoredPosition.x * 2 > imageSize.x) 
        {
            rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x - imageSize.x,
             rectTransform.anchoredPosition.y);
            referencePos.x += imageSize.x; 
            return;
        }
        if (rectTransform.anchoredPosition.x * 2 < -imageSize.x) 
        {
            rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x + imageSize.x,
             rectTransform.anchoredPosition.y);
            referencePos.x -= imageSize.x; 
            return;
        }
        if (rectTransform.anchoredPosition.y * 2 > imageSize.y) 
        {
            rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x,
             rectTransform.anchoredPosition.y - imageSize.y);
            referencePos.y += imageSize.y; 
            return;
        }
        if (rectTransform.anchoredPosition.y * 2 < -imageSize.y) 
        {
            rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x,
             rectTransform.anchoredPosition.y + imageSize.y);
            
            referencePos.y -= imageSize.y;
            return;
        }
    }

    private void SetImagePosition(int xPos, int yPos, int imageIndex)
    {
        Image image = images[imageIndex];
        image.rectTransform.anchoredPosition = new Vector3(Mathf.Sign(xPos) * 0.5f * imageSize.x,
         Mathf.Sign(yPos) * 0.5f * imageSize.y, 0);
    }

    private void SetupImages()
    {
        for (int i = 0; i < 4; i++)
        {
            float bgSize = Mathf.Max(Screen.width, Screen.height) * spriteScale / 2;

            images[i].rectTransform.sizeDelta = new Vector2(bgSize, bgSize);
            images[i].sprite = bgSprite;
            images[i].color = new Color(1, 1, 1, spriteOpacity);
        }

        imageSize = images[0].rectTransform.sizeDelta;  

        SetImagePosition(-1, 1, 0);
        SetImagePosition(1, 1, 1);
        SetImagePosition(-1, -1, 2);
        SetImagePosition(1, -1, 3);
    }
}
