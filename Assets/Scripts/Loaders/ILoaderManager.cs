using System;
using System.Collections;

namespace Loaders
{
    public interface ILoaderManager
    {
        void AddLoaderInQueue(ILoader loader);
        void GetAsync<T>(Action<T> onComplete, bool unload) where T : ILoader;
        T Get<T>(bool unload = false) where T : ILoader;
    }
}