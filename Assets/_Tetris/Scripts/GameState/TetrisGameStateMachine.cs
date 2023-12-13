sealed class TetrisGameStateMachine : IGameStateMachine
{
    public event GameStateChangedCallback StateChanged;

    private GameState m_State;
    public GameState State
    {
        get => m_State;
        private set
        {
            if (m_State == value)
                return;
            
            var prevState = m_State;
            m_State = value;
            OnStateChanged(prevState, m_State);
        }
    }

    private void OnStateChanged(GameState prevState, GameState currState)
    {
        StateChanged?.Invoke(prevState, currState);
    }

    public void TransitionTo(GameState state)
    {
        State = state;
    }
}