using System.Runtime.CompilerServices;
using UnityEngine;

[CreateAssetMenu(menuName = "Factories/Grid Factory")]
public sealed class GridProvider : ScriptableObject
{
    private IGrid m_Grid;
    
    public IGrid Get()
    {
        if (m_Grid == null)
        {
            m_Grid = new Grid(10, 20);
        }

        return m_Grid;
    }
}

sealed class Grid : IGrid
{
    public int Width { get; }
    public int Height { get; }

    private readonly ICell[] m_Cells;
    
    public Grid(int width, int height)
    {
        Width = width;
        Height = height;
        m_Cells = new ICell[width * height];
    }
    
    public void Fill(Vector2Int pos, ICell cell)
    {
        Fill(pos.x, pos.y, cell);
    }

    public void Fill(int x, int y, ICell cell)
    {
        if (x >= Width)
            return;
        
        if (x < 0)
            return;
        
        if (y >= Height)
            return;
        
        if (y < 0)
            return;
        
        var index = ComputeIndex(x, y);
        m_Cells[index] = cell;    
    }

    public void Clear(Vector2Int pos, ICell cell)
    {
       Clear(pos.x, pos.y, cell);
    }

    public void Clear(int x, int y, ICell cell)
    {
        if (x < 0 || x >= Width)
            return;
        
        if (y < 0 || y >= Height)
            return;
        
        var index = ComputeIndex(x, y);
        if (m_Cells[index] == cell)
            m_Cells[index] = null;
    }

    public ICell GetAndClear(int x, int y)
    {
        var index = ComputeIndex(x, y);
        var cell = m_Cells[index];
        m_Cells[index] = null;
        return cell;
    }

    public bool IsOccupied(int x, int y)
    {
        if (x < 0 || x >= Width)
            return false;

        if (y < 0 || y >= Height)
            return false;
        
        var index = ComputeIndex(x, y);
        return m_Cells[index] != null;
    }

    public bool IsOccupied(Vector2Int pos)
    {
        return IsOccupied(pos.x, pos.y);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int ComputeIndex(Vector2Int pos)
    {
        return ComputeIndex(pos.x, pos.y);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int ComputeIndex(int x, int y)
    {
        return x + y * Width;
    }
}