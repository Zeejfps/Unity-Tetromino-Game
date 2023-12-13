
public delegate void InputPerformedCallback(IInputAction inputAction);

public interface IInputAction
{
    event InputPerformedCallback Triggered;
}