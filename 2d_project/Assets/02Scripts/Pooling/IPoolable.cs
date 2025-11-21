namespace Pooling
{
    public interface IPoolable
    {
        void OnCreatedInPool();
        void OnGetFromPool();
        void OnReturnToPool();
    }
}
