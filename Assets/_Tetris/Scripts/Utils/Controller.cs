using UnityEngine;

public abstract class Controller : MonoBehaviour
{
    [SerializeField] private DiContainer m_DiContainer;

    protected virtual void Awake()
    {
        m_DiContainer.Inject(this);
    }
}
