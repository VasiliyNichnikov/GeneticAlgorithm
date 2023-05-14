using System.Collections;
using ShipLogic.Stealth;
using UnityEngine;

namespace ShipLogic
{
    public interface IShipGun
    {
        void SetTarget(ITargetToAttack target);

        void Shoot();
    }

    public class ShipGun : IShipGun
    {
        private readonly float _rateOfFire; // Скорострельность
        private readonly float _damage; // Урон
        private readonly float _speedProjectile;

        private readonly PlayerType _playerType;
        private readonly MonoBehaviour _ship;
        private readonly Transform[] _gunpoints; // Точка вылета снаряда
        private ITargetToAttack _target; // Объект, в который будем стрелять

        private IEnumerator _rechargeCheck;
        private float _rechargeTime;

        public ShipGun(MonoBehaviour ship, Transform[] gunpoints, PlayerType playerType, float rateOfFire, float damage, float speedProjectile)
        {
            _ship = ship;
            _gunpoints = gunpoints;
            _rateOfFire = rateOfFire;
            _damage = damage;
            _playerType = playerType;
            _speedProjectile = speedProjectile;
        }

        public void SetTarget(ITargetToAttack target)
        {
            if (Equals(_target, target))
            {
                return;
            }

            _target = target;
        }

        public void Shoot()
        {
            if (_target == null || _rechargeCheck != null)
            {
                return;
            }

            // todo тип игрока нужно определить
            // Стреляем
            var randomGunpoint = GetRandomGunpointPosition();
            Main.Instance.ProjectileFactory.CreateProjectile(
                randomGunpoint, 
                _target.Position,
                _playerType,
                _speedProjectile,
                _damage);
            _rechargeTime = _rateOfFire;
            _rechargeCheck = Recharge();

            // Перезаряжаемся
            _ship.StartCoroutine(_rechargeCheck);
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

        private Vector3 GetRandomGunpointPosition()
        {
            var randomGunpoint = _gunpoints[Random.Range(0, _gunpoints.Length)];
            return randomGunpoint.position;
        }
    }
}