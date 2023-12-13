public sealed class MainInputActions : InputActionGroup
{
    public MoveLeftInputAction MoveLeftInputAction { get; }
    public MoveRightInputAction MoveRightInputAction { get; }
    public MoveDownInputAction MoveDownInputAction { get; }
    public RotateInputAction RotateInputAction { get; }
    public InstantDropInputAction InstantDropInputAction { get; }
    
    public MainInputActions(
        MoveLeftInputAction moveLeftInputAction,
        MoveRightInputAction moveRightInputAction,
        MoveDownInputAction moveDownInputAction,
        RotateInputAction rotateInputAction,
        InstantDropInputAction instantDropInputAction
    ) {
        MoveDownInputAction = moveDownInputAction;
        MoveLeftInputAction = moveLeftInputAction;
        MoveRightInputAction = moveRightInputAction;
        RotateInputAction = rotateInputAction;
        InstantDropInputAction = instantDropInputAction;
        
        Add(moveDownInputAction);
        Add(moveRightInputAction);
        Add(moveLeftInputAction);
        Add(rotateInputAction);
        Add(instantDropInputAction);
    }
}