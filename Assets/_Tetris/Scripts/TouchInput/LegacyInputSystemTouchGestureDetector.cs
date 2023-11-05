using UnityEngine;
using UnityEngine.EventSystems;

[DefaultExecutionOrder(-10)]
sealed class LegacyInputSystemTouchGestureDetector : MonoBehaviour, ITouchGestureDetector
{
    private Vector2 m_PrevTouchPosition;
    private Vector2 m_TouchTotalMoveDelta;
    private bool m_SwipeDetected;
    private bool m_TapDetected;
    private bool m_SwipeLeftDetected;
    private bool m_SwipeRightDetected;
    private bool m_SwipeDownDetected;
    private bool m_IsTrackingTouch;
    
    public bool SwipeLeftDetected()
    {
        return m_SwipeLeftDetected;
    }

    public bool SwipeRightDetected()
    {
        return m_SwipeRightDetected;
    }

    public bool SwipeDownDetected()
    {
        return m_SwipeDownDetected;
    }

    public bool TouchDetected()
    {
        return m_TapDetected;
    }

    private void Update()
    {
        m_TapDetected = false;
        m_SwipeLeftDetected = false;
        m_SwipeRightDetected = false;
        m_SwipeDownDetected = false;
        
        var touchCount = Input.touchCount;
        if (touchCount > 0)
        {
            var touch = Input.GetTouch(0);
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject(touch.fingerId))
            {
                return;
            }
            if (touch.phase == TouchPhase.Began)
            {
                m_IsTrackingTouch = true;
                m_SwipeDetected = false;
                m_PrevTouchPosition = touch.position;
                m_TouchTotalMoveDelta = Vector2.zero;
            }
            else if (m_IsTrackingTouch && touch.phase == TouchPhase.Moved)
            {
                var position = touch.position;
                m_TouchTotalMoveDelta += position - m_PrevTouchPosition;
                m_PrevTouchPosition = position;
                var deltaMagnitude = m_TouchTotalMoveDelta.magnitude;
                if (deltaMagnitude < Screen.width / 12f)
                {
                    return;
                }
                
                var touchMoveDelta = m_TouchTotalMoveDelta;
                if (Mathf.Abs(touchMoveDelta.x) > Mathf.Abs(touchMoveDelta.y))
                {
                    // Horizontal swipe
                    if (touchMoveDelta.x < 0)
                        m_SwipeLeftDetected = true;
                    else
                        m_SwipeRightDetected = true;
                    
                    m_TouchTotalMoveDelta = Vector2.zero;
                    m_SwipeDetected = true;
                }
            }
            else if (m_IsTrackingTouch && touch.phase == TouchPhase.Ended)
            {
                var touchMoveDelta = m_TouchTotalMoveDelta;
                var deltaMagnitude = touchMoveDelta.magnitude;
                
                if (m_SwipeDetected)
                    return;
                
                if (deltaMagnitude < 100f)
                {
                    m_TapDetected = true;
                }
                else if (Mathf.Abs(touchMoveDelta.x) > Mathf.Abs(touchMoveDelta.y))
                {
                    // Horizontal swipe
                    if (touchMoveDelta.x < 0)
                        m_SwipeLeftDetected = true;
                    else
                        m_SwipeRightDetected = true;
                }
                else
                {
                    // Vertical swipe
                    if (touchMoveDelta.y < 0)
                        m_SwipeDownDetected = true;
                }

                m_IsTrackingTouch = false;
            }
            else if (m_IsTrackingTouch && touch.phase == TouchPhase.Canceled)
            {
                m_IsTrackingTouch = false;
            }
        }
    }
}