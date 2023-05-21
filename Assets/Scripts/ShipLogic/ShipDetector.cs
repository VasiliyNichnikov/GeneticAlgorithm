#nullable enable
using Players;
using SpaceObjects;
using UnityEngine;

namespace ShipLogic
{
    /// <summary>
    /// todo это теперь детектор врагов, поэтому его нужно переименовать и поправить
    /// </summary>
    public class ShipDetector : MonoBehaviour
    {
        [SerializeField] private SphereCollider _collider = null!;
        
        public float Radius => _collider.radius;

        private PlayerType _currentPlayer;
        private IShipCommander _commander = null!;

        public void Init(PlayerType playerType, IShipCommander commander)
        {
            _currentPlayer = playerType;
            _commander = commander;
        }
        
        public void SetRadius(float radius)
        {
            if (radius <= 0)
            {
                Debug.LogError($"Not corrected value radius: {radius}");
                return;
            }
            
            _collider.radius = radius;
        }

        private void OnTriggerEnter(Collider other)
        {
            var detectedShip = other.GetComponent<IDetectedObject>();
            if (detectedShip == null)
            {
                return;
            }

            if (detectedShip.PlayerType == _currentPlayer)
            {
                return;
            }
            
            _commander.AddFoundEnemy(detectedShip);
        }
        

        private void OnTriggerExit(Collider other)
        {
            var detectedShip = other.GetComponent<IDetectedObject>();
            if (detectedShip == null)
            {
                return;
            }
            
            if (detectedShip.PlayerType == _currentPlayer)
            {
                return;
            }

            _commander.RemoveFoundEnemy(detectedShip);
        }
    }
}