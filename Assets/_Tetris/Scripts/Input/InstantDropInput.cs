using UnityEngine;

sealed class InstantDropInput : IInput, IUpdatableInput
{
    public event InputPerformedCallback Performed;

    private readonly ITouchGestureDetector m_TouchGestureDetector;

    public InstantDropInput(ITouchGestureDetector touchGestureDetector)
    {
        m_TouchGestureDetector = touchGestureDetector;
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) ||
            m_TouchGestureDetector.SwipeDownDetected())
        {
            Performed?.Invoke(this);
        }
    }
}