using System;

public interface ITetrominoSpawner
{
    event Action<Tetromino> TetrominoSpawned;
    
    Tetromino NextTetrominoPrefab { get; }
    Tetromino Spawn();
}