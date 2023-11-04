using System.Runtime.CompilerServices;
using UnityEngine;

[CreateAssetMenu(menuName = "Factories/Grid Factory")]
public sealed class GridFactory : ScriptableObject
{
    private IGrid m_Grid;
    
    public IGrid Create()
    {
        if (m_Grid == null)
        {
            m_Grid = new Grid(10, 22);
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
        var index = ComputeIndex(pos);
        m_Cells[index] = cell;
    }

    public void Clear(Vector2Int pos, ICell cell)
    {
        var index = ComputeIndex(pos);
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