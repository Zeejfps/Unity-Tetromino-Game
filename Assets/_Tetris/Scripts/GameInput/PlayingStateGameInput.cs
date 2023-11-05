abstract class PlayingStateGameInput : IInput, IUpdatableInput
{
    public event InputPerformedCallback Performed;

    private readonly IGameStateMachine m_GameStateMachine;

    protected PlayingStateGameInput(IGameStateMachine gameStateMachine)
    {
        m_GameStateMachine = gameStateMachine;
    }

    public void Update()
    {
        if (m_GameStateMachine.State != GameState.Playing)
            return;

        OnUpdate();
    }

    protected abstract void OnUpdate();
    
    protected virtual void OnPerformed()
    {
        Performed?.Invoke(this);
    }
}