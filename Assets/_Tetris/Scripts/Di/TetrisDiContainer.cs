using UnityEngine;

[CreateAssetMenu]
public sealed class TetrisDiContainer : DiContainer, ITetrominoPrefabsProvider
{
    [SerializeField] private Tetromino[] m_TetrominoPrefabs;

    public Tetromino[] Prefabs => m_TetrominoPrefabs;
    
    protected override void OnInit()
    {
        var grid = new Grid(10, 20);
        RegisterSingleton<IGrid>(grid);
        
        RegisterSingleton<ITetrominoPrefabsProvider>(this);
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