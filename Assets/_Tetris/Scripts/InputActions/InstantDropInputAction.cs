using UnityEngine;

public sealed class InstantDropInputAction : BaseInputAction
{
    private readonly ITouchGestureDetector m_TouchGestureDetector;
    
    public InstantDropInputAction(IClock clock, ITouchGestureDetector touchGestureDetector) : base(clock)
    {
        m_TouchGestureDetector = touchGestureDetector;
    }

    protected override void OnUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Space) ||
            m_TouchGestureDetector.SwipeDownDetected())
        {
            OnTriggered();
        }
    }
}