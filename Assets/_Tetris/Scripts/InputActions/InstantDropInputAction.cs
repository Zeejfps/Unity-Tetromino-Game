using UnityEngine;

public sealed class InstantDropInputAction : PlayingStateGameInputAction
{
    private readonly ITouchGestureDetector m_TouchGestureDetector;
    
    public InstantDropInputAction(IClock clock, IGameStateMachine gameStateMachine, 
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