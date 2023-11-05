
public delegate void InputPerformedCallback(IInput input);

public interface IInput
{
    event InputPerformedCallback Performed;
}