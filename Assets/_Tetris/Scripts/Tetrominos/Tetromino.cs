using System;
using UnityEngine;

public sealed class Tetromino : MonoBehaviour
{
    [SerializeField] private GridProvider m_GridFactory;
    [SerializeField] private Transform m_Pivot;
    
    private Vector3[] m_Offsets;
    private IGrid m_Grid;
    private Cell[] m_Cells;

    private void Awake()
    {
        m_Cells = GetComponentsInChildren<Cell>();
        m_Grid = m_GridFactory.Get();

        var cellCount = m_Cells.Length;
        m_Offsets = new Vector3[cellCount];
        CalculateOffsets();
    }

    private void Start()
    {
        FillGridAtMyPosition();
    }

    public bool TryMoveLeft()
    {
        ClearGridAtMyPosition();
        transform.position += Vector3.left;
        if (IsInValidPosition())
        {
            FillGridAtMyPosition();
            return true;
        }

        transform.position += Vector3.right;
        FillGridAtMyPosition();
        return false;
    }

    public bool TryMoveRight()
    {
        ClearGridAtMyPosition();
        transform.position += Vector3.right;
        if (IsInValidPosition())
        {
            FillGridAtMyPosition();
            return true;
        }

        transform.position += Vector3.left;
        FillGridAtMyPosition();
        return false;
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
            x = (int)(Mathf.Floor(worldPosition.x) + m_Grid.Width * 0.5),
            y = (int)(Mathf.Floor(worldPosition.y))
        };
    }

    public bool TryMoveDown()
    {
        ClearGridAtMyPosition();
        
        var myTransform = transform;
        var prevPosition = myTransform.position;
        myTransform.position += Vector3.down;
        
        if (IsInValidPosition())
        {
            FillGridAtMyPosition();
            return true;
        }
        
        myTransform.position = prevPosition;
        FillGridAtMyPosition();
        return false;
    }

    public bool TryRotate()
    {
        ClearGridAtMyPosition();
        m_Pivot.Rotate(Vector3.forward, -90f);
        CalculateOffsets();

        if (IsInValidPosition())
        {
            FillGridAtMyPosition();
            return true;
        }
        
        // Undo rotation
        m_Pivot.Rotate(Vector3.forward, 90f);
        CalculateOffsets();
        FillGridAtMyPosition();
        return false;
    }

    public bool IsInValidPosition()
    {
        var cellCount = m_Cells.Length;
        var myPosition = transform.position;
        for (var i = 0; i < cellCount; i++)
        {
            var offset = m_Offsets[i];
            var gridPos = WorldToGridPosition(myPosition + offset);
            if (gridPos.y < 0)
                return false;
            if (gridPos.x >= m_Grid.Width)
                return false;
            if (gridPos.x < 0)
                return false;
            if (m_Grid.IsOccupied(gridPos))
                return false;
        }

        return true;
    }

    private void CalculateOffsets()
    {
        var cellCount = m_Cells.Length;
        var myPosition = transform.position;
        for (var i = 0; i < cellCount; i++)
        {
            var cell = m_Cells[i];
            var cellPosition = cell.transform.position;
            var xOffset = cellPosition.x - myPosition.x;
            var yOffset = cellPosition.y - myPosition.y;
            m_Offsets[i] = new Vector3(xOffset, yOffset, 0f);
            //Debug.Log($"Offset[{i}]: {m_Offsets[i]}");
        }
    }

    public void DecomposeAndDestroy()
    {
        var myParent = transform.parent;
        foreach (var cell in m_Cells)
            cell.transform.SetParent(myParent);
        
        Destroy(gameObject);
    }

    public void Destroy()
    {
        var go = gameObject;
        go.SetActive(false);
        Destroy(go);
    }
}