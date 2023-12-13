
public delegate void ClockTickedCallback(IClock clock);

public interface IClock
{
    event ClockTickedCallback Ticked;
}