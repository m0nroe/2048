using UnityEngine;

public class SwipeDetection : MonoBehaviour
{
    public delegate void OnSwipeInput(Vector2 direction);
    public static event OnSwipeInput SwipeEvent;

    private Vector2 startPosition;
    private Vector2 swipeDelta;

    private float deadZone = 80;
    private bool isSwiping;
    private bool isMobile;

    private void Start()
    {
        isMobile = Application.isMobilePlatform;
    }

    private void Update()
    {
        if (!isMobile) 
        {
            if (Input.GetMouseButtonDown(0))
            {
                isSwiping = true;
                startPosition = Input.mousePosition;
            }
            else if (Input.GetMouseButtonUp(0))
                ResetSwipe();
        }
        else 
        { 
            if(Input.touchCount > 0) 
            {
                Touch touch = Input.GetTouch(0);
                if(touch.phase == TouchPhase.Began) 
                {
                    isSwiping = true;
                    startPosition = touch.position;
                }
                else if(touch.phase == TouchPhase.Canceled || touch.phase == TouchPhase.Ended) 
                    ResetSwipe();
            }
        }

        CheckSwipe();
    }

    private void CheckSwipe() 
    {
        swipeDelta = Vector2.zero;
        if (isSwiping) 
        {
            if (!isMobile && Input.GetMouseButton(0)) 
            { 
                swipeDelta = (Vector2)Input.mousePosition - startPosition;

            }
            else if (Input.touchCount > 0)
                swipeDelta = Input.GetTouch(0).position - startPosition;
        }

        if (swipeDelta.magnitude > deadZone) 
        { 
            if(SwipeEvent != null) 
            {
                if (Mathf.Abs(swipeDelta.x) > Mathf.Abs(swipeDelta.y))
                    SwipeEvent(swipeDelta.x > 0 ? Vector2.right : Vector2.left);
                else
                    SwipeEvent(swipeDelta.y > 0 ? Vector2.up : Vector2.down);
            }

            ResetSwipe();
        }
    }

    private void ResetSwipe()
    {
        isSwiping = false;
        startPosition = Vector2.zero;
        swipeDelta = Vector2.zero;
    }
}
