using UnityEngine;

[CreateAssetMenu(menuName = "Providers/Game Input Provider")]
public sealed class GameInputProvider : ScriptableObject
{
    [SerializeField] private TouchGestureDetectorProvider m_TouchGestureDetectorProvider;
    [SerializeField] private GameStateMachineProvider m_GameStateMachineProvider;
    
    private IGameInput m_GameInput;
    
    public IGameInput Get()
    {
        if (m_GameInput == null)
        {
            var go = new GameObject("[Game Input Updater]");
            DontDestroyOnLoad(go);
            var inputUpdater = go.AddComponent<GameInputUpdater>();
            
            var touchGestureDetector = m_TouchGestureDetectorProvider.Get();
            var gameStateMachine = m_GameStateMachineProvider.Get();
            
            var moveLeftInput = new MoveLeftInput(gameStateMachine, touchGestureDetector);
            var moveRightInput = new MoveRightInput(gameStateMachine, touchGestureDetector);
            var moveDownInput = new MoveDownInput(gameStateMachine);
            var rotateInput = new RotateInput(gameStateMachine, touchGestureDetector);
            var instantDropInput = new InstantDropInput(gameStateMachine, touchGestureDetector);
            
            inputUpdater.Add(moveLeftInput);
            inputUpdater.Add(moveRightInput);
            inputUpdater.Add(moveDownInput);
            inputUpdater.Add(rotateInput);
            inputUpdater.Add(instantDropInput);
            
            m_GameInput = new GameInput(
                moveLeftInput,
                moveRightInput,
                moveDownInput,
                rotateInput,
                instantDropInput
            );
        }
        return m_GameInput;
    }
}