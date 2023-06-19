using System;
using System.Collections.Generic;
using Loaders;
using Map;
using Players;
using Production;
using ShipLogic;
using ShipLogic.Editor;
using SpaceObjects;
using UI.Dialog.InfoAboutPlayerPlanet;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Planets.PlayerPlanet
{
    public class PlayerPlanet : MonoBehaviour, IPlayerPlanet, PlanetClickHandler.IPlanetClick
    {
        public float ThreatLevel { get; private set; }
        public MapObjectType TypeObject => MapObjectType.Planet;
        public PlayerType PlayerType => _player;
        public bool IsStatic => true;
        public Vector3 ObjectPosition => transform.position;
        public Vector3 LeftBottomPosition => _perimeter.LeftBottomPoint;
        public Vector3 RightTopPosition => _perimeter.RightTopPoint;
        public float CurrentGold => _goldManager.CurrentGold;
        public ShipProductionQueue.Production Production => _shipProductionQueue.CurrentProduction;
        public PlanetType Type => PlanetType.Player;
        
        [SerializeField] private PlayerType _player;
        [SerializeField] private ShipCharacteristics[] _characteristics;
        [SerializeField] private Transform _startingPoint;
        [SerializeField] private PerimeterOfObject _perimeter;
        [SerializeField] private MeshRenderer _renderer;
        [SerializeField] private PlanetClickHandler _clickHandler;

        private ShipProductionQueue _shipProductionQueue;
        private PlayerGoldManager _goldManager;
        private InfoAboutPlayerPlanetDialog _playerPlanetDialog;

        public void Init()
        {
            SpaceMap.Map.AddObjectOnMap(this);
            _goldManager = new PlayerGoldManager();
            _shipProductionQueue = new ShipProductionQueue(_player, _startingPoint, _goldManager);
            _clickHandler.Init(this);
            InitMaterial();

            Main.Instance.LoaderManager.LoadAsync<WeightsLoader>(loader =>
            {
                ThreatLevel = loader.GetWeightForSelectedPlanet(Type);
            }, false);
        }

        
        public Vector3 GetPointToApproximate()
        {
            var emptyPoint = SpaceMap.Map.TryGetRandomEmptyPointAroundObject(this, 4, out var isFound);
            if (!isFound)
            {
                Debug.LogWarning("Not found empty point around object");
                return Vector3.zero;
            }

            return emptyPoint;
        }
        
        public IReadOnlyCollection<Vector3> GetPointsInSector()
        {
            throw new NotImplementedException();
        }

        public void AddShipToProduction(ShipType type, Action<ShipBase> onCompleteProduction)
        {
            // todo пока через костыли, потом надо будет переделать
            foreach (var characteristic in _characteristics)
            {
                var data = characteristic.ConvertToShipData();
                if (data.ShipType == type)
                {
                    _shipProductionQueue.AddShipToProduction(data, onCompleteProduction);
                    return;
                }
            }
        }

        public void CreateRandomShipDebug()
        {
            var randomCharacteristics = _characteristics[Random.Range(0, _characteristics.Length)];
            _shipProductionQueue.AddShipToProduction(randomCharacteristics.ConvertToShipData(), null);
        }

        private void InitMaterial()
        {
            var material = Main.Instance.MaterialStorage.GetMaterialForPlanet(_player);
            _renderer.material = material;
        }

        public void OnStartDrag()
        {
            if (_playerPlanetDialog != null)
            {
                return;
            }
            
            _playerPlanetDialog = Main.Instance.DialogManager.GetNewLocationDialog<InfoAboutPlayerPlanetDialog>();
            _playerPlanetDialog.Init(this, _player, transform);
        }

        public void OnEndDrag()
        {
            if (_playerPlanetDialog == null)
            {
                Debug.LogError("Planet dialog is null");
                return;
            }

            _playerPlanetDialog.Hide();
            _playerPlanetDialog = null;
        }

        public void AddExtractedGold(float gold)
        {
            _goldManager.AddGold(gold);
            if (_playerPlanetDialog != null)
            {
                _playerPlanetDialog.UpdateCurrentGold();
            }
        }
    }
}