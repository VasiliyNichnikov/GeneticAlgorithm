using System;
using System.Collections;
using ShipLogic;
using ShipLogic.Stealth;
using UnityEngine;

namespace ProjectileLogic
{
    public class ShipProjectile : MonoBehaviour
    {
        private float _damage;
        private float _speed;
        private PlayerType _type;

        private bool _isInitialized;
        
        public void Init(float speed, float damage, PlayerType type)
        {
            if (_isInitialized)
            {
                return;
            }

            _type = type;
            _damage = damage;
            _speed = speed;
            _isInitialized = true;
            
            StartCoroutine(LaunchFly());
        }

        private IEnumerator LaunchFly()
        {
            while (true)
            {
                var movement = Vector3.forward * _speed * Time.deltaTime;
                transform.Translate(movement);
                yield return null;
            }
        }

        
        private void OnTriggerEnter(Collider other)
        {
            var target = other.GetComponent<ITargetToAttack>();
            if (target == null || target.PlayerType == _type || !target.IsShip)
            {
                return;
            }

            var currentShip = target as ShipBase;
            if (currentShip == null)
            {
                Debug.LogError("Object is not ship");
                return;
            }
            currentShip.GetHealth().DealDamage(_damage);
            // todo нужно будет перенести в pool
            Destroy(gameObject);
        }
    }
}