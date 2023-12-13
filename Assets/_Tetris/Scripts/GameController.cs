using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class GameController : Controller
{
    [Header("Settings")]
    [Range(1, 10)]
    [SerializeField] private int m_InitialLevel = 1;

    [Injected] public ITetrominoSpawner TetrominoSpawner { get; set; }
    [Injected] public IGrid Grid { get; set; }
    [Injected] public IGameStateMachine GameStateMachine { get; set; }
    [Injected] public IGameScore GameScore { get; set; }
    [Injected] public MoveLeftInputAction MoveLeft { get; set; }
    [Injected] public MoveRightInputAction MoveRight { get; set; }
    [Injected] public MoveDownInputAction MoveDown { get; set; }
    [Injected] public RotateInputAction Rotate { get; set; }
    [Injected] public InstantDropInputAction InstantDrop { get; set; }
    [Injected] public MainInputActions MainInputActions { get; set; }

    private readonly List<int> m_CompletedRowsCache = new();
    private Tetromino m_Tetromino;
    private int m_Level;
    private int m_Iterations;
    private int m_TotalLinesCleared;
    private WaitForSeconds m_UpdateGameDelay;
    private Coroutine m_UpdateGameRoutine;

    private void Start()
    {
        Application.targetFrameRate = 60;
        GameStateMachine.StateChanged += GameStateMachine_OnStateChanged;
        MoveLeft.Triggered += MoveLeftInput_OnPerformed;
        MoveRight.Triggered += MoveRightInput_OnPerformed;
        MoveDown.Triggered += MoveDownInput_OnPerformed;
        Rotate.Triggered += RotateInput_OnPerformed;
        InstantDrop.Triggered += InstantDropInput_OnPerformed;
    }

    private void OnDestroy()
    {
        GameStateMachine.StateChanged -= GameStateMachine_OnStateChanged;
        MoveLeft.Triggered -= MoveLeftInput_OnPerformed;
        MoveRight.Triggered -= MoveRightInput_OnPerformed;
        MoveDown.Triggered -= MoveDownInput_OnPerformed;
        Rotate.Triggered -= RotateInput_OnPerformed;
        InstantDrop.Triggered -= InstantDropInput_OnPerformed;
    }
    
    private void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus && GameStateMachine.State == GameState.Playing)
            GameStateMachine.TransitionTo(GameState.Paused);
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        if (GameStateMachine.State == GameState.Playing)
            GameStateMachine.TransitionTo(GameState.Paused);
    }
    
    private void OnGameStarted()
    {
        MainInputActions.Enable();
        ResetTotalLinesCleared();
        UpdateLevel();
        UpdateDelay();
        SpawnTetromino();
        StartUpdateGameRoutine();
    }

    private void OnGamePaused()
    {
        MainInputActions.Disable();
        StopUpdateGameRoutine();
    }

    private void OnGameResumed()
    {
        MainInputActions.Enable();
        if (m_Tetromino == null) 
            SpawnTetromino();

        StartUpdateGameRoutine();
    }

    private void OnGameEnded()
    {
        MainInputActions.Disable();
        StopUpdateGameRoutine();
    }

    private void OnGameRestarted()
    {
        StartCoroutine(RestartGameRoutine());
    }
    
    private IEnumerator OnTetrominoLanded()
    {
        AwardPointsForLanding();
        m_Iterations = 0;
        m_Tetromino.DecomposeAndDestroy();
        m_Tetromino = null;
        yield return FindAndClearCompletedRowsRoutine();
        m_Tetromino = TetrominoSpawner.Spawn();
        if (!m_Tetromino.IsInValidPosition())
        {
            m_Tetromino.Destroy();
            m_Tetromino = null;
            GameStateMachine.TransitionTo(GameState.GameOver);
        }
    }

    private void RotateInput_OnPerformed(IInputAction input)
    {
        if (m_Tetromino != null)
            m_Tetromino.TryRotate();
    }

    private void MoveLeftInput_OnPerformed(IInputAction input)
    {
        if (m_Tetromino != null)
            m_Tetromino.TryMoveLeft();
    }
    
    private void MoveRightInput_OnPerformed(IInputAction input)
    {
        if (m_Tetromino != null)
            m_Tetromino.TryMoveRight();
    }

    private void MoveDownInput_OnPerformed(IInputAction input)
    {
        if (m_Tetromino != null)
            m_Tetromino.TryMoveDown();
    }

    private async void InstantDropInput_OnPerformed(IInputAction input)
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

    private void SpawnTetromino()
    {
        m_Tetromino = TetrominoSpawner.Spawn();
    }
    
    private void StartUpdateGameRoutine()
    {
        StopUpdateGameRoutine();
        m_UpdateGameRoutine = StartCoroutine(UpdateGameRoutine());
    }

    private void StopUpdateGameRoutine()
    {
        if (m_UpdateGameRoutine != null)
        {
            StopCoroutine(m_UpdateGameRoutine);
            m_UpdateGameRoutine = null;
        }
    }
    
    private IEnumerator RestartGameRoutine()
    {
        GameScore.ResetPoints();
        yield return DestroyAllCells();
        OnGameStarted();
    }

    private void ResetTotalLinesCleared()
    {
        m_TotalLinesCleared = 0;
    }

    private IEnumerator DestroyAllCells()
    {
        for (var x = 0; x < Grid.Width; x++)
        {
            for (var y = 0; y < Grid.Height; y++)
            {
                var cell = Grid.GetAndClear(x, y);
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

    private void AwardPointsForLanding()
    {
        var actualLevel = Mathf.Max(m_InitialLevel, m_Level);
        var pointAward = (Grid.Height + 1 + (3 * actualLevel)) - m_Iterations;
        GameScore.IncreasePoints(pointAward);
    }

    private void AwardPointsForCompletedRows()
    {
        var completedRowsCount = m_CompletedRowsCache.Count;
        if (completedRowsCount > 0)
        {
            if (completedRowsCount > 3)
            {
                GameScore.IncreasePoints(800);
            }
            else if (completedRowsCount > 2)
            {
                GameScore.IncreasePoints(500);
            }
            else if (completedRowsCount > 1)
            {
                GameScore.IncreasePoints(300);
            }
            else
            {
                GameScore.IncreasePoints(100);
            }
        }
    }
    
    private IEnumerator FindAndClearCompletedRowsRoutine()
    {
        FindCompletedRows();
        UpdateLevel();
        UpdateDelay();
        AwardPointsForCompletedRows();
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

    private IEnumerator ClearCompletedRows()
    {
        for (var x = 0; x < Grid.Width; x++)
        {
            foreach (var y in m_CompletedRowsCache)
            {
                var cell = Grid.GetAndClear(x, y);
                cell.Destroy();
            }

            yield return new WaitForSeconds(0.05f);
        }
        
        for (var i = m_CompletedRowsCache.Count - 1; i >= 0; i--)
        {
            for (var x = 0; x < Grid.Width; x++)
            {
                var yStart = m_CompletedRowsCache[i];
                for (var y = yStart + 1; y < Grid.Height; y++)
                {
                    var cell = Grid.GetAndClear(x, y);
                    if (cell != null)
                    {
                        cell.MoveDown();
                        Grid.Fill(x, y - 1, cell);
                    }
                }
            }
        }

        yield return new WaitForSeconds(0.25f);
    }

    private void FindCompletedRows()
    {
        m_CompletedRowsCache.Clear();
        for (var y = 0; y < Grid.Height; y++)
        {
            var isRowComplete = true;
            for (var x = 0; x < Grid.Width; x++)
            {
                if (!Grid.IsOccupied(x, y))
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
