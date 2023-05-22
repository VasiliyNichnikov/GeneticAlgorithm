using System;
using System.Collections.Generic;
using Players;
using SpaceObjects;
using UI.Dialog.InfoAboutMiningPlanet;
using UnityEngine;
using Utils;

namespace Planets.MiningPlayer
{
    public class MiningPlanet : MonoBehaviour, IMiningPlanet
    {
        public event Action<float> OnUpdateRemainingTime;
        public event Action<PlayerType> OnUpdatePlayerType;
        public float CaptureTime => _captureTime;

        // todo желательно вынести в отдельный файл с настройками
        [SerializeField, Header("Время захвата в сек"), Range(0, 100)]
        private float _captureTime;

        [SerializeField] private PlanetDetector _detector;
        [SerializeField] private InfoAboutMiningPlanetDialog _miningPlanetDialog;

        private CapturingPlanet _capturingPlanet;

        private readonly List<IDetectedObject> _detectedObjects = new List<IDetectedObject>();

        private void Start()
        {
            Init();
        }

        private void Init()
        {
            _detector.Init(this);
            _capturingPlanet = new CapturingPlanet(_captureTime);
            _miningPlanetDialog = Main.Instance.DialogManager.GetNewLocationDialog<InfoAboutMiningPlanetDialog>();
            _miningPlanetDialog.Init(this, transform);
            Main.Instance.OnUpdateGame += CustomUpdate;
        }
        public void AddFoundShip(IDetectedObject detectedObject)
        {
            if (_detectedObjects.Contains(detectedObject))
            {
                Debug.LogWarning("Detected object is already contains in list");
                return;
            }

            // todo
            // Проблема, что мы не проверяем уничтожился корабль или нет
            _detectedObjects.Add(detectedObject);
            CheckOutPlanetCapture(detectedObject);
        }

        public void RemoveFoundShip(IDetectedObject detectedObject)
        {
            if (!_detectedObjects.Contains(detectedObject))
            {
                Debug.LogWarning("Detected objects is not contains in list");
                return;
            }

            // todo
            // Проблема, что мы не проверяем уничтожился корабль или нет
            _detectedObjects.Remove(detectedObject);
            CheckOutPlanetCapture(detectedObject);
        }

        // Проверяем захват планеты
        private void CheckOutPlanetCapture(IDetectedObject detectedObject)
        {
            var hasEnemy = IsThereEnemy();
            var hasPlayers = AreThereAnyPlayers();
            
            var currentState = _capturingPlanet.CurrentState;
            switch (currentState)
            {
                case CapturingPlanet.CapturingStates.NotCaptured:
                    if (!hasEnemy && hasPlayers)
                    {
                        _capturingPlanet.StartCapturing(detectedObject.PlayerType);
                    }
                    break;
                case CapturingPlanet.CapturingStates.InProcessOfCapture:
                    if (hasEnemy && hasPlayers)
                    {
                        _capturingPlanet.PauseCapturing();
                    }
                    else if(!hasPlayers)
                    {
                        _capturingPlanet.ResetCapture();
                    }
                    break;
                case CapturingPlanet.CapturingStates.OnPause:
                    if (!hasEnemy && hasPlayers)
                    {
                        _capturingPlanet.ContinueCapturing();
                    }
                    else if(!hasPlayers)
                    {
                        _capturingPlanet.ResetCapture();
                    }
                    break;
                case CapturingPlanet.CapturingStates.Captured:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        // todo работает не правильно
        private bool IsThereEnemy()
        {
            foreach (var playerType in GetPlayerTypesOnPlanet())
            {
                if (_capturingPlanet.ExcitingPlayer == PlayerType.None)
                {
                    return false;
                }

                if (playerType != _capturingPlanet.ExcitingPlayer)
                {
                    return true;
                }
            }

            return false;
        }

        private bool AreThereAnyPlayers()
        {
            return _detectedObjects.Count > 0;
        }
        
        
        private IReadOnlyCollection<PlayerType> GetPlayerTypesOnPlanet()
        {
            if (_detectedObjects.Count == 0)
            {
                return ArraySegment<PlayerType>.Empty;;
            }

            var hasPlayerOne = false;
            var hasPlayerTwo = false;
            foreach (var detectedObject in _detectedObjects)
            {
                hasPlayerOne |= detectedObject.PlayerType == PlayerType.Player1;
                hasPlayerTwo |= detectedObject.PlayerType == PlayerType.Player2;

                if (hasPlayerOne && hasPlayerTwo)
                {
                    return new [] { PlayerType.Player1, PlayerType.Player2 };
                }
            }

            if (hasPlayerOne)
            {
                return new[] { PlayerType.Player1 };
            }

            if (hasPlayerTwo)
            {
                return new[] { PlayerType.Player2 };
            }
            
            return ArraySegment<PlayerType>.Empty;;
        }

        private void OnDestroy()
        {
            _capturingPlanet.Dispose();
            Main.Instance.OnUpdateGame -= CustomUpdate;
        }

        private void CustomUpdate()
        {
            var value = Converter.ConvertFromOneRangeToAnother(0, _captureTime, 0, 1,
                _capturingPlanet.RemainingTimeBeforeCapture);
            if (value > 0)
            {
                OnUpdateRemainingTime?.Invoke(value);
            }
            OnUpdatePlayerType?.Invoke(_capturingPlanet.ExcitingPlayer);
        }
    }
}