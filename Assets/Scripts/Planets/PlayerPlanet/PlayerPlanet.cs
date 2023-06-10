using System;
using System.Collections.Generic;
using Group;
using Map;
using Players;
using ShipLogic.Editor;
using SpaceObjects;
using UI.Dialog.InfoAboutPlayerPlanet;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Planets.PlayerPlanet
{
    public class PlayerPlanet : MonoBehaviour, IPlayerPlanet, PlanetClickHandler.IPlanetClick
    {
        public float ThreatLevel => 1.0f;
        public MapObjectType TypeObject => MapObjectType.Planet;
        public bool IsStatic => true;
        public Vector3 ObjectPosition => transform.position;
        public Vector3 LeftBottomPosition => _perimeter.LeftBottomPoint;
        public Vector3 RightTopPosition => _perimeter.RightTopPoint;
        public float CurrentGold => _goldManager.CurrentGold;
        public PlayerType Player => _player;

        [SerializeField] private PlayerType _player;
        [SerializeField] private ShipCharacteristics[] _characteristics;
        [SerializeField] private Transform _startingPoint;
        [SerializeField] private PerimeterOfObject _perimeter;
        [SerializeField] private MeshRenderer _renderer;
        [SerializeField] private PlanetClickHandler _clickHandler;
        
        private PlayerGoldManager _goldManager;
        private InfoAboutPlayerPlanetDialog _playerPlanetDialog;

        private void Start()
        {
            Init();
        }

        private void Init()
        {
            SpaceMap.Map.AddObjectOnMap(this);
            // todo эти параметры нужно будет брать из настроек
            _goldManager = new PlayerGoldManager(500);
            _clickHandler.Init(this);
            InitMaterial();
        }

        
        public Vector3 GetPointToApproximate()
        {
            var emptyPoint = SpaceMap.Map.TryGetRandomEmptyPointAroundObject(this, 5, out var isFound);
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
        
        public void CreateRandomShipDebug()
        {
            var randomCharacteristics = _characteristics[Random.Range(0, _characteristics.Length)];
            var ship = Main.Instance.ShipFactory.AddShipOnMap(_player, randomCharacteristics.ConvertToShipData());
            ship.transform.position = _startingPoint.position;
            if (ship.GetCommander() is ISupportedGroup shipInGroup)
            {
                Main.Instance.ShipGroupManager.AddShipInGroup(_player, shipInGroup);
            }
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