using UnityEngine;

sealed class InstantDropInput : PlayingStateGameInput
{
    private readonly ITouchGestureDetector m_TouchGestureDetector;
    
    public InstantDropInput(IClock clock, IGameStateMachine gameStateMachine, 
        ITouchGestureDetector touchGestureDetector) : base(clock, gameStateMachine)
    {
        m_TouchGestureDetector = touchGestureDetector;
    }

    protected override void OnUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Space) ||
            m_TouchGestureDetector.SwipeDownDetected())
        {
            OnPerformed();
        }
    }
}