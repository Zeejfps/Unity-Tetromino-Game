using UnityEngine;

[CreateAssetMenu(menuName = "Providers/Game Input Provider")]
public sealed class GameInputProvider : ScriptableObject
{
    [SerializeField] private TouchGestureDetectorProvider m_TouchGestureDetectorProvider;
    
    private IGameInput m_GameInput;
    
    public IGameInput Get()
    {
        if (m_GameInput == null)
        {
            var go = new GameObject("[Game Input Updater]");
            DontDestroyOnLoad(go);
            var inputUpdater = go.AddComponent<GameInputUpdater>();
            
            var touchGestureDetector = m_TouchGestureDetectorProvider.Get();
            
            var moveLeftInput = new MoveLeftInput(touchGestureDetector);
            var moveRightInput = new MoveRightInput(touchGestureDetector);
            var moveDownInput = new MoveDownInput();
            var rotateInput = new RotateInput(touchGestureDetector);
            var instantDropInput = new InstantDropInput(touchGestureDetector);
            
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