using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class GameController : MonoBehaviour
{
    [SerializeField] private GridFactory m_GridFactory;
    [SerializeField] private TetrominoSpawnerFactory m_TetrominoSpawnerFactory;

    private IGrid m_Grid;
    private List<int> m_CompletedRowsCache;
    private Tetromino m_Tetromino;
    private ITetrominoSpawner m_TetrominoSpawner;
    
    private void Start()
    {
        m_CompletedRowsCache = new();
        m_Grid = m_GridFactory.Create();
        m_TetrominoSpawner = m_TetrominoSpawnerFactory.Create();
        m_Tetromino = m_TetrominoSpawner.Spawn();
        StartCoroutine(MoveDownRoutine());
    }

    private void Update()
    {
        if (m_Tetromino == null)
            return;
        
        if (Input.GetKeyDown(KeyCode.A))
            m_Tetromino.TryMoveLeft();
        else if (Input.GetKeyDown(KeyCode.D))
            m_Tetromino.TryMoveRight();
        else if (Input.GetKeyDown(KeyCode.R))
            m_Tetromino.TryRotate();
        else if (Input.GetKey(KeyCode.S))
            m_Tetromino.TryMoveDown();
    }
    
    private IEnumerator MoveDownRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.5f);
            if (!m_Tetromino.TryMoveDown())
            {
                m_Tetromino.Decompose();
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
