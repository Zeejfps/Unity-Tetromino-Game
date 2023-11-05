using UnityEngine;

sealed class MoveRightInput : PlayingStateGameInput
{
    private readonly ITouchGestureDetector m_TouchGestureDetector;
    
    public MoveRightInput(IGameStateMachine gameStateMachine, ITouchGestureDetector touchGestureDetector) : base(gameStateMachine)
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