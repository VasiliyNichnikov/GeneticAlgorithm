using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Loaders
{
    public class LoaderManager : ILoaderManager
    {
        private readonly Dictionary<Type, ILoader> _uploaded = new();

        private readonly Queue<ILoader> _unloadedData = new();
        private readonly MonoBehaviour _mainBehaviour;

        public void LoadAsync<T>(Action<T> onComplete, bool unload) where T : ILoader
        {
            _mainBehaviour.StartCoroutine(LoadData(onComplete, unload));
        }

        private IEnumerator LoadData<T>(Action<T> onComplete, bool unload) where T : ILoader
        {
            var loadedData = Get<T>(unload);
            while (loadedData == null)
            {
                loadedData = Get<T>(unload);
                yield return null;
            }
            
            onComplete?.Invoke(loadedData);
        }

        public T Get<T>(bool unload) where T : ILoader
        {
            if (_uploaded.ContainsKey(typeof(T)))
            {
                var data = _uploaded[typeof(T)];
                if (unload)
                {
                    _uploaded.Remove(typeof(T));
                }

                return (T)data;
            }

            return default;
        }

        public LoaderManager(MonoBehaviour mainBehaviour)
        {
            _mainBehaviour = mainBehaviour;
            _mainBehaviour.StartCoroutine(CheckData());
        }

        public void AddLoaderInQueue(ILoader loader)
        {
            if (_unloadedData.Contains(loader))
            {
                Debug.LogWarning("Loader contains in unloaded data");
                return;
            }

            if (_uploaded.ContainsKey(loader.GetType()))
            {
                Debug.LogWarning("Loader contains in loaded data");
                return;
            }

            _unloadedData.Enqueue(loader);
        }

        private IEnumerator CheckData()
        {
            while (true)
            {
                yield return CheckUnloadedData();
            }
        }

        private IEnumerator CheckUnloadedData()
        {
            while (_unloadedData.Count > 0)
            {
                var data = _unloadedData.Dequeue();
                if (_uploaded.ContainsKey(data.GetType()))
                {
                    Debug.LogWarning("Data contains in loaded data");
                    continue;
                }

                data.Load();

                _uploaded.Add(data.GetType(), data);
                yield return null;
            }
        }
    }
}