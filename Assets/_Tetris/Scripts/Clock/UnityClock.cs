using UnityEngine;
using UnityEngine.SceneManagement;

public sealed class UnityClock : IClock
{
    public event ClockTickedCallback Ticked;

    public UnityClock()
    {
        if (SceneManager.loadedSceneCount > 0)
        {
            InitClockRunner();
        }
        else
        {
            SceneManager.sceneLoaded += SceneManager_OnSceneLoaded;
        }
    }

    private void SceneManager_OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        SceneManager.sceneLoaded -= SceneManager_OnSceneLoaded;
        InitClockRunner();
    }

    private void InitClockRunner()
    {
        if (!Application.isPlaying)
            return;
        
        var go = new GameObject("[Clock Runner]");
        var clockRunner = go.AddComponent<UnityClockRunner>();
        clockRunner.Init(this);
        Object.DontDestroyOnLoad(go);
    }

    public void Tick(float deltaTime)
    {
        Ticked?.Invoke(this);
    }
}