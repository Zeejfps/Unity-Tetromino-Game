public interface IGameInput
{
    IInput MoveLeft { get; }
    IInput MoveRight { get; }
    IInput Rotate { get; }
    IInput MoveDown { get; }
    IInput InstantDrop { get; }
}