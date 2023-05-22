using System;
using Players;
using UnityEngine;

namespace Planets.MiningPlayer
{
    public class CapturingPlanet : IDisposable
    {
        public enum CapturingStates
        {
            NotCaptured, // Не захачено
            InProcessOfCapture, // В процессе захвата
            OnPause, // На паузе
            Captured // Захвачено
        }
        
        public float RemainingTimeBeforeCapture => _remainingTimeBeforeCapture;
        public CapturingStates CurrentState => _currentState;

        public PlayerType ExcitingPlayer => _excitingPlayer ?? PlayerType.None;

        private readonly float _captureTime;

        private float _remainingTimeBeforeCapture;

        private bool _countDownStarted;
        private bool _isPaused;
        private PlayerType? _excitingPlayer;
        private CapturingStates _currentState;

        private bool IsPointCaptured => _remainingTimeBeforeCapture <= 0;

        public CapturingPlanet(float captureTime)
        {
            _captureTime = captureTime;
            _currentState = CapturingStates.NotCaptured;
        }

        public void StartCapturing(PlayerType type)
        {
            if (_countDownStarted)
            {
                Debug.LogWarning("Capturing is already started");
                return;
            }

            if (_excitingPlayer == type)
            {
                return;
            }
            
            _excitingPlayer = type;
            _remainingTimeBeforeCapture = _captureTime;
            _countDownStarted = true;
            _isPaused = false;
            _currentState = CapturingStates.InProcessOfCapture;
            Main.Instance.OnUpdateGame += CountDown;
        }

        public void PauseCapturing()
        {
            if (!_countDownStarted)
            {
                Debug.LogWarning("Capturing is not started");
                return;
            }

            _currentState = CapturingStates.OnPause;
            _isPaused = true;
        }

        public void ContinueCapturing()
        {
            if (!_countDownStarted)
            {
                Debug.LogWarning("Capturing is not started");
                return;
            }

            _currentState = CapturingStates.InProcessOfCapture;
            _isPaused = false;
        }

        public void ResetCapture()
        {
            if (!_countDownStarted)
            {
                Debug.LogWarning("Capturing is not started");
                return;
            }

            _countDownStarted = false;
            _excitingPlayer = null;
            _currentState = CapturingStates.NotCaptured;
            Main.Instance.OnUpdateGame -= CountDown;
        }

        private void CompleteCapture()
        {
            _currentState = CapturingStates.Captured;
        }
        
        private void CountDown()
        {
            if (IsPointCaptured || _isPaused)
            {
                return;
            }
            
            _remainingTimeBeforeCapture -= Time.deltaTime;
            if (_remainingTimeBeforeCapture <= 0)
            {
                CompleteCapture();
            }
        }

        public void Dispose()
        {
            if (_countDownStarted)
            {
                ResetCapture();
            }
        }
    }
}