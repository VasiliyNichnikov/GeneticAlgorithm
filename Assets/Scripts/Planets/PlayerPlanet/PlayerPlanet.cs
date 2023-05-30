using FindingPath;
using HandlerClicks;
using Players;
using ShipLogic.Editor;
using SpaceObjects;
using UI.Dialog.InfoAboutPlayerPlanet;
using UnityEngine;

namespace Planets.PlayerPlanet
{
    public class PlayerPlanet : MonoBehaviour, IPlayerPlanet, PlanetClickHandler.IPlanetClick
    {
        public MapObjectType TypeObject => MapObjectType.Planet;
        public bool IsStatic => true;
        public Vector3 WorldPosition => transform.position;
        public Vector3 LeftBottomPosition => _perimeter.LeftBottomPoint;
        public Vector3 RightTopPosition => _perimeter.RightTopPoint;
        public float CurrentGold => _goldManager.CurrentGold;


        [SerializeField] private ShipCharacteristics[] _characteristics;
        
        [SerializeField] private Transform _startingPoint;
        [SerializeField] private PerimeterOfObject _perimeter;
        [SerializeField] private MeshRenderer _renderer;
        [SerializeField] private PlanetClickHandler _clickHandler;

        private PlayerType _player;
        private PlayerGoldManager _goldManager;
        private InfoAboutPlayerPlanetDialog _playerPlanetDialog;
        private IPlayerBrain _brain;


        public void Init(PlayerType player, IPlayerBrain brain)
        {
            TrafficMap.Map.AddObjectOnMap(this);
            _brain = brain;
            _player = player;
            // todo эти параметры нужно будет брать из настроек
            _goldManager = new PlayerGoldManager(500);
            _clickHandler.Init(this);
            InitMaterial();
        }

        public void CreateRandomShipDebug()
        {
            var randomCharacteristics = _characteristics[Random.Range(0, _characteristics.Length)];
            var ship = Main.Instance.ShipManager.AddShipOnMap(_player, randomCharacteristics.ConvertToShipData());
            ship.transform.position = _startingPoint.position;
            ship.GetCommander().SetPointForMovement(_brain.GetPointForMovement());
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