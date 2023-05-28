using UnityEngine;
using UnityEngine.AI;

namespace ShipLogic
{
    public class ShipSkinData : MonoBehaviour
    {
        public enum SkinType
        {
            Stealth, // Разведчик
            Fighter, // Истребитель 
            HeavyShip // Тяжелый корабль с большим кол-вом оружия и брони 
        }

        public SkinType Skin => _skinType;
        
        public NavMeshAgent Agent => _agent;
        public GunPoint[] GunPoints => _gunPoints;
        public float RadiusShip => _radiusShip;
        public ShipEffectsManager EffectsManager => _effectsManager;

        [SerializeField] private NavMeshAgent _agent;
        [SerializeField] private GunPoint[] _gunPoints;
        [SerializeField] private float _radiusShip;
        [SerializeField] private SkinType _skinType;
        [SerializeField] private ShipEffectsManager _effectsManager;
        [SerializeField] private MeshRenderer _renderer;
        
        public void SetMaterial(Material material)
        {
            _renderer.material = material;
        }

    }
}