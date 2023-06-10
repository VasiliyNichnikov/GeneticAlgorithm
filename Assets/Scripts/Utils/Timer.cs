using System;
using UnityEngine;

namespace Utils
{
    public class Timer : IDisposable
    {
        private float _currentTime;
        private readonly float _time;
        private readonly Action _onCompleteAction;

        public bool IsRunning => Math.Abs(_time - _currentTime) > 0.01f;
        
        private Timer(float time, Action onCompleteAction)
        {
            _time = time;
            _onCompleteAction = onCompleteAction;

            Start();
        }

        public static Timer StartTimer(float time, Action onCompleteTimer)
        {
            return new Timer(time, onCompleteTimer);
        }

        private void UpdateTime()
        {
            _currentTime -= Time.deltaTime;
            if (_currentTime <= 0)
            {
                OnComplete();
                Main.Instance.OnUpdateGame -= UpdateTime;
            }
        }

        public void Start()
        {
            _currentTime = _time;
            if (!IsRunning)
            {
                Main.Instance.OnUpdateGame += UpdateTime;
            }
        }

        public void Restart()
        {
            Stop();
            Start();
        }

        public void Stop()
        {
            if (IsRunning)
            {
                Main.Instance.OnUpdateGame -= UpdateTime;
            }
            _currentTime = _time;
        }

        private void OnComplete()
        {
            Stop();
            _onCompleteAction?.Invoke();
        }

        public void Dispose()
        {
            if (IsRunning)
            {
                Main.Instance.OnUpdateGame -= UpdateTime;
            }
        }
    }
}