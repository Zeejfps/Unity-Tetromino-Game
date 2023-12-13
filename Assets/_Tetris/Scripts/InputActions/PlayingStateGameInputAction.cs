public abstract class PlayingStateGameInputAction : IInputAction
{
    public event InputPerformedCallback Triggered;

    private readonly IClock m_Clock;
    private readonly IGameStateMachine m_GameStateMachine;

    protected PlayingStateGameInputAction(IClock clock, IGameStateMachine gameStateMachine)
    {
        m_Clock = clock;
        m_GameStateMachine = gameStateMachine;
        
        m_Clock.Ticked += Clock_OnTicked;
    }

    private void Clock_OnTicked(IClock clock)
    {
        Update();
    }

    private void Update()
    {
        if (m_GameStateMachine.State != GameState.Playing)
            return;

        OnUpdate();
    }

    protected abstract void OnUpdate();
    
    protected virtual void OnPerformed()
    {
        Triggered?.Invoke(this);
    }
}