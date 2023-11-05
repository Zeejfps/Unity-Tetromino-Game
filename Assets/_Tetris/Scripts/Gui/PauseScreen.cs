using UnityEngine;
using UnityEngine.UI;

public sealed class PauseScreen : MonoBehaviour
{
    [SerializeField] private GameStateMachineProvider m_GameStateMachineProvider;
    [SerializeField] private Button m_ResumeButton;
    [SerializeField] private Button m_RestartButton;

    private IGameStateMachine m_GameStateMachine;

    private void Awake()
    {
        m_GameStateMachine = m_GameStateMachineProvider.Get();
    }

    private void Start()
    {
        m_ResumeButton.onClick.AddListener(ResumeButton_OnClicked);
        m_RestartButton.onClick.AddListener(RestartButton_OnClicked);
        m_GameStateMachine.StateChanged += GameStateMachine_OnStateChanged;
        gameObject.SetActive(m_GameStateMachine.State == GameState.Paused);
    }

    private void OnDestroy()
    {
        m_ResumeButton.onClick.RemoveListener(ResumeButton_OnClicked);
        m_RestartButton.onClick.RemoveListener(RestartButton_OnClicked);
        m_GameStateMachine.StateChanged -= GameStateMachine_OnStateChanged;
    }

    private void RestartButton_OnClicked()
    {
        m_GameStateMachine.TransitionTo(GameState.GameOver);
        m_GameStateMachine.TransitionTo(GameState.Playing);
    }

    private void ResumeButton_OnClicked()
    {
        m_GameStateMachine.TransitionTo(GameState.Playing);
    }

    private void GameStateMachine_OnStateChanged(GameState prevstate, GameState currstate)
    {
        gameObject.SetActive(currstate == GameState.Paused);
    }
}
