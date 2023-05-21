using UnityEngine;

namespace ShipLogic
{
    public class GunPoint : MonoBehaviour
    {
        [SerializeField] private ParticleSystem _particle;
        
        public void Activate()
        {
            _particle.Play();
        }

        public void Deactivate()
        {
            _particle.Stop();
        }
    }
}