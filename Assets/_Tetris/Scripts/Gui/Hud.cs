using EnvDev;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public sealed class Hud : MonoBehaviour, IAnimate
{
    [SerializeField] private GameScoreProvider m_GameScoreProvider;
    [SerializeField] private GameStateMachineProvider m_GameStateMachineProvider;
    [SerializeField] private Button m_PauseButton;
    [SerializeField] private TMP_Text m_ScoreText;

    private IGameStateMachine m_GameStateMachine;
    private IGameScore m_GameScore;

    private int m_PrevPointCount;
    
    private void Awake()
    {
        m_GameStateMachine = m_GameStateMachineProvider.Get();
        m_GameScore = m_GameScoreProvider.Get();
    }

    private void Start()
    {
        m_PauseButton.onClick.AddListener(PauseButton_OnClicked);
        m_GameScore.PointsChanged += GameScore_OnPointsChanged;
        m_PrevPointCount = m_GameScore.TotalPoints;
        m_ScoreText.text = m_PrevPointCount.ToString();
    }

    private void OnDestroy()
    {
        m_PauseButton.onClick.RemoveListener(PauseButton_OnClicked);
        m_GameScore.PointsChanged -= GameScore_OnPointsChanged;
    }

    private void PauseButton_OnClicked()
    {
        m_GameStateMachine.TransitionTo(GameState.Paused);
    }

    private void GameScore_OnPointsChanged(int pointsScored)
    {
        var targetPointCount = m_GameScore.TotalPoints;
        this.Animate(0.5f, EaseFunctions.SineInOut, t =>
        {
            m_ScoreText.text = Mathf.LerpUnclamped(m_PrevPointCount, targetPointCount, t).ToString("0");
        }, () =>
        {
            m_PrevPointCount = m_GameScore.TotalPoints;
        });
    }

    public Coroutine AnimationRoutine { get; set; }
}
