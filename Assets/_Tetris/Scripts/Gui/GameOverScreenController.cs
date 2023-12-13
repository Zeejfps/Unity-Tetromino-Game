using TMPro;
using UnityEngine;
using UnityEngine.UI;

public sealed class GameOverScreenController : MonoBehaviour
{
    [SerializeField] private DiContainer m_DiContainer;
    [SerializeField] private GameObject m_GameOverScreen;
    [SerializeField] private TMP_Text m_ScoreText;
    [SerializeField] private TMP_Text m_PersonalBestText;
    [SerializeField] private Button m_PlayAgainButton;

    [Injected] public IGameScore GameScore { get; set; }
    [Injected] public IGameStateMachine GameStateMachine { get; set; }

    private void Awake()
    {
        m_DiContainer.Inject(this);
    }

    private void Start()
    {
        GameStateMachine.StateChanged += GameStateMachine_OnStateChanged;
        if (GameStateMachine.State == GameState.GameOver)
            ShowScreen();
        else
            HideScreen();
        
        m_PlayAgainButton.onClick.AddListener(PlayAgainButton_OnClicked);
    }

    private void OnDestroy()
    {
        GameStateMachine.StateChanged -= GameStateMachine_OnStateChanged;
        m_PlayAgainButton.onClick.RemoveListener(PlayAgainButton_OnClicked);
    }

    private void PlayAgainButton_OnClicked()
    {
        GameStateMachine.TransitionTo(GameState.Playing);
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
        m_ScoreText.text = GameScore.TotalPoints.ToString();
        m_PersonalBestText.text = GameScore.BestPoints.ToString();
        m_GameOverScreen.SetActive(true);
    }

    private void HideScreen()
    {
        m_GameOverScreen.SetActive(false);
    }
}
