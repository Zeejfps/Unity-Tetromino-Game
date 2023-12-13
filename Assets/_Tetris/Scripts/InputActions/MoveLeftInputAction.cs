using UnityEngine;

public sealed class MoveLeftInputAction : PlayingStateGameInputAction
{
    private readonly ITouchGestureDetector m_TouchGestureDetector;
    
    public MoveLeftInputAction(
        IClock clock,
        IGameStateMachine gameStateMachine, 
        ITouchGestureDetector touchGestureDetector) : base(clock, gameStateMachine)
    {
        m_TouchGestureDetector = touchGestureDetector;
    }

    protected override void OnUpdate()
    {
        if (Input.GetKeyDown(KeyCode.A) ||
            Input.GetKeyDown(KeyCode.LeftArrow) ||
            m_TouchGestureDetector.SwipeLeftDetected())
        {
            OnPerformed();
        }
    }
}