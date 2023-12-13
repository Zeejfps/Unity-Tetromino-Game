using System.Collections.Generic;
using UnityEngine;

sealed class TetrominoSpawner : ITetrominoSpawner
{
    private readonly IGrid m_Grid;
    private readonly List<Tetromino> m_TetrominoPrefabs;

    public TetrominoSpawner(IGrid grid, IEnumerable<Tetromino> tetrominoPrefabs)
    {
        m_Grid = grid;
        m_TetrominoPrefabs = new List<Tetromino>(tetrominoPrefabs);
    }
    
    public Tetromino Spawn()
    {
        // NOTE(Zee): This assumes the tetromino origin is the top left
        var spawnPosY = m_Grid.Height;
        var rand = Random.Range(0, m_TetrominoPrefabs.Count);
        var tetrominoPrefab = m_TetrominoPrefabs[rand];
        var tetromino = Object.Instantiate(tetrominoPrefab, new Vector3(0, spawnPosY, 0), Quaternion.identity);
        return tetromino;
    }
}