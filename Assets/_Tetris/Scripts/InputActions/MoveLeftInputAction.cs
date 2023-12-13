using UnityEngine;

public sealed class MoveLeftInputAction : BaseInputAction
{
    private readonly ITouchGestureDetector m_TouchGestureDetector;
    
    public MoveLeftInputAction(
        IClock clock,
        ITouchGestureDetector touchGestureDetector) : base(clock)
    {
        m_TouchGestureDetector = touchGestureDetector;
    }

    protected override void OnUpdate()
    {
        if (Input.GetKeyDown(KeyCode.A) ||
            Input.GetKeyDown(KeyCode.LeftArrow) ||
            m_TouchGestureDetector.SwipeLeftDetected())
        {
            OnTriggered();
        }
    }
}