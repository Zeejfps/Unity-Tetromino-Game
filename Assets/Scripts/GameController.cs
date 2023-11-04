using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public sealed class GameController : MonoBehaviour
{
    [FormerlySerializedAs("m_GridFactory")] 
    [SerializeField] private GridProvider m_GridProvider;
    [FormerlySerializedAs("m_TetrominoSpawnerFactory")] 
    [SerializeField] private TetrominoSpawnerProvider m_TetrominoSpawnerProvider;
    [SerializeField] private TouchGestureDetectorProvider m_TouchGestureDetectorProvider;
    [SerializeField] private GameStateMachineProvider m_GameStateMachineProvider;
    [SerializeField] private GameScoreProvider m_GameScoreProvider;
    
    private IGrid m_Grid;
    private List<int> m_CompletedRowsCache;
    private Tetromino m_Tetromino;
    private ITetrominoSpawner m_TetrominoSpawner;
    private ITouchGestureDetector m_TouchGestureDetector;
    private IGameStateMachine m_GameStateMachine;
    private IGameScore m_GameScore;
    
    private Coroutine m_MoveDownRoutine;

    private void Awake()
    {
        Application.targetFrameRate = 60;

        m_CompletedRowsCache = new();
        
        m_Grid = m_GridProvider.Get();
        m_GameScore = m_GameScoreProvider.Get();
        m_TetrominoSpawner = m_TetrominoSpawnerProvider.Get();
        m_GameStateMachine = m_GameStateMachineProvider.Get();
        m_TouchGestureDetector = m_TouchGestureDetectorProvider.Get();
    }

    private void Start()
    {
        m_GameStateMachine.StateChanged += GameStateMachine_OnStateChanged;
    }

    private void OnDestroy()
    {
        m_GameStateMachine.StateChanged -= GameStateMachine_OnStateChanged;
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

    private void Update()
    {
        if (m_Tetromino == null)
            return;
        
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow) || m_TouchGestureDetector.SwipeLeftDetected())
            m_Tetromino.TryMoveLeft();
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow) || m_TouchGestureDetector.SwipeRightDetected())
            m_Tetromino.TryMoveRight();
        else if (Input.GetKeyDown(KeyCode.R) || Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W) || m_TouchGestureDetector.TouchDetected())
            m_Tetromino.TryRotate();
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            m_Tetromino.TryMoveDown();
        else if (Input.GetKeyDown(KeyCode.Space) || m_TouchGestureDetector.SwipeDownDetected())
        {
            var canMoveDown = true;
            do
            {
                canMoveDown = m_Tetromino.TryMoveDown();
            } while (canMoveDown);
        }
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
        
        //Debug.Log($"Completed Rows: {m_CompletedRowsCache.Count}");

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
                //Debug.Log($"Cleared Row: {y}");
            }

            yield return new WaitForSeconds(0.05f);
        }

        for (var i = 0; i < m_CompletedRowsCache.Count; i++)
        {
            for (var x = 0; x < m_Grid.Width; x++)
            {
                var yStart = m_CompletedRowsCache[0];
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
