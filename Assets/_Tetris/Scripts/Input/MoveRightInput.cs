using UnityEngine;

sealed class MoveRightInput : IInput, IUpdatableInput
{
    public event InputPerformedCallback Performed;
    
    private readonly ITouchGestureDetector m_TouchGestureDetector;

    public MoveRightInput(ITouchGestureDetector touchGestureDetector)
    {
        m_TouchGestureDetector = touchGestureDetector;
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.D) ||
            Input.GetKeyDown(KeyCode.RightArrow) ||
            m_TouchGestureDetector.SwipeRightDetected())
        {
            Performed?.Invoke(this);
        }
    }
}