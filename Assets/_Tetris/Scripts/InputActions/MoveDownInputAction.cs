using UnityEngine;

public sealed class MoveDownInputAction : BaseInputAction
{
    public MoveDownInputAction(IClock clock) : base(clock)
    {
    }
    
    protected override void OnUpdate()
    {
        if (Input.GetKeyDown(KeyCode.S) ||
            Input.GetKeyDown(KeyCode.DownArrow))
        {
            OnTriggered();
        }
    }
}