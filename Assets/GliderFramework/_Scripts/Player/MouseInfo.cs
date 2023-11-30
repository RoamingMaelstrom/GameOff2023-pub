using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using SOEvents;

public class MouseInfo : MonoBehaviour
{
    [SerializeField] IntSOEvent mouseDownEvent;
    [SerializeField] IntSOEvent mouseUpEvent;

    public bool leftMouseHeldDown = false;
    public float timeLMButtonDown;

    public bool rightMouseHeldDown = false;
    public float timeRMButtonDown;

    public Vector2 mousePosScreen;
    public Vector2 mousePosWorld;

    private Vector2 mousePosTMinus1;
    public Vector2 deltaMousePosScreen;

    public static Vector2 mousePosScreenGlobal {get; private set;}
    public static Vector2 mousePosWorldGlobal {get; private set;}
    public static bool mouseOverUIGlobal {get; private set;}

    public bool mouseOverUI = false;



    public void OnLMB(InputValue value)
    {
        if (value.Get<float>() == 1) 
        {
            mouseDownEvent.Invoke(0);
            leftMouseHeldDown = true;
        }
        if (value.Get<float>() == 0) 
        {
            mouseUpEvent.Invoke(0);
            leftMouseHeldDown = false;
        }
    }

    public void OnRMB(InputValue value)
    {
        if (value.Get<float>() == 1) 
        {
            mouseDownEvent.Invoke(2);
            rightMouseHeldDown = true;
        }
        if (value.Get<float>() == 0) 
        {
            mouseUpEvent.Invoke(2);
            rightMouseHeldDown = false;
        }
    }

    public void OnMousePosition(InputValue value)
    {
        mousePosScreen = value.Get<Vector2>();
        mousePosWorld =  Camera.main.ScreenToWorldPoint(mousePosScreen);
    }

    private void Update() 
    {
        mouseOverUI = EventSystem.current.IsPointerOverGameObject();
        mousePosWorld =  Camera.main.ScreenToWorldPoint(mousePosScreen);

        if (leftMouseHeldDown) timeLMButtonDown += Time.deltaTime;
        else timeLMButtonDown = 0;

        if (rightMouseHeldDown) timeRMButtonDown += Time.deltaTime;
        else timeRMButtonDown = 0;

        mousePosScreenGlobal = mousePosScreen;
        mousePosWorldGlobal = mousePosWorld;
        mouseOverUIGlobal = mouseOverUI;
    }


    private void FixedUpdate() 
    {
        deltaMousePosScreen = mousePosScreen - mousePosTMinus1;
        mousePosTMinus1 = mousePosScreen;
    }
}


