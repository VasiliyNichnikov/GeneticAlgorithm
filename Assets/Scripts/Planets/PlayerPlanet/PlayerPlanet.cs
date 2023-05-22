using Players;
using ShipLogic.Editor;
using UI.Dialog.InfoAboutPlayerPlanet;
using UnityEngine;

namespace Planets.PlayerPlanet
{
    public class PlayerPlanet : MonoBehaviour, IPlayerPlanet
    {
        [SerializeField] private ShipCharacteristics[] _characteristics;
        [SerializeField] private PlayerType _type;
        [SerializeField] private Transform _startingPoint;
        [SerializeField] private InfoAboutPlayerPlanetDialog _playerPlanetDialog;


        private IPlayerBrain _brain;

        public void Init(IPlayerBrain brain)
        {
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