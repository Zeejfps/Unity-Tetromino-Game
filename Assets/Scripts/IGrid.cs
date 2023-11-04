using UnityEngine;

public interface IGrid
{
    int Width { get; }
    int Height { get; }

    void Fill(Vector2Int pos, ICell cell);
    void Clear(Vector2Int pos, ICell cell);
    ICell GetAndClear(int x, int y);
    bool IsOccupied(int x, int y);
    bool IsOccupied(Vector2Int pos);
}