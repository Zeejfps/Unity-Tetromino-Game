using TMPro;
using UnityEngine;
using UnityEngine.UI;

public sealed class GameOverScreenController : MonoBehaviour
{
    [SerializeField] private GameStateMachineProvider m_GameStateMachineProvider;
    [SerializeField] private GameScoreProvider m_GameScoreProvider;
    [SerializeField] private GameObject m_GameOverScreen;
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
            ShowScreen();
        else
            HideScreen();
        
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
            ShowScreen();
        else
            HideScreen();
    }

    private void ShowScreen()
    {
        m_ScoreText.text = m_GameScore.TotalPoints.ToString();
        m_PersonalBestText.text = m_GameScore.BestPoints.ToString();
        m_GameOverScreen.SetActive(true);
    }

    private void HideScreen()
    {
        m_GameOverScreen.SetActive(false);
    }
}
