﻿using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Factories/Tetromino Spawner Factory")]
public sealed class TetrominoSpawnerFactory : ScriptableObject
{
    [SerializeField] private GridFactory m_GridFactory;
    [SerializeField] private Tetromino[] m_TetrominoPrefabs;
    
    private ITetrominoSpawner m_TetrominoSpawner;
    
    public ITetrominoSpawner Create()
    {
        if (m_TetrominoSpawner == null)
        {
            var grid = m_GridFactory.Create();
            m_TetrominoSpawner = new TetrominoSpawner(grid, m_TetrominoPrefabs);
        }
        
        return m_TetrominoSpawner;
    }
}

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