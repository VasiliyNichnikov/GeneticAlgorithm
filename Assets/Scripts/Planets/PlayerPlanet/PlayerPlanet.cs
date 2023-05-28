using FindingPath;
using Players;
using ShipLogic.Editor;
using SpaceObjects;
using UI.Dialog.InfoAboutPlayerPlanet;
using UnityEngine;

namespace Planets.PlayerPlanet
{
    public class PlayerPlanet : MonoBehaviour, IPlayerPlanet
    {
        public MapObjectType TypeObject => MapObjectType.Planet;
        public bool IsStatic => true;
        public Vector3 WorldPosition => transform.position;
        public Vector3 LeftBottomPosition => _perimeter.LeftBottomPoint;
        public Vector3 RightTopPosition => _perimeter.RightTopPoint;
        
        
        [SerializeField] private ShipCharacteristics[] _characteristics;
        [SerializeField] private PlayerType _type;
        [SerializeField] private Transform _startingPoint;
        [SerializeField] private InfoAboutPlayerPlanetDialog _playerPlanetDialog;
        [SerializeField] private PerimeterOfObject _perimeter;
        
        private IPlayerBrain _brain;

        public void Init(IPlayerBrain brain)
        {
            TrafficMap.Map.AddObjectOnMap(this);
            _brain = brain;
            _playerPlanetDialog = Main.Instance.DialogManager.GetNewLocationDialog<InfoAboutPlayerPlanetDialog>();
            _playerPlanetDialog.Init(this, transform);
        }

        public void CreateRandomShipDebug()
        {
            var randomCharacteristics = _characteristics[Random.Range(0, _characteristics.Length)];
            var ship = Main.Instance.ShipManager.AddShipOnMap(_type, randomCharacteristics.ConvertToShipData());
            ship.transform.position = _startingPoint.position;
            ship.GetCommander().SetPointForMovement(_brain.GetPointForMovement());
        }
    }
}