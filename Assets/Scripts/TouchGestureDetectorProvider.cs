using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Providers/Touch Gesture Detector Provider")]
public sealed class TouchGestureDetectorProvider : ScriptableObject
{
    private ITouchGestureDetector m_TouchGestureDetector;
    
    public ITouchGestureDetector Get()
    {
        if (m_TouchGestureDetector == null)
        {
            var go = new GameObject("[Touch Gesture Detector]");
            m_TouchGestureDetector = go.AddComponent<LegacyInputSystemTouchGestureDetector>();
        }

        return m_TouchGestureDetector;
    }
}