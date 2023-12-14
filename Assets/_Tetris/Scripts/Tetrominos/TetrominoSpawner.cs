using System;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

sealed class TetrominoSpawner : ITetrominoSpawner
{
    private readonly IGrid m_Grid;
    private readonly ITetrominoPrefabsProvider m_TetrominoPrefabsProvider;

    public event Action<Tetromino> TetrominoSpawned;
    public Tetromino NextTetrominoPrefab { get; private set; }

    public TetrominoSpawner(IGrid grid, ITetrominoPrefabsProvider tetrominoPrefabsProvider)
    {
        m_Grid = grid;
        m_TetrominoPrefabsProvider = tetrominoPrefabsProvider;
        PickNextTetromino();
    }

    public Tetromino Spawn()
    {
        // NOTE(Zee): This assumes the tetromino origin is the top left
        var spawnPosY = m_Grid.Height;
        var tetrominoPrefab = NextTetrominoPrefab;
        var tetromino = Object.Instantiate(tetrominoPrefab, new Vector3(0, spawnPosY, 0), Quaternion.identity);
        PickNextTetromino();
        TetrominoSpawned?.Invoke(tetromino);
        return tetromino;
    }

    private void PickNextTetromino()
    {
        var prefabs = m_TetrominoPrefabsProvider.TetrominoPrefabs;
        var rand = Random.Range(0, prefabs.Length);
        NextTetrominoPrefab = prefabs[rand];
    }
}