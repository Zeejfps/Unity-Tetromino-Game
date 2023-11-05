using UnityEngine;

sealed class MoveDownInput : PlayingStateGameInput
{
    public MoveDownInput(IGameStateMachine gameStateMachine) : base(gameStateMachine)
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