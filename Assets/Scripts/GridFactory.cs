using UnityEngine;

[CreateAssetMenu(menuName = "Factories/Grid Factory")]
public sealed class GridFactory : ScriptableObject
{
    private IGrid m_Grid;
    
    public IGrid Create()
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
    
    private Cell[] m_Cells;
    
    public Grid(int width, int height)
    {
        Width = width;
        m_Cells = new Cell[width * height];
    }
    
    public void Fill(Vector2Int pos, Cell cell)
    {
        var index = pos.x + pos.y * Width;
        m_Cells[index] = cell;
    }

    public void Clear(Vector2Int pos, Cell cell)
    {
        var index = pos.x + pos.y * Width;
        if (m_Cells[index] == cell)
            m_Cells[index] = null;
    }
}