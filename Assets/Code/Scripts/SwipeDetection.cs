using UnityEngine;

public class SwipeDetection : MonoBehaviour
{
    public static event OnSwipeInput SwipeEvent;
    public delegate void OnSwipeInput(Vector2 direction);

    private Vector2 startPosition;
    private Vector2 swipeDelta;

    private float deadZone = 80f;

    private bool isSwiping;
    private bool isMobile;
    private void Start()
    {
        isMobile = Application.isMobilePlatform;
    }
    private void Update()
    {
        if (isMobile)
        {
            if (Input.touchCount > 0)
            {
                if (Input.GetTouch(0).phase == TouchPhase.Began)
                {
                    isSwiping = true;
                    startPosition = Input.GetTouch(0).position;
                }
                else if (Input.GetTouch(0).phase == TouchPhase.Ended)
                    ResetSwipe();
            }
        }
        else
        {
            if (Input.GetMouseButtonDown(0))
            {
                isSwiping = true;
                startPosition = Input.mousePosition;
            }
            else if (Input.GetMouseButtonUp(0))
                ResetSwipe();
        }
        CheckSwipe();
    }
    private void CheckSwipe()
    {
        swipeDelta = Vector2.zero;
        if (isSwiping)
        {
            if (isMobile && Input.touchCount > 0)
                swipeDelta = Input.GetTouch(0).position - startPosition;
            else if(Input.GetMouseButton(0))
                swipeDelta = (Vector2)Input.mousePosition - startPosition;
        }
        if (swipeDelta.magnitude > deadZone)
        {
            if (SwipeEvent != null)
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