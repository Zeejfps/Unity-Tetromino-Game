public sealed class MainInputActions : InputActionGroup
{
    public MainInputActions(
        MoveLeftInputAction moveLeftInputAction,
        MoveRightInputAction moveRightInputAction,
        MoveDownInputAction moveDownInputAction,
        RotateInputAction rotateInputAction,
        InstantDropInputAction instantDropInputAction
    ) {
        Add(moveDownInputAction);
        Add(moveRightInputAction);
        Add(moveLeftInputAction);
        Add(rotateInputAction);
        Add(instantDropInputAction);
    }
}