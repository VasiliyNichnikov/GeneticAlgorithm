using System;
using System.Collections;
using Map;
using SpaceObjects;
using UnityEngine;

namespace ShipLogic
{
    public interface IShipGun : IDisposable
    {
        bool AttackingSelectedTarget(IObjectOnMap selectedTarget);
        void SetTarget(IObjectOnMap target);
        void StartShoot();
        void FinishShoot();
        void Shoot();
    }

    public class ShipGun : IShipGun
    {
        private readonly float _rateOfFire; // Скорострельность
        private readonly float _damage; // Урон
        private readonly float _speedProjectile;
        
        private readonly MonoBehaviour _ship;
        private readonly GunPoint[] _gunpoints; // Точка вылета снаряда
        private IObjectOnMap _target; // Объект, в который будем стрелять

        private IEnumerator _rechargeCheck;
        private float _rechargeTime;

        public ShipGun(MonoBehaviour ship, GunPoint[] gunpoints, float rateOfFire, float damage)
        {
            _ship = ship;
            _gunpoints = gunpoints;
            _rateOfFire = rateOfFire;
            _damage = damage;

            DeactivateGunpoints();
        }

        public bool AttackingSelectedTarget(IObjectOnMap selectedTarget)
        {
            return _target != null && _target.Equals(selectedTarget);
        }

        public void SetTarget(IObjectOnMap target)
        {
            if (Equals(_target, target))
            {
                return;
            }

            _target = target;
        }

        public void StartShoot()
        {
            ActivateGunpoints();
        }

        public void Shoot()
        {
            if (_target == null || _rechargeCheck != null)
            {
                return;
            }
            
            DealDamage();
            _rechargeTime = _rateOfFire;
            _rechargeCheck = Recharge();

            // Перезаряжаемся
            _ship.StartCoroutine(_rechargeCheck);
        }

        public void FinishShoot()
        {
            _target = null;
            DeactivateGunpoints();
        }

        private IEnumerator Recharge()
        {
            // Ожидаем время перезарядки
            while (_rechargeTime > 0)
            {
                _rechargeTime -= Time.deltaTime;
                yield return null;
            }

            _rechargeCheck = null;
        }

        private void ActivateGunpoints()
        {
            foreach (var gunPoint in _gunpoints)
            {
                gunPoint.Activate();
            }
        }

        private void DeactivateGunpoints()
        {
            foreach (var gunPoint in _gunpoints)
            {
                gunPoint.Deactivate();
            }
        }

        private void DealDamage()
        {
            if (_target == null)
            {
                Debug.LogWarning("Not found target.");
                return;
            }
            
            var currentShip = _target as ShipBase;
            if (currentShip == null)
            {
                Debug.LogError("Object is not ship");
                return;
            }
            currentShip.Health.DealDamage(_damage);
        }

        public void Dispose()
        {
            FinishShoot();
            if (_rechargeCheck != null)
            {
                _ship.StopCoroutine(_rechargeCheck);
                _rechargeCheck = null;
            }

            _target = null;
        }
    }
}