using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class GameController : MonoBehaviour
{
    [SerializeField] private GridProvider m_GridProvider;
    [SerializeField] private TetrominoSpawnerProvider m_TetrominoSpawnerProvider;
    [SerializeField] private TouchGestureDetectorProvider m_TouchGestureDetectorProvider;
    [SerializeField] private GameStateMachineProvider m_GameStateMachineProvider;
    [SerializeField] private GameScoreProvider m_GameScoreProvider;
    [SerializeField] private GameInputProvider m_GameInputProvider;
    
    private IGrid m_Grid;
    private List<int> m_CompletedRowsCache;
    private Tetromino m_Tetromino;
    private ITetrominoSpawner m_TetrominoSpawner;
    private ITouchGestureDetector m_TouchGestureDetector;
    private IGameStateMachine m_GameStateMachine;
    private IGameScore m_GameScore;
    private IGameInput m_GameInput;
    
    private Coroutine m_MoveDownRoutine;

    private void Awake()
    {
        Application.targetFrameRate = 60;

        m_CompletedRowsCache = new();
        
        m_Grid = m_GridProvider.Get();
        m_GameInput = m_GameInputProvider.Get();
        m_GameScore = m_GameScoreProvider.Get();
        m_TetrominoSpawner = m_TetrominoSpawnerProvider.Get();
        m_GameStateMachine = m_GameStateMachineProvider.Get();
        m_TouchGestureDetector = m_TouchGestureDetectorProvider.Get();
    }

    private void Start()
    {
        m_GameStateMachine.StateChanged += GameStateMachine_OnStateChanged;
        m_GameInput.MoveLeft.Performed += MoveLeftInput_OnPerformed;
        m_GameInput.MoveRight.Performed += MoveRightInput_OnPerformed;
        m_GameInput.MoveDown.Performed += MoveDownInput_OnPerformed;
        m_GameInput.Rotate.Performed += RotateInput_OnPerformed;
        m_GameInput.InstantDrop.Performed += InstantDropInput_OnPerformed;
    }

    private void OnDestroy()
    {
        m_GameStateMachine.StateChanged -= GameStateMachine_OnStateChanged;
        m_GameInput.MoveLeft.Performed -= MoveLeftInput_OnPerformed;
        m_GameInput.MoveRight.Performed -= MoveRightInput_OnPerformed;
        m_GameInput.MoveDown.Performed -= MoveDownInput_OnPerformed;
        m_GameInput.Rotate.Performed -= RotateInput_OnPerformed;
        m_GameInput.InstantDrop.Performed -= InstantDropInput_OnPerformed;
    }

    private void RotateInput_OnPerformed(IInput input)
    {
        if (m_Tetromino != null)
            m_Tetromino.TryRotate();
    }

    private void MoveLeftInput_OnPerformed(IInput input)
    {
        if (m_Tetromino != null)
            m_Tetromino.TryMoveLeft();
    }
    
    private void MoveRightInput_OnPerformed(IInput input)
    {
        if (m_Tetromino != null)
            m_Tetromino.TryMoveRight();
    }

    private void MoveDownInput_OnPerformed(IInput input)
    {
        if (m_Tetromino != null)
            m_Tetromino.TryMoveDown();
    }

    private void InstantDropInput_OnPerformed(IInput input)
    {
        if (m_Tetromino != null)
        {
            bool canMoveDown;
            do
            {
                canMoveDown = m_Tetromino.TryMoveDown();
            } while (canMoveDown);
        }
    }

    private void GameStateMachine_OnStateChanged(GameState prevstate, GameState currstate)
    {
        if (prevstate == GameState.GameOver && currstate == GameState.Playing)
        {
            RestartGame();
        }
        else if (currstate == GameState.Playing)
        {
            StartGame();
        }
        else if (currstate == GameState.GameOver)
        {
            if (m_MoveDownRoutine != null)
            {
                StopCoroutine(m_MoveDownRoutine);
                m_MoveDownRoutine = null;
            }
        }
    }

    private void StartGame()
    {
        m_Tetromino = m_TetrominoSpawner.Spawn();
        m_MoveDownRoutine = StartCoroutine(MoveDownRoutine());
    }

    private void RestartGame()
    {
        StartCoroutine(RestartGameRoutine());
    }

    private IEnumerator RestartGameRoutine()
    {
        m_GameScore.ResetPoints();
        for (var x = 0; x < m_Grid.Width; x++)
        {
            for (var y = 0; y < m_Grid.Height; y++)
            {
                var cell = m_Grid.GetAndClear(x, y);
                if (cell != null)
                {
                    cell.Destroy();
                }
            }
        }
        yield return new WaitForSeconds(0.25f);
        StartGame();
    }

    private IEnumerator MoveDownRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.5f);
            if (m_Tetromino != null && !m_Tetromino.TryMoveDown())
            {
                m_Tetromino.DecomposeAndDestroy();
                m_Tetromino = null;
                yield return FindAndClearCompletedRowsRoutine();
                m_Tetromino = m_TetrominoSpawner.Spawn();
                if (!m_Tetromino.IsInValidPosition())
                {
                    m_Tetromino.Destroy();
                    m_Tetromino = null;
                    m_GameStateMachine.TransitionTo(GameState.GameOver);
                }
            }
        }
    }

    private IEnumerator FindAndClearCompletedRowsRoutine()
    {
        FindCompletedRows();
        
        var completedRowsCount = m_CompletedRowsCache.Count;
        if (completedRowsCount > 0)
        {
            if (completedRowsCount > 3)
            {
                m_GameScore.IncreasePoints(800);
            }
            else if (completedRowsCount > 2)
            {
                m_GameScore.IncreasePoints(500);
            }
            else if (completedRowsCount > 1)
            {
                m_GameScore.IncreasePoints(300);
            }
            else
            {
                m_GameScore.IncreasePoints(100);
            }
        }
        
        for (var x = 0; x < m_Grid.Width; x++)
        {
            foreach (var y in m_CompletedRowsCache)
            {
                var cell = m_Grid.GetAndClear(x, y);
                cell.Destroy();
            }

            yield return new WaitForSeconds(0.05f);
        }

        for (var i = m_CompletedRowsCache.Count - 1; i >= 0; i--)
        {
            for (var x = 0; x < m_Grid.Width; x++)
            {
                var yStart = m_CompletedRowsCache[i];
                for (var y = yStart + 1; y < m_Grid.Height; y++)
                {
                    var cell = m_Grid.GetAndClear(x, y);
                    if (cell != null)
                    {
                        cell.MoveDown();
                        m_Grid.Fill(x, y - 1, cell);
                    }
                }
            }
        }
    }

    private void FindCompletedRows()
    {
        m_CompletedRowsCache.Clear();
        for (var y = 0; y < m_Grid.Height; y++)
        {
            var isRowComplete = true;
            for (var x = 0; x < m_Grid.Width; x++)
            {
                if (!m_Grid.IsOccupied(x, y))
                {
                    isRowComplete = false;
                    break;
                }
            }
            if (isRowComplete)
                m_CompletedRowsCache.Add(y);
        }
    }
}
