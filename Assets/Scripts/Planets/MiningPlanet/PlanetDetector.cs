using Map;
using UnityEngine;

namespace Planets.MiningPlanet
{
    public class PlanetDetector : MonoBehaviour
    {
        private IMiningPlanet _miningPlanet;
        
        public void Init(IMiningPlanet miningPlanet)
        {
            _miningPlanet = miningPlanet;
        }

        private void OnTriggerEnter(Collider other)
        {
            var detectedObject = other.GetComponent<IObjectOnMap>();

            if (detectedObject == null)
            {
                return;
            }

            _miningPlanet.AddFoundShip(detectedObject);
        }

        private void OnTriggerExit(Collider other)
        {
            var detectedObject = other.GetComponent<IObjectOnMap>();

            if (detectedObject == null)
            {
                return;
            }
            
            _miningPlanet.RemoveFoundShip(detectedObject);
        }
    }
}