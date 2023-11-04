public delegate void GameStateChangedCallback(GameState prevState, GameState currState);

public interface IGameStateMachine
{
    event GameStateChangedCallback StateChanged; 
    void TransitionTo(GameState state);
}