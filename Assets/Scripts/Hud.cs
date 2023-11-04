using System;
using TMPro;
using UnityEngine;

public sealed class Hud : MonoBehaviour
{
    [SerializeField] private GameScoreProvider m_GameScoreProvider;
    [SerializeField] private TMP_Text m_ScoreText;

    private IGameScore m_GameScore;

    private void Awake()
    {
        m_GameScore = m_GameScoreProvider.Get();
    }

    private void Start()
    {
        m_GameScore.PointsChanged += GameScore_OnPointsChanged;
    }

    private void OnDestroy()
    {
        m_GameScore.PointsChanged -= GameScore_OnPointsChanged;
    }

    private void GameScore_OnPointsChanged(int pointsScored)
    {
        m_ScoreText.text = m_GameScore.TotalPoints.ToString();
    }
}
