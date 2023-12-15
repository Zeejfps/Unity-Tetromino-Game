using UnityEngine;

public sealed class UnityAudioSourceMusicPlayer : IMusicPlayer
{
    public bool IsPlaying => m_AudioSource.isPlaying;

    public float Volume
    {
        get => m_AudioSource.volume;
        set => m_AudioSource.volume = value;
    }

    private readonly AudioSource m_AudioSource;

    public UnityAudioSourceMusicPlayer(IMusicClipProvider musicClipProvider)
    {
        var go = new GameObject("[Music Player]");
        m_AudioSource = go.AddComponent<AudioSource>();
        m_AudioSource.loop = true;
        m_AudioSource.clip = musicClipProvider.MusicClip;
        m_AudioSource.volume = 0.5f;
        Object.DontDestroyOnLoad(go);
    }

    public void PlayFromBeginning()
    {
        m_AudioSource.Stop();
        m_AudioSource.Play();
    }

    public void Play()
    {
        m_AudioSource.Play();
    }

    public void Pause()
    {
        m_AudioSource.Pause();
    }
}
