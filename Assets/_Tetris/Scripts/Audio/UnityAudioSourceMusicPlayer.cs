using UnityEngine;

public sealed class UnityAudioSourceMusicPlayer : IMusicPlayer
{
    private readonly AudioSource m_AudioSource;

    public UnityAudioSourceMusicPlayer(IMusicClipProvider musicClipProvider)
    {
        var go = new GameObject("Music Player");
        m_AudioSource = go.AddComponent<AudioSource>();
        m_AudioSource.loop = true;
        m_AudioSource.clip = musicClipProvider.MusicClip;
        m_AudioSource.Play();
    }
    
    public void PlayOrResume()
    {
        m_AudioSource.Play();
    }

    public void Stop()
    {
        m_AudioSource.Pause();
    }
}
