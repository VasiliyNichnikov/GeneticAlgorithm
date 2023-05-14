using System;
using SpaceObjects;
using UnityEngine;

namespace ShipLogic
{
    public class ShipDetector : MonoBehaviour
    {
        [SerializeField] private SphereCollider _collider;
        
        public event Action<IDetectedObject> OnObjectDetected;

        public float Radius => _collider.radius;
        
        private void OnTriggerEnter(Collider other)
        {
            var detectedObject = other.GetComponent<IDetectedObject>(); 
            if (detectedObject == null)
            {
                return;
            }

            OnObjectDetected?.Invoke(detectedObject);
        }
    }
}