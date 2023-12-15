using System;
using UnityEngine;
using Object = UnityEngine.Object;

public sealed class UnityAudioSourceMusicPlayer : IMusicPlayer
{
    public event Action IsPlayingStateChanged;
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
        OnIsPlayingStateChanged();
    }

    public void Play()
    {
        if (m_AudioSource.isPlaying)
            return;
        
        m_AudioSource.Play();
        OnIsPlayingStateChanged();
    }

    public void Pause()
    {
        if (!m_AudioSource.isPlaying)
            return;
        
        m_AudioSource.Pause();
        OnIsPlayingStateChanged();
    }

    private void OnIsPlayingStateChanged()
    {
        IsPlayingStateChanged?.Invoke();
    }
}
