using System;
using UnityEngine;
using UnityEngine.UI;

public sealed class StartScreenController : MonoBehaviour
{
    [SerializeField] private DiContainer m_DiContainer;
    [SerializeField] private GameObject m_StartScreen;
    [SerializeField] private Button m_StartGameButton;

    [Injected] public IGameStateMachine GameStateMachine { get; set; }

    private void Awake()
    {
        m_DiContainer.Inject(this);
    }

    private void Start()
    {
        GameStateMachine.StateChanged += GameStateMachine_OnStateChanged;
        m_StartGameButton.onClick.AddListener(StartGameButton_OnClicked);
        if (GameStateMachine.State == GameState.Start)
            ShowScreen();
    }

    private void OnDestroy()
    {
        GameStateMachine.StateChanged -= GameStateMachine_OnStateChanged;
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
        GameStateMachine.TransitionTo(GameState.Playing);
    }
}
