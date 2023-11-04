public delegate void GameStateChangedCallback(GameState prevState, GameState currState);

public interface IGameStateMachine
{
    event GameStateChangedCallback StateChanged;
    GameState State { get; }
    void TransitionTo(GameState state);
}