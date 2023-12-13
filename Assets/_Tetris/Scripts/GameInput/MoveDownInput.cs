using UnityEngine;

sealed class MoveDownInput : PlayingStateGameInput
{
    public MoveDownInput(IClock clock, IGameStateMachine gameStateMachine) : base(clock, gameStateMachine)
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