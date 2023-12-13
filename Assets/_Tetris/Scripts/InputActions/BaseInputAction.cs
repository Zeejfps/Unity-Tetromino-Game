public abstract class BaseInputAction : IInputAction
{
    public event InputPerformedCallback Triggered;
    
    private bool m_IsEnabled;

    private readonly IClock m_Clock;

    protected BaseInputAction(IClock clock)
    {
        m_Clock = clock;
    }
    
    public void Enable()
    {
        if (m_IsEnabled)
            return;
        
        m_IsEnabled = true;
        m_Clock.Ticked += Clock_OnTicked;
    }

    public void Disable()
    {
        if (!m_IsEnabled)
            return;
        
        m_IsEnabled = false;
        m_Clock.Ticked -= Clock_OnTicked;
    }

    private void Clock_OnTicked(IClock clock)
    {
        Update();
    }

    private void Update()
    {
        OnUpdate();
    }

    protected abstract void OnUpdate();
    
    protected virtual void OnTriggered()
    {
        Triggered?.Invoke(this);
    }
}