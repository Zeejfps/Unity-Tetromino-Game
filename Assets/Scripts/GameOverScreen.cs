using System;
using UnityEngine;
using UnityEngine.UI;

public sealed class GameOverScreen : MonoBehaviour
{
    [SerializeField] private GameStateMachineProvider m_GameStateMachineProvider;
    [SerializeField] private Button m_PlayAgainButton;
    
    private IGameStateMachine m_GameStateMachine;

    private void Awake()
    {
        m_GameStateMachine = m_GameStateMachineProvider.Get();
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
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
