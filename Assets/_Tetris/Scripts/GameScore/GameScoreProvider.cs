using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Providers/Game Score Provider")]
public sealed class GameScoreProvider : ScriptableObject
{
    private IGameScore m_GameScore;
    
    public IGameScore Get()
    {
        if (m_GameScore == null)
        {
            m_GameScore = new GameScore();
        }

        return m_GameScore;
    }    
}

sealed class GameScore : IGameScore
{
    public event Action<int> PointsChanged;

    private int m_TotalPoints;
    public int TotalPoints
    {
        get => m_TotalPoints;
        private set
        {
            if (m_TotalPoints == value)
                return;

            var prevTotalPoints = m_TotalPoints;
            m_TotalPoints = value;
            OnTotalPointsChanged(m_TotalPoints);
        }
    }

    public void IncreasePoints(int points)
    {
        TotalPoints += points;
    }

    public void ResetPoints()
    {
        TotalPoints = 0;
    }

    private void OnTotalPointsChanged(int totalPoints)
    {
        PointsChanged?.Invoke(totalPoints);
    }
}