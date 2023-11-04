using System;
using System.Collections;
using UnityEngine;

public sealed class Tetromino : MonoBehaviour
{
    [SerializeField] private GridFactory m_GridFactory;
    [SerializeField] private Cell[] m_Cells;

    private Vector3[] m_Offsets;
    private IGrid m_Grid;
    
    private void Start()
    {
        var cellCount = m_Cells.Length;
        m_Offsets = new Vector3[cellCount];
        for (var i = 0; i < cellCount; i++)
        {
            var cell = m_Cells[i];
            var cellPosition = cell.transform.localPosition;
            var xOffset = Mathf.Round(cellPosition.x);
            var yOffset = Mathf.Round(cellPosition.y);
            m_Offsets[i] = new Vector3(xOffset, yOffset, 0f);
            //Debug.Log($"Offset[{i}]: {m_Offsets[i]}");
        }

        m_Grid = m_GridFactory.Create();
        FillGridAtMyPosition();
        StartCoroutine(MoveDownRoutine());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
            TryMoveLeft();
        else if (Input.GetKeyDown(KeyCode.D))
            TryMoveRight();
    }

    private IEnumerator MoveDownRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.5f);
            MoveDown();
        }
    }

    private void TryMoveLeft()
    {
        if (CanMoveLeft())
            MoveLeft();
    }

    private void TryMoveRight()
    {
        if (CanMoveRight())
            MoveRight();
    }

    private bool CanMoveRight()
    {
        var cellCount = m_Cells.Length;
        var myPosition = transform.position;
        for (var i = 0; i < cellCount; i++)
        {
            var offset = m_Offsets[i];
            var gridPos = WorldToGridPosition(myPosition + offset);
            if (gridPos.x >= m_Grid.Width)
                return false;
        }

        return true;
    }

    private bool CanMoveLeft()
    {
        var cellCount = m_Cells.Length;
        var myPosition = transform.position;
        for (var i = 0; i < cellCount; i++)
        {
            var offset = m_Offsets[i];
            var gridPos = WorldToGridPosition(myPosition + offset);
            if (gridPos.x <= 0)
                return false;
        }

        return true;
    }
    
    private void MoveRight()
    {
        ClearGridAtMyPosition();
        transform.position += Vector3.right;
        FillGridAtMyPosition();
    }

    private void MoveLeft()
    {
        ClearGridAtMyPosition();
        transform.position += Vector3.left;
        FillGridAtMyPosition();
    }
    
    private void MoveDown()
    {
        var myPosition = transform.position;
        if (myPosition.y <= 0)
            return;
        
        ClearGridAtMyPosition();
        
        myPosition += Vector3.down;
        transform.position = myPosition;

        FillGridAtMyPosition();
    }

    private void ClearGridAtMyPosition()
    {
        var cellCount = m_Cells.Length;
        var myPosition = transform.position;
        for (var i = 0; i < cellCount; i++)
        {
            var cell = m_Cells[i];
            var offset = m_Offsets[i];
            var gridPos = WorldToGridPosition(myPosition + offset);
            m_Grid.Clear(gridPos, cell);
        }
    }

    private void FillGridAtMyPosition()
    {
        var cellCount = m_Cells.Length;
        var myPosition = transform.position;
        for (var i = 0; i < cellCount; i++)
        {
            var cell = m_Cells[i];
            var offset = m_Offsets[i];
            var gridPos = WorldToGridPosition(myPosition + offset);
            m_Grid.Fill(gridPos, cell);
        }
    }

    private Vector2Int WorldToGridPosition(Vector3 worldPosition)
    {
        return new Vector2Int
        {
            x = (int)(worldPosition.x + m_Grid.Width * 0.5),
            y = (int)(worldPosition.x + m_Grid.Width * 0.5)
        };
    }
}