using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class GameController : MonoBehaviour
{
    [SerializeField] private GridFactory m_GridFactory;
    [SerializeField] private TetrominoSpawnerFactory m_TetrominoSpawnerFactory;
    [SerializeField] private TouchGestureDetectorProvider m_TouchGestureDetectorProvider;
    [SerializeField] private GameStateMachineProvider m_GameStateMachineProvider;
    
    private IGrid m_Grid;
    private List<int> m_CompletedRowsCache;
    private Tetromino m_Tetromino;
    private ITetrominoSpawner m_TetrominoSpawner;
    private ITouchGestureDetector m_TouchGestureDetector;
    private IGameStateMachine m_GameStateMachine;
    
    private void Start()
    {
        Application.targetFrameRate = 60;
        
        m_TouchGestureDetector = m_TouchGestureDetectorProvider.Get();
        m_CompletedRowsCache = new();
        m_Grid = m_GridFactory.Create();
        m_TetrominoSpawner = m_TetrominoSpawnerFactory.Create();
        m_GameStateMachine = m_GameStateMachineProvider.Get();
        
        m_GameStateMachine.StateChanged += GameStateMachine_OnStateChanged;
    }

    private void GameStateMachine_OnStateChanged(GameState prevstate, GameState currstate)
    {
        if (currstate == GameState.Playing)
        {
            StartGame();
        }
    }

    private void StartGame()
    {
        m_Tetromino = m_TetrominoSpawner.Spawn();
        StartCoroutine(MoveDownRoutine());
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
            if (!m_Tetromino.TryMoveDown())
            {
                m_Tetromino.DecomposeAndDestroy();
                m_Tetromino = null;
                yield return FindAndClearCompletedRowsRoutine();
                m_Tetromino = m_TetrominoSpawner.Spawn();
            }
        }
    }

    private IEnumerator FindAndClearCompletedRowsRoutine()
    {
        FindCompletedRows();
        
        //Debug.Log($"Completed Rows: {m_CompletedRowsCache.Count}");
        
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
