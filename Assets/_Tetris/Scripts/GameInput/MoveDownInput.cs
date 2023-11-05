using UnityEngine;

sealed class MoveDownInput : IInput, IUpdatableInput
{
    public event InputPerformedCallback Performed;
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.S) ||
            Input.GetKeyDown(KeyCode.DownArrow))
        {
            Performed?.Invoke(this);
        }
    }
}