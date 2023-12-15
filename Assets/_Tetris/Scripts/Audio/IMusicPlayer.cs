public interface IMusicPlayer
{
    bool IsPlaying { get; }
    float Volume { get; set; }
    
    void PlayFromBeginning();
    void Play();
    void Pause();
}
