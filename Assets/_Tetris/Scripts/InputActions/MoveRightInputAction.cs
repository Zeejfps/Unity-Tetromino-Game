using UnityEngine;

public sealed class MoveRightInputAction : BaseInputAction
{
    private readonly ITouchGestureDetector m_TouchGestureDetector;
    
    public MoveRightInputAction(IClock clock,
        ITouchGestureDetector touchGestureDetector) : base(clock)
    {
        m_TouchGestureDetector = touchGestureDetector;
    }
    
    protected override void OnUpdate()
    {
        if (Input.GetKeyDown(KeyCode.D) ||
            Input.GetKeyDown(KeyCode.RightArrow) ||
            m_TouchGestureDetector.SwipeRightDetected())
        {
            OnTriggered();
        }
    }
}