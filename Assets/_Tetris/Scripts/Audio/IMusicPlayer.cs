using System;

public interface IMusicPlayer
{
    event Action IsPlayingStateChanged;
    
    bool IsPlaying { get; }
    float Volume { get; set; }
    
    void PlayFromBeginning();
    void Play();
    void Pause();
}
