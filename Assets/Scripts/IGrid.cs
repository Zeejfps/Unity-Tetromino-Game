using UnityEngine;

public interface IGrid
{
    void Fill(Vector2Int pos, ICell cell);
    void Clear(Vector2Int pos, ICell cell);
    int Width { get; }
}