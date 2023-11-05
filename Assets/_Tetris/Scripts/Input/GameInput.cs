sealed class GameInput : IGameInput
{
    public IInput MoveLeft { get; }
    public IInput MoveRight { get; }
    public IInput Rotate { get; }
    public IInput MoveDown { get; }
    public IInput InstantDrop { get; }

    public GameInput(IInput moveLeft, IInput moveRight, IInput moveDown, IInput rotate, IInput instantDrop)
    {
        MoveLeft = moveLeft;
        MoveRight = moveRight;
        Rotate = rotate;
        InstantDrop = instantDrop;
        MoveDown = moveDown;
    }
}