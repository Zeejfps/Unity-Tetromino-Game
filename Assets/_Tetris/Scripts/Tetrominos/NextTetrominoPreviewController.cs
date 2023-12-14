public sealed class NextTetrominoPreviewController : Controller
{
    [Injected] public ITetrominoSpawner TetrominoSpawner { get; set; }

    private Tetromino m_NextTetromino;
    
    private void Start()
    {
        UpdateNextTetrominoPreview();
        TetrominoSpawner.TetrominoSpawned += TetrominoSpawner_OnTetrominoSpawned;
    }

    private void OnDestroy()
    {
        TetrominoSpawner.TetrominoSpawned -= TetrominoSpawner_OnTetrominoSpawned;
    }

    private void TetrominoSpawner_OnTetrominoSpawned(Tetromino tetromino)
    {
        UpdateNextTetrominoPreview();
    }

    private void UpdateNextTetrominoPreview()
    {
        if (m_NextTetromino != null)
        {
            var go = m_NextTetromino.gameObject;
            go.SetActive(false);
            Destroy(go);
            m_NextTetromino = null;
        }
        var nextTetrominoPrefab = TetrominoSpawner.NextTetrominoPrefab;
        m_NextTetromino = Instantiate(nextTetrominoPrefab, transform);
        m_NextTetromino.ShowPreview = false;
    }
}
