using TMPro;
using UnityEngine;
using UnityEngine.UI;

public sealed class GameOverScreenController : MonoBehaviour
{
    [SerializeField] private GameStateMachineProvider m_GameStateMachineProvider;
    [SerializeField] private GameScoreProvider m_GameScoreProvider;
    [SerializeField] private TMP_Text m_ScoreText;
    [SerializeField] private TMP_Text m_PersonalBestText;
    [SerializeField] private Button m_PlayAgainButton;

    private IGameScore m_GameScore;
    private IGameStateMachine m_GameStateMachine;

    private void Awake()
    {
        m_GameStateMachine = m_GameStateMachineProvider.Get();
        m_GameScore = m_GameScoreProvider.Get();
    }

    private void Start()
    {
        m_GameStateMachine.StateChanged += GameStateMachine_OnStateChanged;
        if (m_GameStateMachine.State == GameState.GameOver)
            Show();
        else
            Hide();
        
        m_PlayAgainButton.onClick.AddListener(PlayAgainButton_OnClicked);
    }

    private void OnDestroy()
    {
        m_GameStateMachine.StateChanged -= GameStateMachine_OnStateChanged;
        m_PlayAgainButton.onClick.RemoveListener(PlayAgainButton_OnClicked);
    }

    private void PlayAgainButton_OnClicked()
    {
        m_GameStateMachine.TransitionTo(GameState.Playing);
    }

    private void GameStateMachine_OnStateChanged(GameState prevstate, GameState currstate)
    {
        if (currstate == GameState.GameOver)
            Show();
        else
            Hide();
    }

    private void Show()
    {
        m_ScoreText.text = m_GameScore.TotalPoints.ToString();
        m_PersonalBestText.text = m_GameScore.BestPoints.ToString();
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
