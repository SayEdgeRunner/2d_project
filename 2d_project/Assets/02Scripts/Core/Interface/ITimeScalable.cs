namespace Core
{
    public interface ITimeScalable
    {
        void SetTimeScale(float scale);
        void Pause();
        void Resume();
    }
}
