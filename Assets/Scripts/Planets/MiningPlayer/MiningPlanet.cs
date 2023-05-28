using System;
using System.Collections.Generic;
using System.Linq;
using FindingPath;
using Players;
using SpaceObjects;
using UI.Dialog.InfoAboutMiningPlanet;
using UnityEngine;
using Utils;

namespace Planets.MiningPlayer
{
    public class MiningPlanet : MonoBehaviour, IMiningPlanet
    {
        public Vector3 GetPointToApproximate()
        {
            var emptyPoint = TrafficMap.Map.TryGetRandomEmptyPointAroundObject(this, 5, out var isFound);
            if (!isFound)
            {
                Debug.LogWarning("Not found empty point around object");
                return Vector3.zero;
            }

            return emptyPoint;
        }
        
        public MapObjectType TypeObject => MapObjectType.Planet;
        public bool IsStatic => true;
        public Vector3 WorldPosition => transform.position;
        public Vector3 LeftBottomPosition => _perimeter.LeftBottomPoint;
        public Vector3 RightTopPosition => _perimeter.RightTopPoint;
        
        public event Action<float> OnUpdateRemainingTime;
        public event Action<PlayerType> OnUpdatePlayerType;
        public float CaptureTime => _captureTime;

        // todo желательно вынести в отдельный файл с настройками
        [SerializeField, Header("Время захвата в сек"), Range(0, 100)]
        private float _captureTime;

        [SerializeField] private PlanetDetector _detector;
        [SerializeField] private InfoAboutMiningPlanetDialog _miningPlanetDialog;
        [SerializeField] private PerimeterOfObject _perimeter;

        private CapturingPlanet _capturingPlanet;

        private readonly List<IDetectedObject> _detectedObjects = new List<IDetectedObject>();

        private void Start()
        {
            Init();
        }

        private void Init()
        {
            TrafficMap.Map.AddObjectOnMap(this);
            
            _detector.Init(this);
            _capturingPlanet = new CapturingPlanet(_captureTime);
            _miningPlanetDialog = Main.Instance.DialogManager.GetNewLocationDialog<InfoAboutMiningPlanetDialog>();
            _miningPlanetDialog.Init(this, transform);
            Main.Instance.OnUpdateGame += CustomUpdate;
            Main.Instance.ShipManager.OnDestroyShip += RemoveFoundShip;
        }
        public void AddFoundShip(IDetectedObject detectedObject)
        {
            if (_detectedObjects.Contains(detectedObject))
            {
                Debug.LogWarning("Detected object is already contains in list");
                return;
            }
            
            _detectedObjects.Add(detectedObject);
            CheckOutPlanetForTwoPlayers();
        }

        public void RemoveFoundShip(IDetectedObject detectedObject)
        {
            if (!_detectedObjects.Contains(detectedObject))
            {
                return;
            }
            
            // Проблема, что мы не проверяем уничтожился корабль или нет
            _detectedObjects.Remove(detectedObject);
            CheckOutPlanetForTwoPlayers();
        }

        private void CheckOutPlanetForTwoPlayers()
        {
            CheckOutPlanetCapture(PlayerType.Player1);
            CheckOutPlanetCapture(PlayerType.Player2);
        }
        
        // Проверяем захват планеты
        private void CheckOutPlanetCapture(PlayerType player)
        {
            var hasEnemy = IsThereEnemy(player);
            var hasAllies = AreThereAnyAllies();
            
            var currentState = _capturingPlanet.CurrentState;
            switch (currentState)
            {
                case CapturingPlanet.CapturingStates.NotCaptured:
                    if (!hasEnemy && hasAllies)
                    {
                        _capturingPlanet.StartCapturing(player);
                    }
                    break;
                case CapturingPlanet.CapturingStates.InProcessOfCapture:
                    if (hasEnemy && hasAllies && _capturingPlanet.ExcitingPlayer == player)
                    {
                        _capturingPlanet.PauseCapturing();
                    }
                    else if(!hasEnemy && hasAllies && _capturingPlanet.ExcitingPlayer != player)
                    {
                        _capturingPlanet.ResetCapture();
                        _capturingPlanet.StartCapturing(player);
                    }
                    break;
                case CapturingPlanet.CapturingStates.OnPause:
                    if (!hasEnemy && hasAllies && _capturingPlanet.ExcitingPlayer == player)
                    {
                        _capturingPlanet.ContinueCapturing();
                    }
                    else if(!hasEnemy && hasAllies && _capturingPlanet.ExcitingPlayer != player)
                    {
                        _capturingPlanet.ResetCapture();
                        _capturingPlanet.StartCapturing(player);
                    }
                    break;
                case CapturingPlanet.CapturingStates.Captured:
                    if (_capturingPlanet.ExcitingPlayer != player && !hasEnemy && hasAllies)
                    {
                        _capturingPlanet.ResetCapture();
                        _capturingPlanet.StartCapturing(player);
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        private bool IsThereEnemy(PlayerType player)
        {
            var playersOnPlanet = new List<PlayerType>();
            foreach (var playerType in GetPlayerTypesOnPlanet())
            {
                if (!playersOnPlanet.Contains(playerType))
                {
                    playersOnPlanet.Add(playerType);
                }
            }

            return playersOnPlanet.Any(type => type != player);
        }

        private bool AreThereAnyAllies()
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
            Main.Instance.ShipManager.OnDestroyShip -= RemoveFoundShip;
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