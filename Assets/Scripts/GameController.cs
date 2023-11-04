using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] private GridFactory m_GridFactory;
    [SerializeField] private Tetromino m_TetrominoPrefab;
    [SerializeField] private Tetromino m_Tetromino;

    private IGrid m_Grid;
    private List<int> m_CompletedRowsCache;
    
    private void Start()
    {
        m_Grid = m_GridFactory.Create();
        m_CompletedRowsCache = new();
        StartCoroutine(MoveDownRoutine());
    }

    private void Update()
    {
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
                FindAndClearCompletedRows();
                m_Tetromino.Decompose();
                m_Tetromino = Instantiate(m_TetrominoPrefab);
            }
        }
    }

    private void FindAndClearCompletedRows()
    {
        FindCompletedRows();
        
        //Debug.Log($"Completed Rows: {m_CompletedRowsCache.Count}");
        foreach (var y in m_CompletedRowsCache)
        {
            for (var x = 0; x < m_Grid.Width; x++)
            {
                var cell = m_Grid.GetAndClear(x, y);
                cell.Destroy();
            }
            //Debug.Log($"Cleared Row: {y}");
        }

        for (var i = 0; i < m_CompletedRowsCache.Count; i++)
        {
            var yStart = m_CompletedRowsCache[0];
            for (var y = yStart + 1; y < m_Grid.Height; y++)
            {
                for (var x = 0; x < m_Grid.Width; x++)
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
