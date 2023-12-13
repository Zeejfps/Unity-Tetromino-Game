using UnityEngine;

public sealed class RotateInputAction : PlayingStateGameInputAction
{
    private readonly ITouchGestureDetector m_TouchGestureDetector;

    public RotateInputAction(IClock clock, IGameStateMachine gameStateMachine,
        ITouchGestureDetector touchGestureDetector)
        : base(clock, gameStateMachine)
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
            OnPerformed();
        }
    }
}