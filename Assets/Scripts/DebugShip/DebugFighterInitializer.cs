using Group;
using Players;
using ShipLogic;
using ShipLogic.Editor;
using ShipLogic.Fighter;
using ShipLogic.Stealth;
using UnityEngine;

namespace DebugShip
{
    public class DebugFighterInitializer : MonoBehaviour
    {
        private class DebugBuilderShip: IBuilderShip
        {
            public PlayerType PlayerType { get; private set; }
        
            public ShipData Data { get; private set; }

            public DebugBuilderShip(PlayerType player, ShipCharacteristics debugCharacteristics)
            {
                PlayerType = player;
                Data = debugCharacteristics.ConvertToShipData();
            }
        }
        
        
        [SerializeField] private ShipFighter _shipFighter;
        [SerializeField] private ShipStealth[] _shipStealths;
        [SerializeField] private ShipCharacteristics _characteristics;
        [SerializeField] private Transform _pointForMovement;
        private IShipCommander _commander;

        private void Start()
        {
            InitShip();
        }

        private void InitShip()
        {
            _shipFighter.Init(new DebugBuilderShip(PlayerType.Player1, _characteristics));

            foreach (var shipStealth in _shipStealths)
            {
                shipStealth.Init(new DebugBuilderShip(PlayerType.Player2, _characteristics));
            }

            _shipFighter.GetCommander().SetPointForMovement(new PointForMovement(_pointForMovement.position));
        }
    }
}