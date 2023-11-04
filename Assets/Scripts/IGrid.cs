using UnityEngine;

public interface IGrid
{
    int Width { get; }
    
    void Fill(Vector2Int pos, ICell cell);
    void Clear(Vector2Int pos, ICell cell);
    bool IsOccupied(Vector2Int pos);
}