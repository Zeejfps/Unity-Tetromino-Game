using UnityEngine;

public sealed class Cell : MonoBehaviour, ICell, IAnimate
{
    public void MoveDown()
    {
        transform.position += Vector3.down;
    }

    public void Destroy()
    {
        var startScale = transform.localScale;
        this.Animate(0.25f, t => t, t =>
        {
            transform.localScale = Vector3.LerpUnclamped(startScale, Vector3.zero, t);
        }, () =>
        {
            var go = gameObject;
            go.SetActive(false);
            Destroy(go);
        });
    }

    public Coroutine AnimationRoutine { get; set; }
}