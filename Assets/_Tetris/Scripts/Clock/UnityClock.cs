using UnityEngine;
using UnityEngine.SceneManagement;

public sealed class UnityClock : IClock
{
    public event ClockTickedCallback Ticked;

    public UnityClock()
    {
        SceneManager.sceneLoaded += SceneManager_OnSceneLoaded;
    }

    private void SceneManager_OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        SceneManager.sceneLoaded -= SceneManager_OnSceneLoaded;
        InitClockRunner();
    }

    private void InitClockRunner()
    {
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