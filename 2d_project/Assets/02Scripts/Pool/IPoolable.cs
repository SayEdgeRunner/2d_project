namespace Pool
{
    public interface IPoolable
    {
        void OnCreatedInPool();
        void OnGetFromPool();
        void OnReturnToPool();
    }
}
