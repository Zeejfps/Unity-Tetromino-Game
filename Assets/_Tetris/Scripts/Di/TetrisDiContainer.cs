using UnityEngine;

[CreateAssetMenu]
public sealed class TetrisDiContainer : DiContainer
{
    [SerializeField] private Tetromino[] m_TetrominoPrefabs;

    protected override void OnInit()
    {
        var grid = new Grid(10, 20);
        RegisterSingleton<IGrid>(grid);
        
        var tetrominoSpawner = new TetrominoSpawner(grid, m_TetrominoPrefabs);
        RegisterSingleton<ITetrominoSpawner>(tetrominoSpawner);
        
        RegisterSingleton<IGameStateMachine, TetrisGameStateMachine>();
        RegisterSingleton<IGameScore, GameScore>();
        RegisterSingleton<IClock, UnityClock>();
        RegisterSingleton<ITouchGestureDetector, LegacyInputSystemTouchGestureDetector>();
        RegisterSingleton<MoveLeftInput>();
        RegisterSingleton<MoveRightInput>();
        RegisterSingleton<MoveDownInput>();
        RegisterSingleton<RotateInput>();
        RegisterSingleton<InstantDropInput>();
        
        var moveLeftInput = Get<MoveLeftInput>();
        var moveRightInput = Get<MoveRightInput>();
        var moveDownInput = Get<MoveDownInput>();
        var rotateInput = Get<RotateInput>();
        var instantDropInput = Get<InstantDropInput>();
        var gameInput = new GameInput(
            moveLeftInput,
            moveRightInput,
            moveDownInput,
            rotateInput,
            instantDropInput
        );
        RegisterSingleton<IGameInput>(gameInput);
    }
}
