using UnityEngine;

public sealed class MoveDownInputAction : PlayingStateGameInputAction
{
    public MoveDownInputAction(IClock clock, IGameStateMachine gameStateMachine) : base(clock, gameStateMachine)
    {
    }
    
    protected override void OnUpdate()
    {
        if (Input.GetKeyDown(KeyCode.S) ||
            Input.GetKeyDown(KeyCode.DownArrow))
        {
            OnPerformed();
        }
    }
}