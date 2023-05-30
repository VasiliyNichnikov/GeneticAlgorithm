namespace Loaders
{
    public interface ILoaderManager
    {
        void AddLoaderInQueue(ILoader loader);
        T Get<T>(bool unload = false) where T : ILoader;
    }
}