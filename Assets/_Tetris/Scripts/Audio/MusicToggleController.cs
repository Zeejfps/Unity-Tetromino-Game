using UnityEngine;

public sealed class MusicToggleController : Controller
{
    [SerializeField] private ToggleSwitchWidgetView m_Toggle;
    
    [Injected] public IMusicPlayer MusicPlayer { get; set; }

    private void OnEnable()
    {
        m_Toggle.UpdateInstantly(MusicPlayer.IsPlaying);
        m_Toggle.Clicked += MusicToggle_OnValueChanged;
        MusicPlayer.IsPlayingStateChanged += MusicPlayer_OnIsPlayingStateChanged;
    }

    private void OnDisable()
    {
        MusicPlayer.IsPlayingStateChanged -= MusicPlayer_OnIsPlayingStateChanged;
        m_Toggle.Clicked -= MusicToggle_OnValueChanged;
    }

    private void MusicPlayer_OnIsPlayingStateChanged()
    {
        m_Toggle.UpdateAnimated(MusicPlayer.IsPlaying);
    }

    private void MusicToggle_OnValueChanged(ToggleSwitchWidgetView toggleWidget)
    {
        if (MusicPlayer.IsPlaying)
        {
            MusicPlayer.Pause();
        }
        else
        {
            MusicPlayer.Play();
        }
    }
}
