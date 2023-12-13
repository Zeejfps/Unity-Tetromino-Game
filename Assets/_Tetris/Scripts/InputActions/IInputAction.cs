
public delegate void InputPerformedCallback(IInputAction inputAction);

public interface IInputAction
{
    event InputPerformedCallback Triggered;
    
    void Enable();
    void Disable();
}