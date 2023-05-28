using Planets.MiningPlayer;
using Planets.PlayerPlanet;
using UnityEngine;

namespace Players
{
    public class PlayerBrain : MonoBehaviour, IPlayerBrain
    {
        [SerializeField] private PlayerPlanet _planet;
        [SerializeField] private MiningPlanet _miningPlanets;
        
        


        private void Start()
        {
            Init();
        }

        private void Init()
        {
            _planet.Init(this);
        }
        
        public Vector3 GetPointForMovement()
        {
            return _miningPlanets.GetPointToApproximate();
        }
    }
}