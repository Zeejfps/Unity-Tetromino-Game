using System.Threading.Tasks;
using EnvDev;
using UnityEngine;

public sealed class Tetromino : MonoBehaviour, IAnimate
{
    [SerializeField] private DiContainer m_DiContainer;
    [SerializeField] private Transform m_Pivot;

    private bool m_IsPreview;
    public bool IsPreview
    {
        get => m_IsPreview;
        set
        {
            m_IsPreview = value;
            if (m_IsPreview)
            {
                foreach (var cell in m_Cells)
                {
                    cell.DisplayAsPreview();
                }
            }
        }
    }

    private bool m_ShowPreview = true;
    public bool ShowPreview
    {
        get => m_ShowPreview;
        set
        {
            if (m_ShowPreview == value)
                return;

            m_ShowPreview = value;
            if (m_Initialized)
                UpdatePreview();
        }
    }

    [Injected] public IGrid Grid { get; set; }

    private bool m_Initialized;
    private Vector3[] m_Offsets;
    private Cell[] m_Cells;

    private void Awake()
    {
        m_DiContainer.Inject(this);
        m_Cells = GetComponentsInChildren<Cell>();

        var cellCount = m_Cells.Length;
        m_Offsets = new Vector3[cellCount];
        CalculateOffsets();
    }

    private void Start()
    {
        m_Initialized = true;
        if (!IsPreview)
        {
            FillGridAtMyPosition();
            UpdatePreview();
        }
    }

    public bool TryMoveLeft()
    {
        ClearGridAtMyPosition();
        transform.position += Vector3.left;
        if (IsInValidPosition())
        {
            FillGridAtMyPosition();
            UpdatePreview();
            return true;
        }

        transform.position += Vector3.right;
        FillGridAtMyPosition();
        return false;
    }

    public bool TryMoveRight()
    {
        ClearGridAtMyPosition();
        transform.position += Vector3.right;
        if (IsInValidPosition())
        {
            FillGridAtMyPosition();
            UpdatePreview();
            return true;
        }

        transform.position += Vector3.left;
        FillGridAtMyPosition();
        return false;
    }
    
    public bool TryMoveDown()
    {
        ClearGridAtMyPosition();
        
        var myTransform = transform;
        var prevPosition = myTransform.position;
        myTransform.position += Vector3.down;
        
        if (IsInValidPosition())
        {
            FillGridAtMyPosition();
            return true;
        }
        
        myTransform.position = prevPosition;
        FillGridAtMyPosition();
        return false;
    }

    public bool TryRotate()
    {
        ClearGridAtMyPosition();
        m_Pivot.Rotate(Vector3.forward, -90f);
        CalculateOffsets();

        if (IsInValidPosition())
        {
            FillGridAtMyPosition();
            UpdatePreview();
            return true;
        }
        
        // Undo rotation
        m_Pivot.Rotate(Vector3.forward, 90f);
        CalculateOffsets();
        FillGridAtMyPosition();
        return false;
    }
    
    public Task DropInstantly()
    {
        var startPosition = transform.position;
        bool canMoveDown;
        do
        {
            canMoveDown = TryMoveDown();
        } while (canMoveDown);

        var tsc = new TaskCompletionSource<bool>();
        var targetPosition = transform.position;
        this.Animate(0.1f, EaseFunctions.CubicIn, t =>
        {
            transform.position = Vector3.LerpUnclamped(startPosition, targetPosition, t);
        }, () =>
        {
            tsc.SetResult(true);
        });

        return tsc.Task;
    }

    private void ClearGridAtMyPosition()
    {
        var cellCount = m_Cells.Length;
        var myPosition = transform.position;
        for (var i = 0; i < cellCount; i++)
        {
            var cell = m_Cells[i];
            var offset = m_Offsets[i];
            var gridPos = WorldToGridPosition(myPosition + offset);
            Grid.Clear(gridPos, cell);
        }
    }

    private void FillGridAtMyPosition()
    {
        if (IsPreview)
            return;
        
        var cellCount = m_Cells.Length;
        var myPosition = transform.position;
        for (var i = 0; i < cellCount; i++)
        {
            var cell = m_Cells[i];
            var offset = m_Offsets[i];
            var gridPos = WorldToGridPosition(myPosition + offset);
            Grid.Fill(gridPos, cell);
        }
    }

    private Vector2Int WorldToGridPosition(Vector3 worldPosition)
    {
        return new Vector2Int
        {
            x = (int)(Mathf.Floor(worldPosition.x) + Grid.Width * 0.5),
            y = (int)(Mathf.Floor(worldPosition.y))
        };
    }

    public bool IsInValidPosition()
    {
        var cellCount = m_Cells.Length;
        var myPosition = transform.position;
        for (var i = 0; i < cellCount; i++)
        {
            var offset = m_Offsets[i];
            var gridPos = WorldToGridPosition(myPosition + offset);
            if (gridPos.y < 0)
                return false;
            if (gridPos.x >= Grid.Width)
                return false;
            if (gridPos.x < 0)
                return false;
            if (Grid.IsOccupied(gridPos))
                return false;
        }

        return true;
    }

    private void CalculateOffsets()
    {
        var cellCount = m_Cells.Length;
        var myPosition = transform.position;
        for (var i = 0; i < cellCount; i++)
        {
            var cell = m_Cells[i];
            var cellPosition = cell.transform.position;
            var xOffset = cellPosition.x - myPosition.x;
            var yOffset = cellPosition.y - myPosition.y;
            m_Offsets[i] = new Vector3(xOffset, yOffset, 0f);
            //Debug.Log($"Offset[{i}]: {m_Offsets[i]}");
        }
    }

    private void UpdatePreview()
    {
        if (m_ShowPreview)
        {
            var previewPosition = ComputePreviewPosition();
            DisplayPreview(previewPosition);
        }
        else
        {
            ClearPreview();
        }
    }

    private Vector3 ComputePreviewPosition()
    {
        var currentPosition = transform.position;
        bool canMoveDown;
        do
        {
            canMoveDown = TryMoveDown();
        } while (canMoveDown);

        var previewPosition = transform.position;
        previewPosition.z += 1f;
        
        ClearGridAtMyPosition();
        transform.position = currentPosition;
        FillGridAtMyPosition();

        return previewPosition;
    }

    private Tetromino m_Preview;
    
    private void DisplayPreview(Vector3 previewPosition)
    {
        if (m_Preview == null)
        {
            m_Preview = Instantiate(this, previewPosition, Quaternion.identity, transform.parent);
            m_Preview.m_Pivot.rotation = m_Pivot.rotation;
            m_Preview.IsPreview = true;
        }
        else
        {
            m_Preview.transform.position = previewPosition;
            m_Preview.m_Pivot.rotation = m_Pivot.rotation;
        }
    }

    private void ClearPreview()
    {
        if (m_Preview == null)
            return;

        var go = m_Preview.gameObject;
        go.SetActive(false);
        Destroy(go);
        m_Preview = null;
    }

    public void DecomposeAndDestroy()
    {
        if (m_Preview != null)
        {
            var previewGo = m_Preview.gameObject;
            previewGo.SetActive(false);
            Destroy(previewGo);
        }
        
        var myParent = transform.parent;
        foreach (var cell in m_Cells)
            cell.transform.SetParent(myParent);
        
        Destroy(gameObject);
    }

    public void Destroy()
    {
        var go = gameObject;
        go.SetActive(false);
        Destroy(go);
    }

    public Coroutine AnimationRoutine { get; set; }
}