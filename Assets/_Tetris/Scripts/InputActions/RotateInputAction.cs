using UnityEngine;

public sealed class RotateInputAction : BaseInputAction
{
    private readonly ITouchGestureDetector m_TouchGestureDetector;

    public RotateInputAction(IClock clock,
        ITouchGestureDetector touchGestureDetector)
        : base(clock)
    {
        m_TouchGestureDetector = touchGestureDetector;
    }

    protected override void OnUpdate()
    {
        if (Input.GetKeyDown(KeyCode.R) ||
            Input.GetKeyDown(KeyCode.UpArrow) ||
            Input.GetKeyDown(KeyCode.W) ||
            m_TouchGestureDetector.TouchDetected())
        {
            OnTriggered();
        }
    }
}