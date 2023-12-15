using System;
using EnvDev;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public sealed class ToggleSwitchWidgetView : MonoBehaviour, IAnimate, IPointerClickHandler
{
    public event Action<ToggleSwitchWidgetView> Clicked; 
    
    [SerializeField] private Image m_Background;
    [SerializeField] private RectTransform m_Switch;
    [Header("Style")] 
    [SerializeField] private Color m_OnBackgroundColor = Color.green;
    [SerializeField] private Color m_OffBackgroundColor = Color.red;

    public void UpdateInstantly(bool isOn)
    {
        if (isOn)
        {
            var anchorMin = m_Switch.anchorMin;
            var anchorMax = m_Switch.anchorMax;
            anchorMin.x = 0.5f;
            anchorMax.x = 1.0f;
            m_Switch.anchorMin = anchorMin;
            m_Switch.anchorMax = anchorMax;
            m_Background.color = m_OnBackgroundColor;
        }
        else
        {
            var anchorMin = m_Switch.anchorMin;
            var anchorMax = m_Switch.anchorMax;
            anchorMin.x = 0.0f;
            anchorMax.x = 0.5f;
            m_Switch.anchorMin = anchorMin;
            m_Switch.anchorMax = anchorMax;
            m_Background.color = m_OffBackgroundColor;
        }
    }

    public void UpdateAnimated(bool isOn)
    {
        if (isOn)
        {
            var startAnchorMin = m_Switch.anchorMin;
            var startAnchorMax = m_Switch.anchorMax;
            var targetAnchorMin = startAnchorMin;
            var targetAnchorMax = startAnchorMax;
            targetAnchorMin.x = 0.5f;
            targetAnchorMax.x = 1.0f;
            var startBackgroundColor = m_Background.color;
            var targetBackgroundColor = m_OnBackgroundColor;
            this.Animate(0.2f, EaseFunctions.CubicOut, t =>
            {
                m_Switch.anchorMin = Vector2.LerpUnclamped(startAnchorMin, targetAnchorMin, t);
                m_Switch.anchorMax = Vector2.LerpUnclamped(startAnchorMax, targetAnchorMax, t);
                m_Background.color = Color.LerpUnclamped(startBackgroundColor, targetBackgroundColor, t);
            });
        }
        else
        {
            var startAnchorMin = m_Switch.anchorMin;
            var startAnchorMax = m_Switch.anchorMax;
            var targetAnchorMin = startAnchorMin;
            var targetAnchorMax = startAnchorMax;
            targetAnchorMin.x = 0.0f;
            targetAnchorMax.x = 0.5f;
            var startBackgroundColor = m_Background.color;
            var targetBackgroundColor = m_OffBackgroundColor;
            this.Animate(0.2f, EaseFunctions.CubicOut, t =>
            {
                m_Switch.anchorMin = Vector2.LerpUnclamped(startAnchorMin, targetAnchorMin, t);
                m_Switch.anchorMax = Vector2.LerpUnclamped(startAnchorMax, targetAnchorMax, t);
                m_Background.color = Color.LerpUnclamped(startBackgroundColor, targetBackgroundColor, t);
            });
        }
    }

    public Coroutine AnimationRoutine { get; set; }
    
    public void OnPointerClick(PointerEventData eventData)
    {
        Clicked?.Invoke(this);
    }
}
