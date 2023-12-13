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
        var gameScore = new GameScore();
        var clock = new UnityClock();
        
        var touchGestureDetector = new LegacyInputSystemTouchGestureDetector(clock);
        touchGestureDetector.Enable();
        
        var moveLeftInput = new MoveLeftInput(clock, gameStateMachine, touchGestureDetector);
        var moveRightInput = new MoveRightInput(clock, gameStateMachine, touchGestureDetector);
        var moveDownInput = new MoveDownInput(clock, gameStateMachine);
        var rotateInput = new RotateInput(clock, gameStateMachine, touchGestureDetector);
        var instantDropInput = new InstantDropInput(clock, gameStateMachine, touchGestureDetector);
        
        var gameInput = new GameInput(
            moveLeftInput,
            moveRightInput,
            moveDownInput,
            rotateInput,
            instantDropInput
        );
        
        RegisterSingleton<IGrid>(grid);
        RegisterSingleton<ITetrominoSpawner>(tetrominoSpawner);
        RegisterSingleton<IGameStateMachine>(gameStateMachine);
        RegisterSingleton<IGameScore>(gameScore);
        RegisterSingleton<ITouchGestureDetector>(touchGestureDetector);
        RegisterSingleton<IGameInput>(gameInput);
    }
}
