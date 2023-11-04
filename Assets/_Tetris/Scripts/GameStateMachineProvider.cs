using UnityEngine;

[CreateAssetMenu(menuName = "Providers/Game State Machine Provider")]
public sealed class GameStateMachineProvider : ScriptableObject
{
    private IGameStateMachine m_GameStateMachine;
    
    public IGameStateMachine Get()
    {
        if (m_GameStateMachine == null)
        {
            m_GameStateMachine = new TetrisGameStateMachine();
        }

        return m_GameStateMachine;
    }
}

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