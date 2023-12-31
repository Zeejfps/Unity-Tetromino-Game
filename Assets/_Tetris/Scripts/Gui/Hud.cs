using EnvDev;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public sealed class Hud : MonoBehaviour, IAnimate
{
    [SerializeField] private DiContainer m_DiContainer;
    [SerializeField] private Button m_PauseButton;
    [SerializeField] private TMP_Text m_ScoreText;

    [Injected] public IGameStateMachine GameStateMachine { get; set; }
    [Injected] public IGameScore GameScore { get; set; }

    private int m_PrevPointCount;
    
    private void Awake()
    {
        m_DiContainer.Inject(this);
    }

    private void Start()
    {
        m_PauseButton.onClick.AddListener(PauseButton_OnClicked);
        GameScore.PointsChanged += GameScore_OnPointsChanged;
        m_PrevPointCount = GameScore.TotalPoints;
        m_ScoreText.text = m_PrevPointCount.ToString();
    }

    private void OnDestroy()
    {
        m_PauseButton.onClick.RemoveListener(PauseButton_OnClicked);
        GameScore.PointsChanged -= GameScore_OnPointsChanged;
    }

    private void PauseButton_OnClicked()
    {
        GameStateMachine.TransitionTo(GameState.Paused);
    }

    private void GameScore_OnPointsChanged(int pointsScored)
    {
        var targetPointCount = GameScore.TotalPoints;
        this.Animate(0.5f, EaseFunctions.SineInOut, t =>
        {
            m_ScoreText.text = Mathf.LerpUnclamped(m_PrevPointCount, targetPointCount, t).ToString("0");
        }, () =>
        {
            m_PrevPointCount = GameScore.TotalPoints;
        });
    }

    public Coroutine AnimationRoutine { get; set; }
}
