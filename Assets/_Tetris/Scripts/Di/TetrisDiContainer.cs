using UnityEngine;

[CreateAssetMenu]
public sealed class TetrisDiContainer : DiContainer, IGridConfig, ITetrominoPrefabsProvider
{
    [SerializeField] private Tetromino[] m_TetrominoPrefabs;
    
    public int GridWidth => 10;
    public int GridHeight => 20;
    public Tetromino[] Prefabs => m_TetrominoPrefabs;
    
    protected override void OnInit()
    {
        RegisterSingleton<IGridConfig>(this);
        RegisterSingleton<ITetrominoPrefabsProvider>(this);
        RegisterSingleton<IGrid, Grid>();
        RegisterSingleton<ITetrominoSpawner, TetrominoSpawner>();
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