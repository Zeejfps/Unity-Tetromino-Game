using UnityEngine;

internal sealed class UnityClockRunner : MonoBehaviour
{
    private UnityClock m_Clock;

    public void Init(UnityClock clock)
    {
        m_Clock = clock;
    }

    private void Update()
    {
        if (m_Clock != null)
            m_Clock.Tick(Time.deltaTime);
    }
}