using SOEvents;
using UnityEngine;

public class BackgroundBlurLogic : MonoBehaviour
{
    [SerializeField] SOEvent startScalingEvent;
    [SerializeField] Canvas canvas;

    private void Awake() 
    {
        startScalingEvent.AddListener(MoveBackSortingLayer);
    }

    private void MoveBackSortingLayer()
    {
        canvas.sortingOrder -= 10;
    }
}
