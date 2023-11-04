using UnityEngine;

public interface IGrid
{
    void Fill(Vector2Int pos, Cell cell);
    void Clear(Vector2Int pos, Cell cell);
    int Width { get; }
}