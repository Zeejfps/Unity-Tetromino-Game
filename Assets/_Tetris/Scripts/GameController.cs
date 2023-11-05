using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class GameController : MonoBehaviour
{
    [SerializeField] private GridProvider m_GridProvider;
    [SerializeField] private TetrominoSpawnerProvider m_TetrominoSpawnerProvider;
    [SerializeField] private GameStateMachineProvider m_GameStateMachineProvider;
    [SerializeField] private GameScoreProvider m_GameScoreProvider;
    [SerializeField] private GameInputProvider m_GameInputProvider;
    
    [Header("Settings")]
    [Range(1, 10)]
    [SerializeField] private int m_InitialLevel = 1;
    
    private IGrid m_Grid;
    private List<int> m_CompletedRowsCache;
    private Tetromino m_Tetromino;
    private ITetrominoSpawner m_TetrominoSpawner;
    private IGameStateMachine m_GameStateMachine;
    private IGameScore m_GameScore;
    private IGameInput m_GameInput;

    private int m_Level;
    private int m_Iterations;
    private int m_TotalLinesCleared;
    private WaitForSeconds m_UpdateGameDelay;
    private Coroutine m_UpdateGameRoutine;

    private void Awake()
    {
        Application.targetFrameRate = 60;

        m_CompletedRowsCache = new();
        
        m_Grid = m_GridProvider.Get();
        m_GameInput = m_GameInputProvider.Get();
        m_GameScore = m_GameScoreProvider.Get();
        m_TetrominoSpawner = m_TetrominoSpawnerProvider.Get();
        m_GameStateMachine = m_GameStateMachineProvider.Get();
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
    
    private void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus && m_GameStateMachine.State == GameState.Playing)
            m_GameStateMachine.TransitionTo(GameState.Paused);
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        if (m_GameStateMachine.State == GameState.Playing)
            m_GameStateMachine.TransitionTo(GameState.Paused);
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

    private async void InstantDropInput_OnPerformed(IInput input)
    {
        if (m_Tetromino != null)
        {
            StopCoroutine(m_UpdateGameRoutine);
            await m_Tetromino.DropInstantly();
            m_UpdateGameRoutine = StartCoroutine(UpdateGameRoutine());
        }
    }

    private void GameStateMachine_OnStateChanged(GameState prevstate, GameState currstate)
    {
        if (prevstate == GameState.GameOver && currstate == GameState.Playing)
        {
            OnGameRestarted();
        }
        else if (prevstate == GameState.Paused && currstate == GameState.Playing)
        {
            OnGameResumed();
        }
        else if (currstate == GameState.Paused)
        {
            OnGamePaused();
        }
        else if (currstate == GameState.Playing)
        {
            OnGameStarted();
        }
        else if (currstate == GameState.GameOver)
        {
            OnGameEnded();
        }
    }

    private void OnGameStarted()
    {
        ResetTotalLinesCleared();
        UpdateLevel();
        UpdateDelay();
        SpawnTetromino();
        StartUpdateGameRoutine();
    }

    private void OnGamePaused()
    {
        StopUpdateGameRoutine();
    }

    private void OnGameResumed()
    {
        if (m_Tetromino == null) 
            SpawnTetromino();

        StartUpdateGameRoutine();
    }

    private void OnGameEnded()
    {
        StopUpdateGameRoutine();
    }

    private void OnGameRestarted()
    {
        StartCoroutine(RestartGameRoutine());
    }
    
    private IEnumerator OnTetrominoLanded()
    {
        AwardPointsLanding();
        m_Iterations = 0;
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

    private void SpawnTetromino()
    {
        m_Tetromino = m_TetrominoSpawner.Spawn();
    }

    private void StopUpdateGameRoutine()
    {
        if (m_UpdateGameRoutine != null)
        {
            StopCoroutine(m_UpdateGameRoutine);
            m_UpdateGameRoutine = null;
        }
    }
    
    private void StartUpdateGameRoutine()
    {
        StopUpdateGameRoutine();
        m_UpdateGameRoutine = StartCoroutine(UpdateGameRoutine());
    }

    private void ResetTotalLinesCleared()
    {
        m_TotalLinesCleared = 0;
    }

    private IEnumerator RestartGameRoutine()
    {
        m_GameScore.ResetPoints();
        yield return DestroyAllCells();
        OnGameStarted();
    }

    private IEnumerator DestroyAllCells()
    {
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
    }

    private IEnumerator UpdateGameRoutine()
    {
        while (true)
        {
            yield return m_UpdateGameDelay;
            if (m_Tetromino != null && !m_Tetromino.TryMoveDown())
                yield return OnTetrominoLanded();
            else
                m_Iterations++;
        }
    }

    private void AwardPointsLanding()
    {
        var actualLevel = Mathf.Max(m_InitialLevel, m_Level);
        var pointAward = ( (m_Grid.Height + 1 + (3 * actualLevel)) - m_Iterations );
        m_GameScore.IncreasePoints(pointAward);
    }

    private IEnumerator FindAndClearCompletedRowsRoutine()
    {
        FindCompletedRows();
        UpdateLevel();
        UpdateDelay();
        AwardPoints();
        yield return ClearCompletedRows();
    }

    private void UpdateLevel()
    {
        var linesCleared = m_TotalLinesCleared;
        if (linesCleared <= 0)
        {
            m_Level = 1;
        }
        else if (linesCleared is >= 1 and <= 90)
        {
            m_Level = 1 + ((linesCleared - 1) / 10);
        }
        else
        {
            m_Level = 10;
        }
    }

    private void UpdateDelay()
    {
        var actualLevel = Mathf.Max(m_InitialLevel, m_Level);
        var delayInSeconds = ((11f - actualLevel) * 0.05f);  // [seconds]
        m_UpdateGameDelay = new WaitForSeconds(delayInSeconds);
    }

    private void AwardPoints()
    {
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
    }

    private IEnumerator ClearCompletedRows()
    {
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

        yield return new WaitForSeconds(0.25f);
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

        m_TotalLinesCleared += m_CompletedRowsCache.Count;
    }
}
