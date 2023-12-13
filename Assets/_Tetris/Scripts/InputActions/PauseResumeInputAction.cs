using UnityEngine;

public sealed class PauseResumeInputAction : BaseInputAction
{
    public PauseResumeInputAction(IClock clock) : base(clock)
    {
    }

    protected override void OnUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            OnTriggered();
    }
}