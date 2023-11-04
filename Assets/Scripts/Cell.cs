using UnityEngine;

public sealed class Cell : MonoBehaviour, ICell
{
    public void Destroy()
    {
        var go = gameObject;
        go.SetActive(false);
        Destroy(go);
    }
}