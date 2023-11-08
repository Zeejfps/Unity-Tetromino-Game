using UnityEngine;
using UnityEngine.UI;

public sealed class StartScreenController : MonoBehaviour
{
    [SerializeField] private GameStateMachineProvider m_GameStateMachineProvider;
    [SerializeField] private Button m_StartGameButton;

    private IGameStateMachine m_GameStateMachine;
    
    private void Start()
    {
        m_GameStateMachine = m_GameStateMachineProvider.Get();
        m_GameStateMachine.StateChanged += GameStateMachine_OnStateChanged;
        m_StartGameButton.onClick.AddListener(StartGameButton_OnClicked);
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
            Close();
        }
    }

    private void Close()
    {
        gameObject.SetActive(false);
    }

    private void StartGameButton_OnClicked()
    {
        m_GameStateMachine.TransitionTo(GameState.Playing);
    }
}
