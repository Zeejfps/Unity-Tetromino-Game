using UnityEngine;

sealed class MoveLeftInput : PlayingStateGameInput
{
    private readonly ITouchGestureDetector m_TouchGestureDetector;

    public MoveLeftInput(IGameStateMachine gameStateMachine, ITouchGestureDetector touchGestureDetector) : base(gameStateMachine)
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