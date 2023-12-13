using UnityEngine;

[CreateAssetMenu]
public sealed class TetrisDiContainer : DiContainer
{
    [SerializeField] private Tetromino[] m_TetrominoPrefabs;

    protected override void OnInit()
    {
        var grid = new Grid(10, 20);
        var tetrominoSpawner = new TetrominoSpawner(grid, m_TetrominoPrefabs);
        var gameStateMachine = new TetrisGameStateMachine();
        
        RegisterSingleton<IGrid>(grid);
        RegisterSingleton<ITetrominoSpawner>(tetrominoSpawner);
        RegisterSingleton<IGameStateMachine>(gameStateMachine);
    }
}
