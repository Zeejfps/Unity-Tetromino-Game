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
    
    private readonly ICell[] m_Cells;
    
    public Grid(int width, int height)
    {
        Width = width;
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

    public bool IsOccupied(Vector2Int pos)
    {
        var index = ComputeIndex(pos);
        return m_Cells[index] != null;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int ComputeIndex(Vector2Int pos)
    {
        return pos.x + pos.y * Width;
    }
}