using UnityEngine;

public sealed class MoveRightInputAction : PlayingStateGameInputAction
{
    private readonly ITouchGestureDetector m_TouchGestureDetector;
    
    public MoveRightInputAction(IClock clock, IGameStateMachine gameStateMachine, ITouchGestureDetector touchGestureDetector) 
        : base(clock, gameStateMachine)
    {
        m_TouchGestureDetector = touchGestureDetector;
    }
    
    protected override void OnUpdate()
    {
        if (Input.GetKeyDown(KeyCode.D) ||
            Input.GetKeyDown(KeyCode.RightArrow) ||
            m_TouchGestureDetector.SwipeRightDetected())
        {
            OnPerformed();
        }
    }
}