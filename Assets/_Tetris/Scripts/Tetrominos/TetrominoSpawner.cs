using UnityEngine;

sealed class TetrominoSpawner : ITetrominoSpawner
{
    private readonly IGrid m_Grid;
    private readonly ITetrominoPrefabsProvider m_TetrominoPrefabsProvider;

    public TetrominoSpawner(IGrid grid, ITetrominoPrefabsProvider tetrominoPrefabsProvider)
    {
        m_Grid = grid;
        m_TetrominoPrefabsProvider = tetrominoPrefabsProvider;
    }
    
    public Tetromino Spawn()
    {
        // NOTE(Zee): This assumes the tetromino origin is the top left
        var prefabs = m_TetrominoPrefabsProvider.TetrominoPrefabs;
        var spawnPosY = m_Grid.Height;
        var rand = Random.Range(0, prefabs.Length);
        var tetrominoPrefab = prefabs[rand];
        var tetromino = Object.Instantiate(tetrominoPrefab, new Vector3(0, spawnPosY, 0), Quaternion.identity);
        return tetromino;
    }
}