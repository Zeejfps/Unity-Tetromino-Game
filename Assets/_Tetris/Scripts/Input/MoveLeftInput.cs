using UnityEngine;

sealed class MoveLeftInput : IInput, IUpdatableInput
{
    public event InputPerformedCallback Performed;

    private readonly ITouchGestureDetector m_TouchGestureDetector;

    public MoveLeftInput(ITouchGestureDetector touchGestureDetector)
    {
        m_TouchGestureDetector = touchGestureDetector;
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.A) ||
            Input.GetKeyDown(KeyCode.LeftArrow) ||
            m_TouchGestureDetector.SwipeLeftDetected())
        {
            Performed?.Invoke(this);
        }
    }
}