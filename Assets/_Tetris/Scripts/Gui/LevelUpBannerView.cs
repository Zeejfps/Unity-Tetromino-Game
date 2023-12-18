using System.Collections;
using EnvDev;
using TMPro;
using UnityEngine;

public sealed class LevelUpBannerView : MonoBehaviour, IAnimate
{
    public Coroutine AnimationRoutine { get; set; }

    [SerializeField] private TMP_Text m_Text;
    [Header("Settings")]
    [SerializeField] private float m_VisibleFor = 1.25f;
    
    public void FlashAnimated()
    {
        var t = transform;
        var scale = t.localScale;
        scale.y = 0f;
        t.localScale = scale;

        var color = m_Text.color;
        color.a = 0f;
        m_Text.color = color;
        
        gameObject.SetActive(true);
        StartCoroutine(FlashRoutine());
    }

    private IEnumerator FlashRoutine()
    {
        var tr = transform;
        var startScale = tr.localScale;
        var targetScale = startScale;
        targetScale.y = 1f;
        yield return this.Animate(0.25f, EaseFunctions.CubicOut, t =>
        {
            tr.localScale = Vector3.LerpUnclamped(startScale, targetScale, t);
        });

        var startColor = m_Text.color;
        var targetColor = startColor;
        targetColor.a = 1f;
        yield return this.Animate(0.25f, EaseFunctions.CubicOut, t =>
        {
            m_Text.color = Color.LerpUnclamped(startColor, targetColor, t);
        });
        
        yield return new WaitForSeconds(m_VisibleFor);
        
        yield return this.Animate(0.25f, EaseFunctions.CubicOut, t =>
        {
            m_Text.color = Color.LerpUnclamped(targetColor, startColor, t);
        });
        
        yield return this.Animate(0.25f, EaseFunctions.CubicOut, t =>
        {
            tr.localScale = Vector3.LerpUnclamped(targetScale, startScale, t);
        });
        
        gameObject.SetActive(false);
    }
}
