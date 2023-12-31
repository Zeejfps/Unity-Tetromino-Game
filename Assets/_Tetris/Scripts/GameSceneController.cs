using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class GameSceneController : Controller
{
    [SerializeField] private LevelUpBannerView m_LevelUpBannerView;
    
    [Header("Settings")]
    [Range(1, 10)]
    [SerializeField] private int m_InitialLevel = 1;

    [Injected] public IGrid Grid { get; set; }
    [Injected] public IGameScore GameScore { get; set; }
    [Injected] public MainInputActions MainInputActions { get; set; }
    [Injected] public ITetrominoSpawner TetrominoSpawner { get; set; }
    [Injected] public IGameStateMachine GameStateMachine { get; set; }
    [Injected] public PauseResumeInputAction PauseResumeInputAction { get; set; }

    private readonly List<int> m_CompletedRowsCache = new();
    private Tetromino m_Tetromino;
    private int m_Level;
    private int m_Iterations;
    private int m_TotalLinesCleared;
    private WaitForSeconds m_UpdateGameDelay;
    private Coroutine m_UpdateGameRoutine;

    private void Start()
    {
        m_Level = m_InitialLevel;
        Application.targetFrameRate = 60;
        GameStateMachine.StateChanged += OnGameStateMachineStateChanged;
        MainInputActions.MoveLeftInputAction.Triggered += OnMoveLeftInputActionTriggered;
        MainInputActions.MoveRightInputAction.Triggered += OnMoveRightInputActionTriggered;
        MainInputActions.MoveDownInputAction.Triggered += OnMoveDownInputActionTriggered;
        MainInputActions.RotateInputAction.Triggered += OnRotateInputActionTriggered;
        MainInputActions.InstantDropInputAction.Triggered += OnInstantDropInputActionTriggered;
        PauseResumeInputAction.Triggered += OnPauseResumeInputActionTriggered;
        PauseResumeInputAction.Enable();
    }

    private void OnDestroy()
    {
        GameStateMachine.StateChanged -= OnGameStateMachineStateChanged;
        MainInputActions.MoveLeftInputAction.Triggered -= OnMoveLeftInputActionTriggered;
        MainInputActions.MoveRightInputAction.Triggered -= OnMoveRightInputActionTriggered;
        MainInputActions.MoveDownInputAction.Triggered -= OnMoveDownInputActionTriggered;
        MainInputActions.RotateInputAction.Triggered -= OnRotateInputActionTriggered;
        MainInputActions.InstantDropInputAction.Triggered -= OnInstantDropInputActionTriggered;
        PauseResumeInputAction.Triggered -= OnPauseResumeInputActionTriggered;
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
        ResetTotalLinesCleared();
        UpdateLevel();
        UpdateDelay();
        SpawnTetromino();
        StartUpdateGameRoutine();
        MainInputActions.Enable();
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
    
    private IEnumerator OnTetrominoLanded(Action onComplete = default)
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
        else
        {
            onComplete?.Invoke();
        }
    }
    
    private void OnPauseResumeInputActionTriggered(IInputAction inputAction)
    {
        if (GameStateMachine.State == GameState.Playing)
            GameStateMachine.TransitionTo(GameState.Paused);
        else if(GameStateMachine.State == GameState.Paused)
            GameStateMachine.TransitionTo(GameState.Playing);
    }

    private void OnRotateInputActionTriggered(IInputAction input)
    {
        if (m_Tetromino != null)
            m_Tetromino.TryRotate();
    }

    private void OnMoveLeftInputActionTriggered(IInputAction input)
    {
        if (m_Tetromino != null)
            m_Tetromino.TryMoveLeft();
    }
    
    private void OnMoveRightInputActionTriggered(IInputAction input)
    {
        if (m_Tetromino != null)
            m_Tetromino.TryMoveRight();
    }

    private void OnMoveDownInputActionTriggered(IInputAction input)
    {
        if (m_Tetromino != null)
            m_Tetromino.TryMoveDown();
    }

    private async void OnInstantDropInputActionTriggered(IInputAction input)
    {
        if (m_Tetromino != null)
        {
            StopUpdateGameRoutine();
            await m_Tetromino.DropInstantly();
            StartCoroutine(OnTetrominoLanded(() =>
            {
                if (GameStateMachine.State == GameState.Playing)
                    StartUpdateGameRoutine();
            }));
        }
    }

    private void OnGameStateMachineStateChanged(GameState prevstate, GameState currstate)
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
        if (m_Tetromino != null)
        {
            m_Tetromino.DecomposeAndDestroy();
            m_Tetromino = null;
        }
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
        var prevLevel = m_Level;
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

        if (m_Level > prevLevel)
        {
            OnLeveledUp();
        }
    }

    private void OnLeveledUp()
    {
        m_LevelUpBannerView.FlashAnimated();
    }

    private void UpdateDelay()
    {
        var actualLevel = Mathf.Max(m_InitialLevel, m_Level);
        var delayInSeconds = ((11f - actualLevel) * 0.05f);  // [seconds]
        m_UpdateGameDelay = new WaitForSeconds(delayInSeconds);
    }

    private IEnumerator ClearCompletedRows()
    {
        if (m_CompletedRowsCache.Count == 0)
            yield break;
        
        MainInputActions.Disable();
        
        // TODO: This has a huge potential for bugs; 
        // If this is interrupted for any reason 
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
        MainInputActions.Enable();
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
