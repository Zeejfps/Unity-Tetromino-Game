using UnityEngine;

public sealed class Cell : MonoBehaviour, ICell
{
    public void MoveDown()
    {
        transform.position += Vector3.down;
    }

    public void Destroy()
    {
        var go = gameObject;
        go.SetActive(false);
        Destroy(go);
    }
}