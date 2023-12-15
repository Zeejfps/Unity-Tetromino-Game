using UnityEngine;
using UnityEngine.UI;

public sealed class MusicToggleController : Controller
{
    [SerializeField] private Toggle m_Toggle;
    
    [Injected] public IMusicPlayer MusicPlayer { get; set; }

    private void Start()
    {
        m_Toggle.isOn = MusicPlayer.IsPlaying;
        m_Toggle.onValueChanged.AddListener(MusicToggle_OnValueChanged);
    }

    private void OnDestroy()
    {
        m_Toggle.onValueChanged.RemoveListener(MusicToggle_OnValueChanged);
    }
    
    private void MusicToggle_OnValueChanged(bool isOn)
    {
        if (isOn)
            MusicPlayer.Play();
        else
            MusicPlayer.Pause();
    }
}
