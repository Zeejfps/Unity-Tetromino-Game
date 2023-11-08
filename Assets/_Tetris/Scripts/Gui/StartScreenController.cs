using System;
using UnityEngine;
using UnityEngine.UI;

public sealed class StartScreenController : MonoBehaviour
{
    [SerializeField] private GameStateMachineProvider m_GameStateMachineProvider;
    [SerializeField] private GameObject m_StartScreen;
    [SerializeField] private Button m_StartGameButton;

    private IGameStateMachine m_GameStateMachine;

    private void Awake()
    {
        m_GameStateMachine = m_GameStateMachineProvider.Get();
    }

    private void Start()
    {
        m_GameStateMachine.StateChanged += GameStateMachine_OnStateChanged;
        m_StartGameButton.onClick.AddListener(StartGameButton_OnClicked);
        if (m_GameStateMachine.State == GameState.Start)
            ShowScreen();
    }

    private void OnDestroy()
    {
        m_GameStateMachine.StateChanged -= GameStateMachine_OnStateChanged;
        m_StartGameButton.onClick.RemoveListener(StartGameButton_OnClicked);
    }

    private void GameStateMachine_OnStateChanged(GameState prevstate, GameState currstate)
    {
        if (currstate != GameState.Start)
        {
            HideScreen();
        }
    }

    private void ShowScreen()
    {
        m_StartScreen.SetActive(true);
    }

    private void HideScreen()
    {
        m_StartScreen.SetActive(false);
    }

    private void StartGameButton_OnClicked()
    {
        m_GameStateMachine.TransitionTo(GameState.Playing);
    }
}
