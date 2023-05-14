using System;
using ShipLogic.Stealth;
using SpaceObjects;
using UnityEngine;

namespace ShipLogic
{
    public abstract class ShipBase : MonoBehaviour, ITargetToAttack, IDisposable
    {
        protected float MinimumSpeed => _minimumSpeed;
        protected float MaximumSpeed => _maximumSpeed;
        protected float Weight => _weight;
        public DetectedObjectType ObjectType => DetectedObjectType.Ship;
        public bool IsShip => true;
        public PlayerType PlayerType => _playerType;

        public Vector3 Position => transform.position;
        public bool IsDead => Health.IsDead;


        /// <summary>
        /// todo все параметры будут задаваться ботом
        /// </summary>
        [SerializeField, Header("Минимальная скорость")]
        private float _minimumSpeed;

        [SerializeField, Header("Максимальная скорость")]
        private float _maximumSpeed;

        [SerializeField, Header("Скорость вращения")]
        private float _rotationSpeed;

        [SerializeField, Header("Вес корабля (В тоннах)")]
        private float _weight;

        [SerializeField, Header("Минимальный угол вращения")]
        private float _minimumAngleRotation;

        [SerializeField, Header("Радиус корабля")]
        private float _radius;

        [SerializeField] private PlayerType _playerType;

        [Space] [SerializeField, Header("Скорострельность")]
        private float _rateOfFire;

        [SerializeField] private float _damage;
        [SerializeField] private float _speedProjectile;
        [SerializeField] private Transform[] _gunpoints;
        [Space] [SerializeField] private float _maxHealth;
        [SerializeField] private float _minHealth;
        [SerializeField] private float _maxArmor;
        [SerializeField] private float _minArmor;

        [SerializeField, Header("Процент поглощения урона"), Range(0, 100)]
        private float _percentageOfArmorAbsorption;


        [SerializeField] private ShipDetector _detector;

        private IShipCommander _commander;

        protected ShipEngine Engine { get; private set; }
        protected ShipGun Gun { get; private set; }
        protected ShipHealth Health { get; private set; }

        public void Init(IShipCommander commander)
        {
            Engine = new ShipEngine(transform,
                _radius,
                OnBoostSpeed,
                OnSlowingDownSpeed,
                _minimumAngleRotation,
                _minimumSpeed,
                _rotationSpeed);
            Gun = new ShipGun(this, _gunpoints, _playerType, _rateOfFire, _damage, _speedProjectile);
            Health = new ShipHealth(_minHealth, _maxHealth, _minArmor, _maxArmor, _percentageOfArmorAbsorption);

            _commander = commander;
            _detector.OnObjectDetected += _commander.SendDetectedObject;
        }

        /// <summary>
        /// Ускорение корабля
        /// </summary>
        protected abstract float OnBoostSpeed(float currentSpeed);

        /// <summary>
        /// Замедление коробля
        /// </summary>
        protected abstract float OnSlowingDownSpeed(float currentSpeed);

        public bool SeeOtherShip(ITargetToAttack ship)
        {
            var direction = ship.Position - Position;
            direction.y = 0.0f;
            var angleRotation = Vector3.Angle(direction, transform.forward);
            // todo константы в настройки
            // думаю эти значения тоже нужно подбирать с помощью алгоритм, так как они зависят на результат сражений
            return angleRotation <= 10f && Vector3.Distance(Position, ship.Position) <= 70f;
        }

        /// <summary>
        /// Подготовка к бою
        /// </summary>
        /// <param name="shipEnemy">Корабль врага</param>
        /// <param name="stack">если True, значит корабль застрял, нужно менять маршрут</param>
        public abstract void PreparingForBattle(ITargetToAttack shipEnemy, out bool stack);

        public virtual void TurnOffEngine()
        {
        }

        public virtual void TurnOnEngine()
        {
        }

        public IShipEngine GetEngine()
        {
            return Engine;
        }

        public IShipGun GetGun()
        {
            return Gun;
        }

        public IShipHealth GetHealth()
        {
            return Health;
        }

        public void OnDestroy()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (_detector == null)
            {
                return;
            }
            _detector.OnObjectDetected -= _commander.SendDetectedObject;
        }

#if UNITY_EDITOR

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = new Color(1, 0, 0, 0.25f);
            Gizmos.DrawSphere(transform.position, _radius);
        }
#endif
    }
}