public interface IMusicPlayer
{
    bool IsPlaying { get; }
    void PlayOrResume();
    void Pause();
}
