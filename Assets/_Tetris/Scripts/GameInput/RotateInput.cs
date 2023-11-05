using UnityEngine;

sealed class RotateInput : IInput, IUpdatableInput
{
    public event InputPerformedCallback Performed;
    
    private readonly ITouchGestureDetector m_TouchGestureDetector;

    public RotateInput(ITouchGestureDetector touchGestureDetector)
    {
        m_TouchGestureDetector = touchGestureDetector;
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.R) ||
            Input.GetKeyDown(KeyCode.UpArrow) ||
            Input.GetKeyDown(KeyCode.W) ||
            m_TouchGestureDetector.TouchDetected())
        {
            Performed?.Invoke(this);
        }
    }
}