using System;
using System.Collections.Generic;
using System.Linq;
using Loaders;
using Map;
using Players;
using ShipLogic.Mining;
using SpaceObjects;
using UI.Dialog.InfoAboutMiningPlanet;
using UnityEngine;
using Utils;
using Random = UnityEngine.Random;

namespace Planets.MiningPlanet
{
    public class MiningPlanet : MonoBehaviour, IMiningPlanet
    {
        public float ThreatLevel { get; private set; }

        public Vector3 GetPointToApproximate()
        {
            var emptyPoint = SpaceMap.Map.TryGetRandomEmptyPointAroundObject(this, 4, out var isFound);
            if (!isFound)
            {
                // todo вот тут тоже zero из-за чего корабли могут плыть к центру карты
                Debug.LogWarning("Not found empty point around object");
                return Vector3.zero;
            }

            return emptyPoint;
        }

        public IReadOnlyCollection<Vector3> GetPointsInSector()
        {
            var pointsInSector = SpaceMap.Map.GetObjectOnMapPointsInSectors(this).ToArray();

            var randomPoints = new List<int>();
            var numberPoints = Mathf.Min(pointsInSector.Length, 10);

            while (randomPoints.Count != numberPoints)
            {
                var randomPoint = Random.Range(0, pointsInSector.Length);
                if (randomPoints.Contains(randomPoint))
                {
                    continue;
                }

                randomPoints.Add(randomPoint);
            }

            return randomPoints.Select(p => pointsInSector[p]).ToList();
        }

        public MapObjectType TypeObject => MapObjectType.Planet;
        public PlayerType PlayerType => PlayerType.None;
        public bool IsStatic => true;
        public Vector3 ObjectPosition => transform.position;
        public Vector3 LeftBottomPosition => _perimeter.LeftBottomPoint;
        public Vector3 RightTopPosition => _perimeter.RightTopPoint;
        public event Action<float> OnUpdateRemainingTimeCatch;
        public event Action<PlayerType> OnUpdatePlayerType;
        public event Action<PlayerType, float> OnUpdateRemainingTimeExtraction;
        public float CaptureTime => _captureTime;
        public PlanetType Type => PlanetType.Mining;

        // todo желательно вынести в отдельный файл с настройками
        [SerializeField, Header("Время захвата в сек"), Range(0, 100)]
        private float _captureTime;

        [SerializeField] private PlanetDetector _detector;
        [SerializeField] private InfoAboutMiningPlanetDialog _miningPlanetDialog;
        [SerializeField] private PerimeterOfObject _perimeter;

        private CapturingPlanet _capturingPlanet;

        /// <summary>
        /// Рассчитываем время добычи
        /// </summary>
        private EarnerCalculator _earnerCalculator;

        private readonly List<IObjectOnMap> _detectedObjects = new();

        private void Start()
        {
            Init();
        }

        private void Init()
        {
            SpaceMap.Map.AddObjectOnMap(this);

            _detector.Init(this);
            _capturingPlanet = new CapturingPlanet(_captureTime);
            _earnerCalculator = new EarnerCalculator();
            _earnerCalculator.OnGoldCollected += OnPlayerCollectedGold;

            _miningPlanetDialog = Main.Instance.DialogManager.GetNewLocationDialog<InfoAboutMiningPlanetDialog>();
            _miningPlanetDialog.Init(this, transform);
            Main.Instance.OnUpdateGame += CustomUpdate;
            Main.Instance.ShipFactory.OnDestroyShip += RemoveFoundShip;
            
            // Загрузка важности планеты
            Main.Instance.LoaderManager.LoadAsync<WeightsLoader>(loader =>
            {
                ThreatLevel = loader.GetWeightForSelectedPlanet(Type);
            }, false);
        }

        public void AddFoundShip(IObjectOnMap detectedObject)
        {
            if (_detectedObjects.Contains(detectedObject))
            {
                Debug.LogWarning("Detected object is already contains in list");
                return;
            }

            _detectedObjects.Add(detectedObject);
            if (detectedObject is ShipMining)
            {
                _earnerCalculator.AddShip(detectedObject.PlayerType);
            }
            
            CheckOutPlanetForTwoPlayers();
        }

        public void RemoveFoundShip(IObjectOnMap detectedObject)
        {
            if (!_detectedObjects.Contains(detectedObject))
            {
                return;
            }

            // Проблема, что мы не проверяем уничтожился корабль или нет
            _detectedObjects.Remove(detectedObject);
            if (detectedObject is ShipMining)
            {
                _earnerCalculator.RemoveShip(detectedObject.PlayerType);
            }
            CheckOutPlanetForTwoPlayers();
        }

        private static void OnPlayerCollectedGold(PlayerType player, float value)
        {
            var planet = Main.Instance.PlanetStorage.GetPlayerPlanet(player);
            planet.AddExtractedGold(value);
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
                    else if (!hasEnemy && hasAllies && _capturingPlanet.ExcitingPlayer != player)
                    {
                        _capturingPlanet.ResetCapture();
                        _capturingPlanet.StartCapturing(player);
                    }

                    break;
                case CapturingPlanet.CapturingStates.Pause:
                    if (!hasEnemy && hasAllies && _capturingPlanet.ExcitingPlayer == player)
                    {
                        _capturingPlanet.ContinueCapturing();
                    }
                    else if (!hasEnemy && hasAllies && _capturingPlanet.ExcitingPlayer != player)
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
                return ArraySegment<PlayerType>.Empty;
                ;
            }

            var hasPlayerOne = false;
            var hasPlayerTwo = false;
            foreach (var detectedObject in _detectedObjects)
            {
                hasPlayerOne |= detectedObject.PlayerType == PlayerType.Player1;
                hasPlayerTwo |= detectedObject.PlayerType == PlayerType.Player2;

                if (hasPlayerOne && hasPlayerTwo)
                {
                    return new[] { PlayerType.Player1, PlayerType.Player2 };
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

            return ArraySegment<PlayerType>.Empty;
            ;
        }

        private void OnDestroy()
        {
            _capturingPlanet.Dispose();
            Main.Instance.OnUpdateGame -= CustomUpdate;
            Main.Instance.ShipFactory.OnDestroyShip -= RemoveFoundShip;
            _earnerCalculator.OnGoldCollected -= OnPlayerCollectedGold;
        }

        private void CustomUpdate()
        {
            var timeLeftCapture = Converter.ConvertFromOneRangeToAnother(0, _captureTime, 0, 1,
                _capturingPlanet.RemainingTimeBeforeCapture);
            if (timeLeftCapture > 0)
            {
                OnUpdateRemainingTimeCatch?.Invoke(timeLeftCapture);
            }

            OnUpdatePlayerType?.Invoke(_capturingPlanet.ExcitingPlayer);

            foreach (var playerType in new[] { PlayerType.Player1, PlayerType.Player2 })
            {
                var timeLeftExtraction = _earnerCalculator.GetTimeLeftForSelectedPlayer(playerType);
                if (timeLeftExtraction == null)
                {
                    OnUpdateRemainingTimeExtraction?.Invoke(playerType, 0.0f);
                    continue;
                }

                OnUpdateRemainingTimeExtraction?.Invoke(playerType, timeLeftExtraction.Value);
            }
        }
    }
}