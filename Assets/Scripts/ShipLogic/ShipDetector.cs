using System;
using SpaceObjects;
using UnityEngine;

namespace ShipLogic
{
    public class ShipDetector : MonoBehaviour
    {
        public event Action<IDetectedObject> OnObjectDetected;

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