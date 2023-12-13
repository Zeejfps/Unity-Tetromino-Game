using System.Collections.Generic;

public abstract class InputActionGroup
{
    private bool m_IsEnabled;
    private readonly HashSet<IInputAction> m_InputActions = new();
    
    public void Add(IInputAction inputAction)
    {
        m_InputActions.Add(inputAction);
        if (m_IsEnabled) inputAction.Enable();
        else inputAction.Disable();
    }

    public void Remove(IInputAction inputAction)
    {
        m_InputActions.Remove(inputAction);
    }

    public void Enable()
    {
        m_IsEnabled = true;
        foreach (var inputAction in m_InputActions)
        {
            inputAction.Enable();
        }
    }

    public void Disable()
    {
        m_IsEnabled = false;
        foreach (var inputAction in m_InputActions)
        {
            inputAction.Disable();
        }
    }
}