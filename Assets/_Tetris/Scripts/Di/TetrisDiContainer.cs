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
    }
}
